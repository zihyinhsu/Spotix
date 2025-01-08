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

		public async Task<Area?> GetByIdAsync(int id)
		{
			var areasModel = await areaRepository.GetByIdAsync(id);
			return areasModel;
		}

		public async Task<Area> CreateAsync(Area areaModel)
		{
			// 建立 area
			areaModel = await areaRepository.CreateAsync(areaModel);

			// 依照 area 的 qty 建立 tickets
			var qty = areaModel.Qty;
			var columns = 12;
			var tickets = new List<Ticket>();
			int rowNumber = 1;
			int seatNumber = 1;

			for (int i = 1; i <= qty; i++)
			{
				tickets.Add(new Ticket
				{
					AreaId = areaModel.Id,
					RowNumber = rowNumber,
					SeatNumber = seatNumber,
					TicketNumber = Guid.NewGuid().ToString(),
					IsSold = false,
					IsTransfered = false,
				});

				seatNumber++;

				if (seatNumber > columns)
				{
					seatNumber = 1;
					rowNumber++;
				}
			}

			await ticketRepository.CreateAsync(tickets);
			return areaModel;
		}
	}
}
