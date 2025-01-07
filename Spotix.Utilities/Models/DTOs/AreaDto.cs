using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.DTOs
{
	public class AreaDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public int Price { get; set; }

		public int SessionId { get; set; }

		public int Qty { get; set; }

		public int DisplayOrder { get; set; }

		public List<TicketDto> Tickets { get; set; }
	}
}
