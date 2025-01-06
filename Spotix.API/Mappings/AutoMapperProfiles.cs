using AutoMapper;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.ViewModels;

namespace Spotix.API.Mappings
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<ProfileVM, User>().ReverseMap();

		}
	}
}
