using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.DTOs
{
	public class EventDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Info { get; set; }

		public string CoverUrl { get; set; }

		public string ImgUrl { get; set; }

		public string Host { get; set; }

		public bool Published { get; set; }

		public PlaceDto Place { get; set; }

		public List<SessionDto> Sessions { get; set; }
	}
}
