using AutoMapper;
using Spotix.Utilities.Models;
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
			CreateMap<UserDto, User>().ReverseMap()
				.ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
				.ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders));

			CreateMap<Area, AreaDto>()
				.ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets));

			CreateMap<Ticket, TicketDto>().ReverseMap();
			CreateMap<Place, PlaceDto>().ReverseMap();
			CreateMap<Session, SessionDto>().ReverseMap();
			CreateMap<Area, AreaVM>().ReverseMap();

			CreateMap<Order, OrderVM>().ReverseMap();
			CreateMap<OrderVM, OrderDto>().ReverseMap();
			CreateMap<Order, OrderDto>()
				.ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets));
		}
	}
}
