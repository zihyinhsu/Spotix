using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{
	public class OrderVM
	{
		public DateTime CreatedTime { get; set; }

		public int Total { get; set; }

		public string ItemDesc { get; set; }

		public string TicketIds { get; set; }

	}
}
