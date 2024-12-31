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
		[Required(ErrorMessage = "Email為必填")]
		[EmailAddress]
		public string Email { get; set; }


		[Required(ErrorMessage = "密碼 為必填")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
