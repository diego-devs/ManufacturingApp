using ManufacturingApp.API.Data;
using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ManufacturingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly IManufacturingRepository<Supplier> _repo;
        private readonly ILogger<SupplierController> _logger;
        public SupplierController(IManufacturingRepository<Supplier> repo, ILogger<SupplierController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // GET: api/<SupplierController>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                return Ok(await _repo.GetAllAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all Suppliers");
                return StatusCode(500, "An error occurred while getting all Suppliers"); // Internal Server Error
            }
        }

        // GET api/<SupplierController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var supplier = await _repo.GetAsync(id);
                if (supplier == null)
                {
                    return NotFound();
                }
                return Ok(supplier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the Supplier");
                return StatusCode(500, "An error occurred while getting the Supplier");
            }
        }

        // POST api/<SupplierController>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _repo.CreateAsync(supplier);

                if (supplier.Id == 0)
                {
                    _logger.LogError("Failed to assign a valid ID to the Supplier");
                    return StatusCode(500, "Failed to assign a valid ID to the Supplier"); // Internal Server Error
                }

                var actionName = nameof(GetByIdAsync);
                var routeValues = new { id = supplier.Id };

                // Create the response
                var uri = Url.Action(nameof(GetByIdAsync), new { id = supplier.Id });
                return Created(uri, supplier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new Supplier");
                return StatusCode(500, "An error occurred while creating a new Supplier");
            }
        }

        // PUT api/<SupplierController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromBody] Supplier supplier)
        {
            try
            {
                await _repo.UpdateAsync(supplier);
                return Ok(new { MessageContent = "Supplier updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the Supplier");
                return StatusCode(500, "An error occurred while updating the Supplier");
            }
        }

        // DELETE api/<SupplierController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await _repo.DeleteAsync(id);
                return Ok(new { MessageContent = "Supplier deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Supplier");
                return StatusCode(500, "An error occurred while deleting Supplier"); // Internal Server Error
            }
        }
    }
}
