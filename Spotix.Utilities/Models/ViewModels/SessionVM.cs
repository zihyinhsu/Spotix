using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{
	public class SessionVM
	{
		public int Id { get; set; }

		[Display(Name = "場次名稱")]
		[Required(ErrorMessage = "{0} 為必填")]
		[StringLength(50)]
		public string Name { get; set; }


		[Display(Name = "開演時間")]
		[Required(ErrorMessage = "{0} 為必填")]
		public DateTime SessionTime { get; set; }

		[Display(Name = "開賣時間")]
		[Required(ErrorMessage = "{0} 為必填")]
		public DateTime AvailableTime { get; set; }

		[Display(Name = "上架時間")]
		[Required(ErrorMessage = "{0} 為必填")]
		public DateTime PublishTime { get; set; }

		[Display(Name = "上架")]
		[Required]
		public bool Published { get; set; }

		[Display(Name = "活動名稱")]
		//[Required(ErrorMessage = "{0} 為必填")]
		public string? EventName { get; set; }= null;

		[Display(Name = "活動名稱Id")]
		//[Required(ErrorMessage = "{0} 為必填")]
		public int? EventId { get; set; } = null;
	}
}

