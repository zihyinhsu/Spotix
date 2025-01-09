using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.Interfaces
{
	public interface IOrderRepository
	{
		Task<List<Order>> GetAllAsync();
		Task<Order?> GetByIdAsync(int id);
		Task<List<Order>> GetByUserIdAsync(string userId);
		Task<Order> CreateAsync(Order model);
		Task<Order?> UpdateAsync(int id, Order model);
		Task<Order?> DeleteAsync(int id);
	}
}
