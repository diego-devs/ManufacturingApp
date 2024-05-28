using Azure.Messaging;
using ManufacturingApp.API.DTOs;
using ManufacturingApp.API.Extensions;
using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Data;
using ManufacturingApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ManufacturingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RawMaterialsController : ControllerBase
    {
        private readonly IManufacturingRepository<RawMaterial> _rawMaterialsRepo;
        private readonly ILogger<RawMaterialsController> _logger;
        public RawMaterialsController(IManufacturingRepository<RawMaterial> repo, 
                                        ILogger<RawMaterialsController> logger)
        {
            _rawMaterialsRepo = repo;
            _logger = logger;
        }

        // GET: api/<RawMaterialController>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var rawMaterials = await _rawMaterialsRepo.GetAllAsync();
                if (rawMaterials.IsNullOrEmpty())
                {
                    return NoContent();
                }
                var rawMaterialsDtos = rawMaterials.Select(rm => new RawMaterialDTO
                {
                    Id = rm.Id,
                    Name = rm.Name,
                    Description = rm.Description
                }).ToList();

                return Ok(rawMaterialsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingAll);
                return StatusCode(500, ApiMessages.ErrorGettingAll);
            }
        }

        // GET api/<RawMaterialController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var rawMat = await _rawMaterialsRepo.GetAsync(id);
                if (rawMat == null)
                {
                    return NotFound(new {MessageContent = ApiMessages.ItemNotfound});
                }
                var rawMaterialDto = new RawMaterialDTO
                {
                    Id = rawMat.Id,
                    Name = rawMat.Name,
                    Description = rawMat.Description
                };
                return Ok(rawMaterialDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingById);
                return StatusCode(500, ApiMessages.ErrorGettingById);
            }
        }

        // POST api/<RawMaterialController>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RawMaterialDTO rawMaterialDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"{ApiMessages.InvalidModelWarning} : {ModelState}");
                return BadRequest(ModelState);
            }
            var rawMaterial = new RawMaterial
            {
                Id = rawMaterialDto.Id,
                Name = rawMaterialDto.Name,
                Description = rawMaterialDto.Description
            };
            try
            {
                await _rawMaterialsRepo.CreateAsync(rawMaterial);
                if (rawMaterial.Id == 0)
                {
                    _logger.LogError(ApiMessages.ErrorAssigningId);
                    return StatusCode(500, ApiMessages.ErrorAssigningId);
                }

                var actionName = nameof(GetByIdAsync);
                var routeValues = new { id = rawMaterial.Id };
                var uri = Url.Link(actionName, routeValues);

                var createdRawMaterialDto = new RawMaterialDTO
                {
                    Id = rawMaterial.Id,
                    Name = rawMaterial.Name,
                    Description = rawMaterial.Description
                };

                return Created(uri, createdRawMaterialDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorCreating);
                return StatusCode(500, ApiMessages.ErrorCreating); 
            }
        }

        // PUT api/<RawMaterialController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] RawMaterialDTO rawMaterialDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"{ApiMessages.InvalidModelWarning} : {ModelState}");
                return BadRequest(ModelState);
            }
            try
            {
                var rawMaterial = await _rawMaterialsRepo.GetAsync(id);
                if (rawMaterial == null)
                {
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }
                rawMaterial.Name = rawMaterialDto.Name;
                rawMaterial.Description = rawMaterialDto.Description;

                await _rawMaterialsRepo.UpdateAsync(rawMaterial);
                return Ok(new { MessageContent = ApiMessages.SuccessUpdated });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorUpdating);
                return StatusCode(500, ApiMessages.ErrorUpdating);
            }
        }

        // DELETE api/<RawMaterialController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var existingRawMaterial = await _rawMaterialsRepo.GetAsync(id);
                if (existingRawMaterial == null)
                {
                    _logger.LogWarning($"{ApiMessages.InvalidModelWarning} : {ModelState}");
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }

                await _rawMaterialsRepo.DeleteAsync(id);
                return Ok(new { MessageContent = ApiMessages.SuccessDeleted });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorDeleting);
                return StatusCode(500, ApiMessages.ErrorDeleting);
            }
        }
    }
}
