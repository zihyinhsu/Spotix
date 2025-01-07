using AutoMapper;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using Spotix.Utilities.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.Services
{
	public class AreaService
	{
		private readonly IAreaRepository areaRepository;

		public AreaService(IAreaRepository areaRepository)
		{
			this.areaRepository = areaRepository;
		}


		public async Task<List<Area>> GetBySessionId(int sessionId)
		{
			var areasModel = await areaRepository.GetBySessionIdAsync(sessionId);

			return areasModel;
		}

		public async Task<Area> CreateAsync(Area areaModel)
		{
			areaModel = await areaRepository.CreateAsync(areaModel);

			//tickets 新增邏輯 repository

			return areaModel;
		}
	}
}
