using Microsoft.AspNetCore.Mvc;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public async Task<List<Order>> GetByUserId(string id)
		{
			return await orderRepository.GetByUserIdAsync(id);
		}
	}
}
