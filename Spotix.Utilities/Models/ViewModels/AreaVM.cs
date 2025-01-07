using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{
	public class AreaVM
	{
		[Required]
		public string Name { get; set; }

		public int Price { get; set; }

		public int SessionId { get; set; }

		public int Qty { get; set; }

		public int DisplayOrder { get; set; }
	}
}
