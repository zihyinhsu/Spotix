using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{
	public class LoginVM
	{
		[Display(Name = "Email")]
		[Required(ErrorMessage = "{0} 為必填")]
		[EmailAddress]
		public string Email { get; set; }

		[Display(Name = "密碼")]
		[Required(ErrorMessage = "{0} 為必填")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
