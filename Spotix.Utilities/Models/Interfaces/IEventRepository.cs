using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.Interfaces
{
	public interface IEventRepository
	{
		Task<List<Event>> GetAllAsync(string? filterQuery = null, int? year = null, int? month = null, string? sortBy = null, int pageNumber = 1, int pageSize = 10);
		Task<Event?> GetByIdAsync(int id);
		void CreateAsync(Event region);
		Task<Event?> UpdateAsync(int id, Event model);
		Task<Event?> DeleteAsync(int id);
	}
}
