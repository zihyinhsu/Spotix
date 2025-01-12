using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spotix.API.CustomActionFilter;
using Spotix.API.Exceptions;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.Interfaces;
using Spotix.Utilities.Models.Services;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventsController : ControllerBase
	{
		private readonly IEventRepository eventRepository;
		private readonly IMapper mapper;

		public EventsController(IEventRepository eventRepository, IMapper mapper)
		{
			this.eventRepository = eventRepository;
			this.mapper = mapper;
		}

		[HttpGet]
		[ValidateModel]
		public async Task<IActionResult> GetAllEvent(
			[FromQuery] string? filterQuery = null,
			[FromQuery] int? year = null,
			[FromQuery] int? month = null,
			[FromQuery] string? sortBy = null,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			var eventsModel = await eventRepository.GetAllAsync(filterQuery, year, month, sortBy, pageNumber, pageSize);

			HttpContext.Items["message"] = "搜尋成功";
			
			var eventDtos = mapper.Map<List<EventDto>>(eventsModel);

			return Ok(eventDtos);
		}

		[HttpGet]
		[Route("{id:int}")]
		[ValidateModel]
		public async Task<IActionResult> GetById(int id)
		{
			var eventModel = await eventRepository.GetByIdAsync(id);
			if (eventModel == null) throw new ResourceNotFoundException("搜尋失敗");

			HttpContext.Items["message"] = "搜尋成功";
			var eventDto = mapper.Map<EventDto>(eventModel);
			return Ok(eventDto);
		}
	}
}
