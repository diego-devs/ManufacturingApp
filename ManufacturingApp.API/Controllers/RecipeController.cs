using Azure.Messaging;
using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ManufacturingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IManufacturingRepository<Recipe> _repo;
        private readonly ILogger<RecipeController> _logger;

        public RecipeController(IManufacturingRepository<Recipe> repo, ILogger<RecipeController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // GET: api/<RecipeController>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                return Ok(await _repo.GetAllAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all Recipes");
                return StatusCode(500, "An error occurred while getting all Recipes");
            }
        }

        // GET api/<RecipeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var recipe = await _repo.GetAsync(id);
                if (recipe == null)
                {
                    return NotFound();
                }
                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the Recipe");
                return StatusCode(500, "An error occurred while getting the Recipe");
            }
        }

        // POST api/<RecipeController>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Recipe recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _repo.CreateAsync(recipe);

                if (recipe.Id == 0)
                {
                    _logger.LogError("Failed to assign a valid ID to the Recipe");
                    return StatusCode(500, "Failed to assign a valid ID to the Recipe");
                }
                var actionName = nameof(GetByIdAsync);
                var routeValues = new { id = recipe.Id };

                // Create the response
                var uri = Url.Action(nameof(GetByIdAsync), new { id = recipe.Id });
                return Created(uri, recipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new Recipe");
                return StatusCode(500, "An error occurred while creating a new Recipe");
            }
        }

        // PUT api/<RecipeController>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromBody] Recipe recipe)
        {
            try
            {
                await _repo.UpdateAsync(recipe);
                return Ok(new { MessageContent = "Recipe updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the Recipe");
                return StatusCode(500, "An error occurred while updating the Recipe");
            }
        }

        // DELETE api/<RecipeController>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await _repo.DeleteAsync(id);
                return Ok(new { MessageContent = "Recipe deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Recipe");
                return StatusCode(500, "An error occurred while deleting the Recipe");
            }
        }
    }
}
