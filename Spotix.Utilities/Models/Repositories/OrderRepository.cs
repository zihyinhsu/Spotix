using Microsoft.EntityFrameworkCore;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly AppDbContext dbContext;
		public OrderRepository(AppDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<Order> CreateAsync(Order model)
		{
			await dbContext.Orders.AddAsync(model);
			await dbContext.SaveChangesAsync();
			return model;
		}

		public Task<Order?> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<List<Order>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<Order?> GetByIdAsync(int id)
		{
			return await dbContext.Orders.Include(x => x.Tickets).FirstOrDefaultAsync(x => x.Id == id);
		}
		
		public async Task<Order?> GetByOrderNumberAsync(string OrderNumber)
		{
			return await dbContext.Orders.Include(x => x.Tickets).FirstOrDefaultAsync(x => x.OrderNumber == OrderNumber);
		}

		public async Task<List<Order>> GetByUserIdAsync(string userId)
		{
			return await dbContext.Orders.Include(x => x.Tickets).Where(x => x.UserId == userId).ToListAsync();
		}
		

		public Task<Order?> UpdateAsync(int id, Order model)
		{
			throw new NotImplementedException();
		}
	}
}
