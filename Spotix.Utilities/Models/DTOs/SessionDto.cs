using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.DTOs
{
	public class SessionDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public DateTime SessionTime { get; set; }

		public DateTime AvailableTime { get; set; }

		public DateTime PublishTime { get; set; }

		public bool Published { get; set; }

		public int EventId { get; set; }

		public List<AreaDto> Areas { get; set; }

	}
}
