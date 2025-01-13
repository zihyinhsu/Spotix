using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using Spotix.Utilities.Models.ViewModels;
using Spotix.Utilities.NewebPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Spotix.Utilities.Models.Services
{
	public class OrderService
	{
		private readonly IOrderRepository orderRepository;

		public OrderService(IOrderRepository orderRepository)
		{
			this.orderRepository = orderRepository;
		}

		public async Task<Order> Create(Order model)
		{
			// 要填寫底下的票的userId 和 isSold
			return await orderRepository.CreateAsync(model);
		}
		public async Task<Order?> GetById(int id)
		{
			return await orderRepository.GetByIdAsync(id);
		}
		
		public async Task<List<Order>> GetByUserId(string userId)
		{
			return await orderRepository.GetByUserIdAsync(userId);
		}

		public async Task<Order?> GetByOrderNumber(string OrderNumber)
		{
			return await orderRepository.GetByOrderNumberAsync(OrderNumber);
		}


		public EncryptOrderDto Encrypt(OrderVM model, string userEmail, string userId)
		{

			// 使用 Unix Timestamp 作為訂單編號（金流也需要加入時間戳記）
			var TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

			var order = new EncryptOrderDto
			{
				TimeStamp = TimeStamp,
				MerchantID = NewebpaySettings.MerchantID,
				Amt = int.Parse(model.Total.ToString()),
				MerchantOrderNo = TimeStamp,
				Version = NewebpaySettings.Version,
				NotifyUrl = NewebpaySettings.NotifyUrl,
				ReturnUrl = NewebpaySettings.ReturnUrl,
				Email = userEmail,
				ItemDesc = model.ItemDesc.ToString(),
				UserId = userId,
				CreatedTime = model.CreatedTime,
				OrderNumber = Guid.NewGuid().ToString(),
				Total = int.Parse(model.Total.ToString()),
				TicketIds = model.TicketIds
			};

			// 進行訂單加密
			// 加密第一段字串，此段主要是提供交易內容給予藍新金流
			var aesEncrypt = Crypto.AESEncrypt(order);
			// 使用 HASH 再次 SHA 加密字串，作為驗證使用
			var shaEncrypt = Crypto.SHA256Encrypt(aesEncrypt);
			order.TradeInfo = aesEncrypt;
			order.TradeSha = shaEncrypt;



			return order;
		}
		public NewebpayNotifyResponse Notify(NewebpayNotifyDto model)
		{
			// 解密交易內容
			if (model.Status == "SUCCESS")
			{
				var aesdecrypt = Crypto.AESDecrypt(model.TradeInfo);

				//// 使用 HASH 再次 SHA 加密字串，確保比對一致（確保不正確的請求觸發交易成功）
				//var thisShaEncrypt = Crypto.SHA256Encrypt(model.TradeInfo);

				//if (thisShaEncrypt != model.TradeSha)
				//{
				//	// 如果 SHA 加密字串不一致，返回 null
				//	return null;
				//}

				Console.WriteLine(aesdecrypt);


				// 返回解密後的訂單資訊
				return aesdecrypt;
			}

			// 如果交易狀態不是 SUCCESS，返回 null
			return null;
		}
	}
}
