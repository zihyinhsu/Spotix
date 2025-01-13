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
		[Required]
		[StringLength(50)]
		public string Name { get; set; }

		[Display(Name = "開演時間")]
		[Required]
		public DateTime SessionTime { get; set; }

		[Display(Name = "開賣時間")]
		[Required]
		public DateTime AvailableTime { get; set; }

		[Display(Name = "上架時間")]
		[Required]
		public DateTime PublishTime { get; set; }

		[Display(Name = "上架")]
		[Required]
		public bool Published { get; set; }

		[Display(Name = "活動名稱")]
		[Required]
		public string EventName { get; set; }
	}
}
