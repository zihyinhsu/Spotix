using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models
{
	public class UserDto
	{
		public string UserName { get; set; }

		public string Email { get; set; }

		public string LineId { get; set; }

		public bool Gender { get; set; }

		public DateTime Birthday { get; set; }

		public string Address { get; set; }

		public string AvatarUrl { get; set; }

		public string PhoneNumber { get; set; }

		public List<string> Roles { get; set; }

		public List<OrderDto> Orders { get; set; }

	}
}
