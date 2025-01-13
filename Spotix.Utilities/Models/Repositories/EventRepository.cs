using AutoMapper;
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
	public class EventRepository : IEventRepository
	{
		private readonly AppDbContext dbContext;
		private readonly IMapper mapper;

		public EventRepository(AppDbContext dbContext)
		{
			this.dbContext = dbContext;

			// 手動配置 AutoMapper
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<Event, Event>()
					.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
			});

			mapper = config.CreateMapper();
		}

		public async void CreateAsync(Event model)
		{
			await dbContext.Events.AddAsync(model);
			await dbContext.SaveChangesAsync();
		}

		public async Task<Event?> DeleteAsync(int id)
		{
			var existEvent = await GetByIdAsync(id);

			if (existEvent == null) return null;

			dbContext.Events.Remove(existEvent);
			await dbContext.SaveChangesAsync();
			return existEvent;
		}

		// 還要根據月份篩選
		public async Task<List<Event>> GetAllAsync(string? filterQuery = null, int? year = null, int? month = null, string? sortBy = null, int pageNumber = 1, int pageSize = 10)
		{
			// AsQueryable 使查詢在實際執行之前不會被評估，這樣可以在需要時動態構建查詢。
			var events = dbContext.Events.Include("Place").Include("Sessions").AsQueryable();

			// Filtering
			if (!string.IsNullOrWhiteSpace(filterQuery))
			{
				events = events.Where(e => e.Name.Contains(filterQuery) && e.Published == true);
			}
			// Sorting
			if (!string.IsNullOrWhiteSpace(sortBy))
			{
				if (sortBy.Equals("SessionTime", StringComparison.OrdinalIgnoreCase))
				{
					events = events.OrderByDescending(e => e.Sessions.OrderByDescending(s => s.SessionTime).FirstOrDefault().SessionTime);
				}
				else if (sortBy.Equals("AvailableTime", StringComparison.OrdinalIgnoreCase))
				{
					events = events.OrderByDescending(e => e.Sessions.OrderByDescending(s => s.AvailableTime).FirstOrDefault().AvailableTime);
				}
				else if (sortBy.Equals("PublishTime", StringComparison.OrdinalIgnoreCase))
				{
					events = events.OrderByDescending(e => e.Sessions.OrderByDescending(s => s.PublishTime).FirstOrDefault().PublishTime);
				}
			}


			// 篩選年份
			if (year.HasValue)
			{
				events = events.Where(e => e.Sessions.Any(s => s.SessionTime.Year == year.Value));
			}

			// 篩選月份
			if (month.HasValue)
			{
				events = events.Where(e => e.Sessions.Any(s => s.SessionTime.Month == month.Value));
			}

			// Pagination
			var skipResults = (pageNumber - 1) * pageSize;

			return await events.Skip(skipResults).Take(pageSize).ToListAsync();
		}

		public async Task<Event?> GetByIdAsync(int id)
		{
			return await dbContext.Events
			  .Include(e => e.Place)
			  .Include(e => e.Sessions)
			  .FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<Event?> UpdateAsync(int id, Event model)
		{
			var existEvent = await GetByIdAsync(id);
			if (existEvent == null) return null;

			mapper.Map(model, existEvent);

			await dbContext.SaveChangesAsync();
			return existEvent;

		}
	}
}
