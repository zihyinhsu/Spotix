using AutoMapper;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.ViewModels;

namespace Spotix.API.Mappings
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<ProfileVM, User>().ReverseMap();
			CreateMap<Event, EventDto>()
				.ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
				.ForMember(dest => dest.Sessions, opt => opt.MapFrom(src => src.Sessions));

			CreateMap<Place, PlaceDto>().ReverseMap();
			CreateMap<Session, SessionDto>().ReverseMap();

		}
	}
}
