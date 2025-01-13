using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.DTOs
{
	public class LoginResponseDto
	{
		public string UserName { get; set; }
		public string Email { get; set; }
		public string JwtToken { get; set; }
		public string AvatarUrl { get; set; }
	}
}
