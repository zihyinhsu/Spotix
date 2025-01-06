using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.Repositories
{
	internal class EventRepository
	{
		private readonly AppDbContext dbContext;

		public EventRepository(AppDbContext dbContext)
		{
			this.dbContext = dbContext;
		}
	}
}
