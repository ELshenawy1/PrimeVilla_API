using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using PrimeVilla_VillaAPI.Models;
using PrimeVilla_VillaAPI.Models.DTO;
using PrimeVilla_VillaAPI.Repository.IRepository;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace PrimeVilla_VillaAPI.Controllers.v2
{
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiVersion("2.0")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly IVillaRepository _dbVilla;
        private readonly ILogger<VillaAPIController> _logger;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public VillaAPIController(IVillaRepository dbVilla, ILogger<VillaAPIController> logger, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _logger = logger;
            _mapper = mapper;
            _response = new();
        }


        [HttpGet]
        //[ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "FilterOccupancy")] int occupancy,
            [FromQuery] string search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                _logger.LogInformation("Getting All Villas");
                IEnumerable<Villa> villaList;

                if (occupancy > 0)
                {
                    villaList = await _dbVilla.GetAllAsync(v => v.Occupancy == occupancy, pageSize: pageSize, pageNumber: pageNumber);
                }
                else
                {
                    villaList = await _dbVilla.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    villaList = villaList.Where(u => u.Name.ToLower().Contains(search));
                }
                Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {


                if (id <= 0)
                {
                    _logger.LogError($"Get villa error with ID: {id}");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromForm] VillaCreateDTO createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return BadRequest(createDto);
                }

                if (await _dbVilla.GetAsync(v => v.Name.ToLower() == createDto.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Name Must Be Unique!!!");
                    return BadRequest(ModelState);
                }

                Villa villa = _mapper.Map<Villa>(createDto);

                await _dbVilla.CreateAsync(villa);

                if (createDto.Image != null)
                {
                    string fileName = villa.Id + Path.GetExtension(createDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImage\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    FileInfo file = new FileInfo(directoryLocation);

                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        createDto.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    villa.ImageUrl = baseUrl + "/ProductImage/" + fileName;
                    villa.ImageLocalPath = filePath;
                }
                else
                {
                    villa.ImageUrl = "https://plcasehold.co/600x400";
                }

                await _dbVilla.UpdateAsync(villa);

                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {


                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(villa.ImageLocalPath))
                {
                    var oldFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), villa.ImageLocalPath);

                    FileInfo file = new FileInfo(oldFileDirectory);

                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                await _dbVilla.RemoveAsync(villa);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromForm] VillaUpdateDTO updateDto)
        {
            try
            {
                if (updateDto == null || id != updateDto.Id)
                {
                    return BadRequest();
                }

                Villa model = _mapper.Map<Villa>(updateDto);

                if (updateDto.Image != null)
                {
                    if (!string.IsNullOrEmpty(model.ImageLocalPath))
                    {
                        var oldFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), model.ImageLocalPath);

                        FileInfo file = new FileInfo(oldFileDirectory);

                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    string fileName = updateDto.Id + Path.GetExtension(updateDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImage\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);


                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        updateDto.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    model.ImageUrl = baseUrl + "/ProductImage/" + fileName;
                    model.ImageLocalPath = filePath;
                }
                else
                {
                    model.ImageUrl = "https://plcasehold.co/600x400";
                }

                await _dbVilla.UpdateAsync(model);

                _response.StatusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDto)
        {

            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);
            if (villa == null)
            {
                return BadRequest();
            }


            VillaUpdateDTO villaDto = _mapper.Map<VillaUpdateDTO>(villa);

            patchDto.ApplyTo(villaDto, ModelState);

            Villa model = _mapper.Map<Villa>(villaDto);

            await _dbVilla.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }

    }

}
