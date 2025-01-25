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
		public int Id { get; set; }

		[Display(Name = "區域名稱")]
		[Required(ErrorMessage = "{0} 為必填")]
		public string Name { get; set; }

		[Display(Name = "價位")]
		[Required(ErrorMessage = "{0} 為必填")]
		public int Price { get; set; }

		[Display(Name = "場次名稱Id")]
		//[Required(ErrorMessage = "{0} 為必填")]
		public int? SessionId { get; set; } = null;

		[Display(Name = "場次名稱")]
		//[Required(ErrorMessage = "{0} 為必填")]
		public string? SessionName { get; set; } = null;


		[Display(Name = "數量")]
		[Required(ErrorMessage = "{0} 為必填")]
		public int Qty { get; set; }

		[Display(Name = "顯示順序")]
		[Required(ErrorMessage = "{0} 為必填")]
		public int? DisplayOrder { get; set; }
	}
}
