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
	public class TicketRepository : ITicketRepository
	{
		private readonly AppDbContext dbContext;

		public TicketRepository(AppDbContext dbContext)
		{
			this.dbContext = dbContext;
		}
		public async Task CreateAsync(List<Ticket> tickets)
		{
			await dbContext.Tickets.AddRangeAsync(tickets);
			await dbContext.SaveChangesAsync();
		}

		public Task<Ticket?> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<List<Ticket>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Ticket?> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<Ticket?> UpdateAsync(int id, Ticket model)
		{
			throw new NotImplementedException();
		}
	}
}
