
using System.ComponentModel.DataAnnotations;

namespace Spotix.Utilities.Models.ViewModels
{
	public class VerifyEmailVM
	{
		[Required(ErrorMessage = "{0} 為必填")]
		[EmailAddress]
		public string Email { get; set; }
	}
}
