using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.DTOs
{
	public class TicketDto
	{
		public int Id { get; set; }

		public int AreaId { get; set; }

		public int RowNumber { get; set; }

		public int SeatNumber { get; set; }

		public string TicketNumber { get; set; }

		public bool IsSold { get; set; }

		public bool IsTransfered { get; set; }

		public string RecieverId { get; set; }

		public int OrderId { get; set; }
	}
}
