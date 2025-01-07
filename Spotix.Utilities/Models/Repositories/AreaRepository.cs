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
	public class AreaRepository : IAreaRepository
	{
		private readonly AppDbContext dbContext;
		private readonly IMapper mapper;

		public AreaRepository(AppDbContext dbContext)
		{
			this.dbContext = dbContext;

			// 手動配置 AutoMapper
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<Area, Area>()
					.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
			});

			mapper = config.CreateMapper();
		}
		public async Task<Area> CreateAsync(Area model)
		{
			await dbContext.Areas.AddAsync(model);
			await dbContext.SaveChangesAsync();
			return model;
		}

		public async Task<Area?> DeleteAsync(int id)
		{
			var existArea = await GetByIdAsync(id);
			if (existArea == null) return null;

			dbContext.Areas.Remove(existArea);
			await dbContext.SaveChangesAsync();

			return existArea;
		}

		public async Task<List<Area>> GetAllAsync()
		{
			return await dbContext.Areas
				.Include(x => x.Tickets)
				.ToListAsync();
		}

		public async Task<Area?> GetByIdAsync(int id)
		{
			return await dbContext.Areas
				.Include(x => x.Tickets)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<Area>> GetBySessionIdAsync(int sessionId)
		{
			return await dbContext.Areas
				.Include(x => x.Tickets)
				.Where(x => x.SessionId == sessionId)
				.ToListAsync();
		}

		public async Task<Area?> UpdateAsync(int id, Area model)
		{
			var existArea = await GetByIdAsync(id);
			if (existArea == null) return null;

			mapper.Map(model, existArea);

			await dbContext.SaveChangesAsync();
			return existArea;
		}
	}
}
