using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.DTOs.LineLogin
{
	public class LineUserProfileDto
	{
		public string UserId { get; set; }
		public string DisplayName { get; set; }
		public string StatusMessage { get; set; }
		public string PictureUrl { get; set; }
	}
}
