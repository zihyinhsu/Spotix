using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{

	//public class FilterEventVM
	//{
	//	public string? FilterQuery { get; set; }
	//	public int? Year { get; set; }
	//	public int? Month { get; set; }
	//	public string? SortBy { get; set; }
	//	public int PageNumber { get; set; } = 1;
	//	public int PageSize { get; set; } = 10;

	//	public List<EventVM> Data { get; set; }
	//}
	public class EventVM
	{

		public int Id { get; set; }

		[Display(Name = "活動名稱")]
		[Required(ErrorMessage = "{0} 為必填")]
		public required string Name { get; set; }

		[Display(Name = "活動資訊")]
		[Required(ErrorMessage = "{0} 為必填")]
		public required string Info { get; set; }

		[Display(Name = "封面圖片")]
		[Required(ErrorMessage = "{0} 為必填")]
		public required string CoverUrl { get; set; }

		[Display(Name = "座位表")]
		[Required(ErrorMessage = "{0} 為必填")]
		public required string ImgUrl { get; set; }

		[Display(Name = "場館地點")]
		//[Required(ErrorMessage = "{0} 為必填")]
		public string? PlaceName { get; set; } = null;

		[Display(Name = "場館地點")]
		[Required(ErrorMessage = "{0} 為必填")]
		public required int PlaceId { get; set; }

		[Display(Name = "主辦單位")]
		[Required(ErrorMessage = "{0} 為必填")]
		public required string Host { get; set; }

		[Display(Name = "上架")]
		[Required(ErrorMessage = "{0} 為必填")]
		public bool Published { get; set; }

		[Display(Name = "活動開演時間")]
		public DateTime? FirstSessionTime { get; set; } = null;

		[Display(Name = "活動場次")]
		public string? SessionName{ get; set; } = null;

	}
}
