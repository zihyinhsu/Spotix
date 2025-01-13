using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Spotix.API.CustomActionFilter;
using Spotix.API.Exceptions;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.DTOs.Messages;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using Spotix.Utilities.Models.Repositories;
using Spotix.Utilities.Models.Services;
using Spotix.Utilities.Models.ViewModels;
using Spotix.Utilities.NewebPay;
using Spotix.Utilities.Providers;
using System.Collections.Generic;
using System.Security.Claims;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly UserManager<User> userManager;

		private readonly OrderService orderService;
		private readonly ITicketRepository ticketRepository;
		private readonly IMapper mapper;
		private readonly LineBotService lineBotService;

		private readonly IDistributedCache distributedCache;


		public OrdersController(OrderService orderService, IMapper mapper, UserManager<User> userManager, IDistributedCache distributedCache, ITicketRepository ticketRepository, LineBotService lineBotService)
		{
			this.orderService = orderService;
			this.mapper = mapper;
			this.userManager = userManager;
			this.distributedCache = distributedCache;
			this.ticketRepository = ticketRepository;
			this.lineBotService = lineBotService;
		}

		//// 新增訂單
		//[HttpPost]
		//[ValidateModel]
		//[Authorize]
		//public async Task<IActionResult> Create(OrderVM model)
		//{
		//	var order = mapper.Map<Order>(model);

		//	var orderModel = await orderService.Create(order);

		//	var orderDto = mapper.Map<OrderDto>(model);

		//	return CreatedAtAction(nameof(Create), null);
		//}

		//[HttpGet("ByUserId")]
		//[ValidateModel]
		//[Authorize]
		//public async Task<IActionResult> GetByUserId()
		//{
		//	var userEmail = User.FindFirstValue(ClaimTypes.Email);

		//	var user = await userManager.FindByEmailAsync(userEmail);

		//	var orderModel = await orderService.GetByUserId(user.Id);
		//	HttpContext.Items["message"] = "搜尋成功";
		//	var ordernDto = mapper.Map<List<OrderDto>>(orderModel);
		//	return Ok(ordernDto);
		//}

		[HttpGet]
		[Route("{id:int}")]
		[Authorize]
		public async Task<IActionResult> GetById(int id)
		{
			var orderModel = await orderService.GetById(id);
			if (orderModel == null) throw new ResourceNotFoundException("搜尋失敗");
			HttpContext.Items["message"] = "搜尋成功";
			var orderDto = mapper.Map<OrderDto>(orderModel);
			return Ok(orderDto);
		}

		[HttpPost]
		[Route("Encrypt")]
		[Authorize]
		[ValidateModel]
		public async Task<ActionResult<EncryptOrderDto>> Encrypt([FromBody] OrderVM model)
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			var user = await userManager.FindByEmailAsync(userEmail);

			var encryptOrder = orderService.Encrypt(model, userEmail, user.Id);

			// 暫存加密的訂單資訊到 SQL Server 快取
			var cacheKey = encryptOrder.MerchantOrderNo.ToString();

			//var cacheValue = JsonConvert.SerializeObject(encryptOrder);
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // 設定快取過期時間
			};
			await distributedCache.SetObjectAsJsonAsync(cacheKey, encryptOrder, cacheOptions);

			return Ok(encryptOrder);
		}

		[HttpPost]
		[Route("Notify")]
		public async Task<ActionResult<EncryptOrderDto?>> Notify([FromForm] NewebpayNotifyDto model)
		{
			var aesdecrypt = orderService.Notify(model);
			if (aesdecrypt == null) throw new ResourceNotFoundException("付款失敗：TradeSha 不一致");

			// 從 SQL Server 快取中讀取相應的訂單資訊

			var cacheKey = aesdecrypt.Result.MerchantOrderNo.ToString();
			var orderInLocal = await distributedCache.GetObjectFromJsonAsync<EncryptOrderDto>(cacheKey);


			Console.WriteLine(orderInLocal);


			// 檢查訂單是否已經存在
			var existingOrder = await orderService.GetByOrderNumber(orderInLocal.OrderNumber.ToString());
			if (existingOrder == null)
			{
				// 新增訂單
				var order = new Order
				{
					CreatedTime = orderInLocal.CreatedTime,
					Total = orderInLocal.Total,
					UserId = orderInLocal.UserId,
					OrderNumber = orderInLocal.OrderNumber
				};


				await orderService.Create(order);

				var ticktIds = orderInLocal.TicketIds.Split(',').Select(int.Parse).ToList();

				// 創立 order 之後把票填上 existingOrder 的Id 和 isSold = true

				var tickets = await ticketRepository.UpdateByIdsAsync(ticktIds, order.Id);

				Console.WriteLine(tickets);

				// 傳送 line bot 通知
				var message = new List<TextMessageDto>
				{
					new TextMessageDto
					{
						Text = "您的訂單已成立，請至訂單頁面確認"
					}
				};

				var user = await userManager.FindByIdAsync(order.UserId);
				if (user != null)
				{
					try
					{
						await lineBotService.PushMessageAsync(user.LineId, message);
					}
					catch (Exception ex)
					{
						return BadRequest(ex.Message);
					}
				}

			}



			return Ok(aesdecrypt);
		}
	}
}
