using Azure.Messaging;
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
    public class RawMaterialController : ControllerBase
    {
        private readonly IManufacturingRepository<RawMaterial> _repo;
        private readonly ILogger<RawMaterialController> _logger;
        public RawMaterialController(IManufacturingRepository<RawMaterial> repo, ILogger<RawMaterialController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // GET: api/<RawMaterialController>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                return Ok(await _repo.GetAllAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all Raw Materials");
                return StatusCode(500, "An error occurred while getting all Raw Materials"); // Internal Server Error
            }
        }

        // GET api/<RawMaterialController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var rawMat = await _repo.GetAsync(id);
                if (rawMat == null)
                {
                    return NotFound();
                }
                return Ok(rawMat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the Raw Material");
                return StatusCode(500, "An error occurred while getting the Raw Material");
            }
        }

        // POST api/<RawMaterialController>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RawMaterial rawMaterial)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _repo.CreateAsync(rawMaterial);

                if (rawMaterial.Id == 0)
                {
                    _logger.LogError("Failed to assign a valid ID to the Raw Material");
                    return StatusCode(500, "Failed to assign a valid ID to the Raw Material"); // Internal Server Error
                }
                var actionName = nameof(GetByIdAsync);

                var routeValues = new { id = rawMaterial.Id };

                // Create response
                var uri = Url.Action(nameof(GetByIdAsync), new { id = rawMaterial.Id });
                return Created(uri, rawMaterial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new Raw Material");
                return StatusCode(500, "An error occurred while creating a new Raw Material"); // Internal Server Error
            }
        }

        // PUT api/<RawMaterialController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromBody] RawMaterial rawMaterial)
        {
            try
            {
                await _repo.UpdateAsync(rawMaterial);
                return Ok(new { MessageContent = "Raw Material updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the Raw Material");
                return StatusCode(500, "An error occurred while updating the Raw Material");
            }
        }

        // DELETE api/<RawMaterialController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await _repo.DeleteAsync(id);
                return Ok(new { MessageContent = "Raw Material deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Raw Material");
                return StatusCode(500, "An error occurred while deleting Raw Material"); // Internal Server Error
            }
        }
    }
}
