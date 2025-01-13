using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.Interfaces
{
	public interface ITicketRepository
	{
		Task<List<Ticket>> GetAllAsync();
		Task<Ticket?> GetByIdAsync(int id);
		Task<List<Ticket>> UpdateByIdsAsync(IEnumerable<int> ids, int orderId);
		Task CreateAsync(List<Ticket> model);
		Task<Ticket?> UpdateAsync(int id, Ticket model);
		Task<Ticket?> DeleteAsync(int id);
	}
}
