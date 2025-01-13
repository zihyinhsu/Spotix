using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{
	public class ProfileVM
	{

		[Display(Name = "姓名")]
		[Required(ErrorMessage = "{0} 為必填")]
		public string UserName { get; set; }

		[Display(Name = "地址")]
		[Required(ErrorMessage = "{0} 為必填")]
		public string Address { get; set; }

		[Display(Name = "性別")]
		[Required(ErrorMessage = "{0} 為必填")]
		public bool Gender { get; set; }

		[Display(Name = "生日")]
		[Required(ErrorMessage = "{0} 為必填")]
		public DateTime Birthday { get; set; }


		[Display(Name = "手機")]
		[Required(ErrorMessage = "{0} 為必填")]
		public string PhoneNumber { get; set; }
	}
}
