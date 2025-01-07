using AutoMapper;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using Spotix.Utilities.Models.Repositories;
using Spotix.Utilities.Models.ViewModels;
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
		private readonly ITicketRepository ticketRepository;

		public AreaService(IAreaRepository areaRepository, ITicketRepository ticketRepository)
		{
			this.areaRepository = areaRepository;
			this.ticketRepository = ticketRepository;
		}


		public async Task<List<Area>> GetBySessionId(int sessionId)
		{
			var areasModel = await areaRepository.GetBySessionIdAsync(sessionId);

			return areasModel;
		}

		public async Task<Area?> GetByIdAsync(int id) {
			var areasModel = await areaRepository.GetByIdAsync(id);
			return areasModel;
		}

		public async Task<Area> CreateAsync(Area areaModel)
		{
			// 建立 area
			areaModel = await areaRepository.CreateAsync(areaModel);

			// 依照 area 的 qty 建立 tickets
			var qty = areaModel.Qty;
			var tickets = new List<Ticket>();
			var columns = 12;
			var rowsNum = (int)Math.Round((double)qty / 12, MidpointRounding.AwayFromZero);

			for (int i = 0; i < rowsNum; i++)
			{
				for (int c = 0; c < columns; c++)
				{
					tickets.Add(new Ticket
					{
						AreaId = areaModel.Id,
						RowNumber = i + 1,
						SeatNumber = c + 1,
						TicketNumber = Guid.NewGuid().ToString(),
						IsSold = false,
						IsTransfered = false,
					});
				}
			}

			await ticketRepository.CreateAsync(tickets);
			return areaModel;
		}
	}
}
