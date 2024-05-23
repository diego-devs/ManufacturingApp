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
        public RawMaterialController(IManufacturingRepository<RawMaterial> manufacturingRepository, ILogger<RawMaterialController> logger)
        {
            _repo = manufacturingRepository;
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
                return StatusCode(500); // Internal Server Error
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
                return StatusCode(500);
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
                return CreatedAtAction(nameof(GetByIdAsync), new { id = rawMaterial.Id }, rawMaterial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new product");
                return StatusCode(500); // Internal Server Error
            }
        }

        // PUT api/<RawMaterialController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] RawMaterial rawMaterial)
        {
            if (id != rawMaterial.Id || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _repo.UpdateAsync(rawMaterial);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the Raw Material");
                return StatusCode(500);
            }
        }

        // DELETE api/<RawMaterialController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _repo.DeleteAsync(id);
                return NoContent(); // dev todo, return something
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Raw Material");
                return StatusCode(500); // Internal Server Error
            }
        }
    }
}
