using Azure.Messaging;
using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ManufacturingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IManufacturingRepository<Product> _repo;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IManufacturingRepository<Product> repo, ILogger<ProductController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // GET: api/<ProductController>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                return Ok(await _repo.GetAllAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all products");
                return StatusCode(500, "An error occurred while getting all products"); // Internal Server Error
            }
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var product = await _repo.GetAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the Product");
                return StatusCode(500, "An error occurred while getting the Product");
            }
        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _repo.CreateAsync(product);

                if (product.Id == 0)
                {
                    _logger.LogError("Failed to assign a valid ID to the product.");
                    return StatusCode(500, "Failed to assign a valid ID to the product."); // Internal Server Error
                }

                var actionName = nameof(GetByIdAsync);

                var routeValues = new { id = product.Id };

                // Create the response
                var uri = Url.Action(nameof(GetByIdAsync), new { id = product.Id });
                return Created(uri, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new product");
                return StatusCode(500, "An error occurred while creating a new product");
            }
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromBody] Product product)
        {
            try
            {
                await _repo.UpdateAsync(product);
                return Ok(new { MessageContent = "Product updated successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product");
                return StatusCode(500, "An error occurred while updating the product");
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var t = _repo.GetAsync(id);
                if (t != null)
                {
                    await _repo.DeleteAsync(id);
                    return Ok(new { MessageContent = "Product deleted successfully" });
                }
                return NotFound();
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting product");
                return StatusCode(500, "An error occurred while deleting product"); // Internal Server Error
            }
        }
    }
}
