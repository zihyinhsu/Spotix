using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.Interfaces
{
	public interface IAreaRepository
	{
		Task<List<Area>> GetAllAsync();
		Task<Area?> GetByIdAsync(int sessionId);
		Task<List<Area>> GetBySessionIdAsync(int sessionId);
		Task<Area> CreateAsync(Area model);
		Task<Area?> UpdateAsync(int id, Area model);
		Task<Area?> DeleteAsync(int id);
	}
}
