using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spotix.API.CustomActionFilter;
using Spotix.API.Exceptions;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using Spotix.Utilities.Models.Repositories;
using Spotix.Utilities.Models.Services;
using Spotix.Utilities.Models.ViewModels;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly OrderService orderService;
		private readonly IMapper mapper;

		public OrdersController(OrderService orderService, IMapper mapper)
		{
			this.orderService = orderService;
			this.mapper = mapper;
		}

		// 新增訂單
		[HttpPost]
		[ValidateModel]
		[Authorize]
		public async Task<IActionResult> Create(OrderVM model)
		{
			var order = mapper.Map<Order>(model);

			var orderModel = await orderService.Create(order);

			var orderDto = mapper.Map<OrderDto>(model);

			return CreatedAtAction(nameof(Create), null);
		}

		[HttpGet]
		[Route("{id:int}")]
		public async Task<IActionResult> GetById(int id)
		{
			var orderModel = await orderService.GetById(id);
			if (orderModel == null) throw new ResourceNotFoundException("搜尋失敗");
			HttpContext.Items["message"] = "搜尋成功";
			var orderDto = mapper.Map<OrderDto>(orderModel);
			return Ok(orderDto);
		}
	}
}
