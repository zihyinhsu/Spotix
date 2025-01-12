using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spotix.API.CustomActionFilter;
using Spotix.API.Exceptions;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.Interfaces;
using Spotix.Utilities.Models.Services;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SessionsController : ControllerBase
	{
		private readonly ISessionRepository sessionRepository;
		private readonly IMapper mapper;

		public SessionsController(ISessionRepository sessionRepository, IMapper mapper)
		{
			this.sessionRepository = sessionRepository;
			this.mapper = mapper;
		}

		[HttpGet("ByEvent/{eventId:int}")]
		[ValidateModel]
		public async Task<IActionResult> GetByEventId(int eventId)
		{
			var sessionModel = await sessionRepository.GetByEventIdAsync(eventId);
			HttpContext.Items["message"] = "搜尋成功";
			var sessionDto = mapper.Map<List<SessionDto>>(sessionModel);
			return Ok(sessionDto);
		}

		[HttpGet]
		[Route("{id:int}")]
		public async Task<IActionResult> GetById(int id)
		{
			var sessionModel = await sessionRepository.GetByIdAsync(id);
			if (sessionModel == null) throw new ResourceNotFoundException("搜尋失敗");
			HttpContext.Items["message"] = "搜尋成功";

			var sessionDto = mapper.Map<SessionDto>(sessionModel);

			return Ok(sessionDto);
		}
	}
}
