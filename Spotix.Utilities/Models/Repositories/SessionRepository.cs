using Microsoft.EntityFrameworkCore;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.Repositories
{
	public class SessionRepository : ISessionRepository
	{
		private AppDbContext dbContext;

		public SessionRepository(AppDbContext dbContext)
		{
			this.dbContext = dbContext;
		}
		public Task<Session> CreateAsync(Session model)
		{
			throw new NotImplementedException();
		}

		public Task<Session?> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<List<Session>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<List<SessionDto>> GetByEventIdAsync(int eventId)
		{
			return await dbContext.Sessions
				.Include(x => x.Areas)
				.Select(session => new SessionDto
				{
					Id = session.Id,
					Name = session.Name,
					SessionTime = session.SessionTime,
					AvailableTime = session.AvailableTime,
					PublishTime = session.PublishTime,
					Published = session.Published,
					EventId = session.EventId,
					Areas = session.Areas.Select(area => new AreaDto
					{
						Id = area.Id,
						Name = area.Name,
						Price = area.Price,
						SessionId = area.SessionId,
						Qty = area.Qty,
						DisplayOrder = area.DisplayOrder,
						TicketsLeftCount = area.Tickets.Count(t => !t.IsSold)
					}).ToList()
				})
				.Where(x => x.EventId == eventId)
				.ToListAsync();
		}
		public async Task<SessionDto?> GetByIdAsync(int id)
		{
			return await dbContext.Sessions
				.Include(x => x.Areas)
				.Select(session => new SessionDto
				{
					Id = session.Id,
					Name = session.Name,
					SessionTime = session.SessionTime,
					AvailableTime = session.AvailableTime,
					PublishTime = session.PublishTime,
					Published = session.Published,
					EventId = session.EventId,
					Areas = session.Areas.Select(area => new AreaDto
					{
						Id = area.Id,
						Name = area.Name,
						Price = area.Price,
						SessionId = area.SessionId,
						Qty = area.Qty,
						DisplayOrder = area.DisplayOrder,
						TicketsLeftCount = area.Tickets.Count(t => !t.IsSold)
					}).ToList()
				})
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public Task<Session?> UpdateAsync(int id, Session model)
		{
			throw new NotImplementedException();
		}
	}
}
