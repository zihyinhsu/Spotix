using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models
{
	public class OrderCache
	{
		public string Id { get; set; }
		public byte[] Value { get; set; }
		public DateTimeOffset ExpiresAtTime { get; set; }
		public long? SlidingExpirationInSeconds { get; set; }
		public DateTimeOffset? AbsoluteExpiration { get; set; }
	}
}
