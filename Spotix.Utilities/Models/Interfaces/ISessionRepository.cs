using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.Interfaces
{
	public interface ISessionRepository
	{
		Task<List<Session>> GetAllAsync();
		Task<SessionDto?> GetByIdAsync(int id);
		Task<Session> CreateAsync(Session model);
		Task<Session?> UpdateAsync(int id, Session model);
		Task<Session?> DeleteAsync(int id);
	}
}
