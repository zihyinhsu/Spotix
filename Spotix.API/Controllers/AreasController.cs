using AutoMapper;
using DotNetLineBotSdk.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spotix.API.CustomActionFilter;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using Spotix.Utilities.Models.Repositories;
using Spotix.Utilities.Models.Services;
using Spotix.Utilities.Models.ViewModels;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AreasController : ControllerBase
	{
		private readonly AreaService areaService;
		private readonly IMapper mapper;
		public AreasController(AreaService areaRepository, IMapper mapper)
		{
			this.areaService = areaRepository;
			this.mapper = mapper;
		}
		[HttpGet]
		[Route("{sessionId:int}")]
		[ValidateModel]
		public async Task<IActionResult> GetBySessionId(int sessionId)
		{
			var areasModel = await areaService.GetBySessionId(sessionId);
			HttpContext.Items["message"] = "搜尋成功";
			var areaDto = mapper.Map<List<AreaDto>>(areasModel);
			return Ok(areaDto);
		}


		[HttpPost]
		[ValidateModel]
		public async Task<IActionResult> Create([FromBody] AreaVM model)
		{
			var areaModel = mapper.Map<Area>(model);

			areaModel = await areaService.CreateAsync(areaModel);

			var areaDto = mapper.Map<AreaDto>(areaModel);

			HttpContext.Items["message"] = "新增成功";

			return CreatedAtAction(nameof(Create), null);
		}

	}
}
