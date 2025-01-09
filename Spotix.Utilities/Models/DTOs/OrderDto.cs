using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.DTOs
{
	public class OrderDto
	{
		public int Id { get; set; }

		public DateTime CreatedTime { get; set; }

		public int Total { get; set; }

		public string Payment { get; set; }

		public string UserId { get; set; }

		public string OrderNumber { get; set; }

		public List<TicketDto> Tickets { get; set; }
	}
}
