﻿using Autofac;
using MansionRentBackend.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace MansionRentBackend.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MansionAPIController : ControllerBase
    {
        private readonly ILifetimeScope _scope;
        private readonly APIResponse _response;

        public MansionAPIController(ILifetimeScope scope)
        {
            _scope = scope;
            _response = _scope.Resolve<APIResponse>();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> Get([FromQuery(Name = "filterOccupancy")] int? occupancy,
            [FromQuery] string? search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                var model = _scope.Resolve<VillaListModel>();
                //var villas = await model.GetAllVillas();
                var villas = await model.GetAllVillasByPage(pageSize, pageNumber);

                if (occupancy > 0)
                {
                    villas = villas.Where(o => o.Occupancy == occupancy).ToList();
                }

                if (!string.IsNullOrEmpty(search))
                {
                    villas = villas.Where(u => u.Name.ToLower().Contains(search)).ToList();
                }

                var pagination = _scope.Resolve<PaginationModel>();
                pagination.PageNumber = pageNumber;
                pagination.PageSize = pageSize;

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.ErrorMessages = new List<string>();
                _response.Result = villas;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.Message
                };
                _response.Result = null;

                return BadRequest(_response);
            }
        }

        [HttpGet("{id}")]
        public async Task<object> Get([FromRoute] Guid id)
        {
            try
            {
                var model = _scope.Resolve<VillaListModel>();
                var villa = await model.GetVilla(id);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.ErrorMessages = new List<string>();
                _response.Result = villa;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.Message
                };
                _response.Result = null;

                return BadRequest(_response);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Post([FromBody] MansionCreateDto mantionDto)
        {
            try
            {
                mantionDto.ResolveDependency(_scope);

                await mantionDto.CreateMantion();

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                _response.ErrorMessages = new List<string>();
                _response.Result = null;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.Message
                };
                _response.Result = null;

                return BadRequest(_response);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var model = _scope.Resolve<VillaListModel>();
                await model.DeleteVilla(id);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                _response.ErrorMessages = new List<string>();
                _response.Result = null;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.Message
                };
                _response.Result = null;

                return BadRequest(_response);
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Put([FromBody] MansionEditDto mansionDto)
        {
            try
            {
                mansionDto.ResolveDependency(_scope);
                await mansionDto.EditMansion();

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                _response.ErrorMessages = new List<string>();
                _response.Result = null;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.Message
                };
                _response.Result = null;

                return BadRequest(_response);
            }
        }
    }
}