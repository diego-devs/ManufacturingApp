using Azure.Messaging;
using ManufacturingApp.API.DTOs;
using ManufacturingApp.API.Extensions;
using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;

namespace ManufacturingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IManufacturingRepository<Recipe> _recipeRepo;
        private readonly IManufacturingRepository<RecipeRawMaterial> _rawMaterialRepo; 
        private readonly IManufacturingRepository<RecipeProduct> _productRepo;
        private readonly IManufacturingRepository<Supplier> _supplierRepo;
        private readonly IManufacturingRepository<SupplierRawMaterial> _supplierRawMaterialRepo;
        private readonly ILogger<RecipesController> _logger;

        public RecipesController(
            IManufacturingRepository<Recipe> recipeRepo, 
            ILogger<RecipesController> logger,
            IManufacturingRepository<RecipeRawMaterial> rawMaterialRepo,
            IManufacturingRepository<RecipeProduct> productRepo,
            IManufacturingRepository<Supplier> supplierRepo,
            IManufacturingRepository<SupplierRawMaterial> supplierRawMaterialRepo)
        {
            _recipeRepo = recipeRepo;
            _logger = logger;
            _rawMaterialRepo = rawMaterialRepo;
            _productRepo = productRepo;
            _supplierRepo = supplierRepo;
            _supplierRawMaterialRepo = supplierRawMaterialRepo;
        }

        // GET: api/<RecipesController>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                // Check recipes exist
                var recipes = await _recipeRepo.GetAllAsync(query => query
                .Include(r => r.RecipeRawMaterials)
                .ThenInclude(rrm => rrm.RawMaterial)
                .Include(r => r.RecipeProducts)
                .ThenInclude(rp => rp.Product));

                if (recipes.IsNullOrEmpty())
                {
                    return NoContent();
                }
                // Fetch suppliers and pricing for raw materials
                var suppliers = await _supplierRepo.GetAllAsync(query => query.Include(s => s.SupplierRawMaterials));
                var supplierDict = suppliers.ToDictionary(s => s.Id, s => s);

                // Create DTOs 
                var recipeDtos = recipes.Select(r => new RecipeDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    RecipeRawMaterials = r.RecipeRawMaterials.Select(rm => new RecipeRawMaterialDTO
                    {
                        RawMaterialId = rm.RawMaterialId,
                        RawMaterialName = rm.RawMaterial.Name,
                        Quantity = rm.Quantity
                    }).ToList(),
                    RecipeProducts = r.RecipeProducts.Select(rp => new RecipeProductDTO
                    {
                        ProductId = rp.ProductId,
                        ProductName = rp.Product.Name,
                        Quantity = rp.Quantity
                    }).ToList(),
                    RecipeSuppliers = suppliers.Select(s => new RecipeSupplierDTO
                    {
                        Name = s.Name,
                        Pricing = s.SupplierRawMaterials
                            .Where(srm => r.RecipeRawMaterials.Any(rrm => rrm.RawMaterialId == srm.RawMaterialId))
                            .ToDictionary(srm => srm.RawMaterial.Name, srm => srm.Price)
                    }).Where(s => s.Pricing.Any()).ToList()
                }).ToList();
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(recipeDtos, options);

                return Ok(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingAll);
                return StatusCode(500, ApiMessages.ErrorGettingAll);
            }
        }

        // GET api/<RecipesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                // Check recipe exists
                var recipe = await _recipeRepo.GetAsync(id, query => query
                .Include(r => r.RecipeRawMaterials)
                .ThenInclude(rrm => rrm.RawMaterial)
                .Include(r => r.RecipeProducts)
                .ThenInclude(rp => rp.Product));
                if (recipe == null)
                {
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }

                // Fetch suppliers and pricing for raw materials
                var suppliers = await _supplierRepo.GetAllAsync(query => query.Include(s => s.SupplierRawMaterials));
                var supplierDict = suppliers.ToDictionary(s => s.Id, s => s);

                // Create DTO 
                var recipeDto = new RecipeDTO
                {
                    Id = recipe.Id,
                    Name = recipe.Name,
                    Description = recipe.Description,
                    RecipeRawMaterials = recipe.RecipeRawMaterials.Select(rm => new RecipeRawMaterialDTO
                    {
                        RawMaterialId = rm.RawMaterialId,
                        RawMaterialName = rm.RawMaterial.Name,
                        Quantity = rm.Quantity
                    }).ToList(),
                    RecipeProducts = recipe.RecipeProducts.Select(rp => new RecipeProductDTO
                    {
                        ProductId = rp.ProductId,
                        ProductName = rp.Product.Name,
                        Quantity = rp.Quantity
                    }).ToList(),
                    RecipeSuppliers = suppliers.Select(s => new RecipeSupplierDTO
                    {
                        Name = s.Name,
                        Pricing = s.SupplierRawMaterials
                            .Where(srm => recipe.RecipeRawMaterials.Any(rrm => rrm.RawMaterialId == srm.RawMaterialId))
                            .ToDictionary(srm => srm.RawMaterial.Name, srm => srm.Price)
                    }).Where(s => s.Pricing.Any()).ToList()
                };

                return Ok(recipeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingById);
                return StatusCode(500, ApiMessages.ErrorGettingById);
            }
        }

        // POST api/<RecipesController>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RecipeDTO recipeDto)
        {
            // Check request model is correct
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"{ApiMessages.InvalidModelWarning} : {ModelState}");
                return BadRequest(ModelState);
            }
            // Validate existence of raw materials
            foreach (var rawMaterialDto in recipeDto.RecipeRawMaterials)
            {
                var badRequest = BadRequest($"Raw material with ID {rawMaterialDto.RawMaterialId} does not exist.");
                if (rawMaterialDto.RawMaterialId == 0)
                {
                    _logger.LogWarning($"{ApiMessages.ItemNotfound}");
                    return badRequest;
                }
                var rawMaterial = await _rawMaterialRepo.GetAsync(rawMaterialDto.RawMaterialId);
                if (rawMaterial == null)
                {
                    _logger.LogWarning($"{ApiMessages.ItemNotfound}");
                    return badRequest;
                }
            }
            // Validate existence of products
            foreach (var productDto in recipeDto.RecipeProducts)
            {
                var badRequest = BadRequest($"Product with ID {productDto.ProductId} does not exist.");
                if (productDto.ProductId == 0)
                {
                    _logger.LogWarning($"{ApiMessages.ItemNotfound}");
                    return badRequest;
                }
                var product = await _productRepo.GetAsync(productDto.ProductId);
                if (product == null)
                {
                    _logger.LogWarning($"{ApiMessages.ItemNotfound}");
                    return badRequest;
                }
            }
            // Create recipe from request DTO
            var recipe = new Recipe
            {
                Name = recipeDto.Name,
                Description = recipeDto.Description,
                RecipeRawMaterials = recipeDto.RecipeRawMaterials.Select(rm => new RecipeRawMaterial
                {
                    RawMaterialId = rm.RawMaterialId,
                    Quantity = rm.Quantity

                }).ToList(),
                RecipeProducts = recipeDto.RecipeProducts.Select(rp => new RecipeProduct
                {
                    ProductId = rp.ProductId,
                    Quantity = rp.Quantity
                }).ToList()
            };

            try
            {
                // Create recipe
                await _recipeRepo.CreateAsync(recipe);
                // Handle error assigning Id
                if (recipe.Id == 0)
                {
                    _logger.LogError(ApiMessages.ErrorAssigningId);
                    return StatusCode(500, ApiMessages.ErrorAssigningId);
                }
                // Return created action
                var actionName = nameof(GetByIdAsync);
                var routeValues = new { id = recipe.Id };
                var uri = Url.Action(actionName, routeValues);

                return Created(uri, recipeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorCreating);
                return StatusCode(500, ApiMessages.ErrorCreating);
            }
        }

        // PUT api/<RecipesController>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] RecipeDTO recipeDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"{ApiMessages.InvalidModelWarning} : {ModelState}");
                return BadRequest(ModelState);
            }

            try
            {
                // Check recipe exists
                var existingRecipe = await _recipeRepo.GetAsync(id);
                if (existingRecipe == null)
                {
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }
                // Validate existence of raw materials
                foreach (var rawMaterialDto in recipeDto.RecipeRawMaterials)
                {
                    var badRequest = BadRequest($"Raw material with ID {rawMaterialDto.RawMaterialId} does not exist.");
                    if (rawMaterialDto.RawMaterialId == 0)
                    {
                        return badRequest;
                    }
                    var rawMaterial = await _rawMaterialRepo.GetAsync(rawMaterialDto.RawMaterialId);
                    if (rawMaterial == null)
                    {
                        return badRequest;
                    }
                }

                // Validate existence of products
                foreach (var productDto in recipeDto.RecipeProducts)
                {
                    var badRequest = BadRequest($"Product with ID {productDto.ProductId} does not exist.");
                    if (productDto.ProductId == 0)
                    {
                        return badRequest;
                    }
                    var product = await _productRepo.GetAsync(productDto.ProductId);
                    if (product == null)
                    {
                        return badRequest;
                    }
                }
                // Edit with new values
                existingRecipe.Name = recipeDto.Name;
                existingRecipe.Description = recipeDto.Description;
                existingRecipe.RecipeRawMaterials = recipeDto.RecipeRawMaterials.Select(rm => new RecipeRawMaterial
                {
                    RecipeId = existingRecipe.Id,
                    RawMaterialId = rm.RawMaterialId,
                    Quantity = rm.Quantity
                }).ToList();
                existingRecipe.RecipeProducts = recipeDto.RecipeProducts.Select(rp => new RecipeProduct
                {
                    RecipeId = existingRecipe.Id,
                    ProductId = rp.ProductId,
                    Quantity = rp.Quantity
                }).ToList();
                // Update recipe
                await _recipeRepo.UpdateAsync(existingRecipe);
                return Ok(new { MessageContent = ApiMessages.SuccessUpdated });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorUpdating);
                return StatusCode(500, ApiMessages.ErrorUpdating);
            }
        }

        // DELETE api/Recipes/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                // Check recipe exists
                var existingRecipe = await _recipeRepo.GetAsync(id);
                if (existingRecipe == null)
                {
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }
                // Delete recipe
                await _recipeRepo.DeleteAsync(id);
                return Ok(new { MessageContent = ApiMessages.SuccessDeleted });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorDeleting);
                return StatusCode(500, ApiMessages.ErrorDeleting);
            }
        }

        // POST api/Recipes/optimizeSuppliers
        [HttpPost("optimizeSuppliers")]
        public async Task<IActionResult> OptimizeSuppliers([FromBody] int recipeId)
        {
            try
            {
                var recipe = await _recipeRepo.GetAsync(recipeId);
                if (recipe == null)
                {
                    _logger.LogWarning(ApiMessages.ItemNotfound);
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }

                // Optimization logic

                // Get all raw materials required by the recipe
                var rawMaterials = recipe.RecipeRawMaterials;

                // Get all suppliers and their prices for the raw materials
                var supplierPrices = new Dictionary<int, Dictionary<int, decimal>>();
                var suppliers = await _supplierRepo.GetAllAsync();
                foreach (var supplier in suppliers)
                {
                    var prices = await _supplierRawMaterialRepo.GetAllAsync();
                    foreach (var price in prices.Where(p => p.SupplierId == supplier.Id))
                    {
                        if (!supplierPrices.ContainsKey(price.RawMaterialId))
                        {
                            supplierPrices[price.RawMaterialId] = new Dictionary<int, decimal>();
                        }
                        supplierPrices[price.RawMaterialId][price.SupplierId] = price.Price;
                    }
                }
                // Calculate the optimal combination of suppliers
                var optimizedSuppliers = new List<OptimizedSupplierDTO>();
                decimal totalCost = 0;

                foreach (var rawMaterial in rawMaterials)
                {
                    var materialId = rawMaterial.RawMaterialId;
                    if (supplierPrices.ContainsKey(materialId))
                    {
                        var bestPrice = supplierPrices[materialId].OrderBy(p => p.Value).First();
                        var supplier = suppliers.First(s => s.Id == bestPrice.Key);
                        optimizedSuppliers.Add(new OptimizedSupplierDTO
                        {
                            SupplierId = supplier.Id,
                            SupplierName = supplier.Name,
                            RawMaterialPrices = new Dictionary<int, decimal> { { materialId, bestPrice.Value } },
                            TotalCost = bestPrice.Value * rawMaterial.Quantity
                        });
                        totalCost += bestPrice.Value * rawMaterial.Quantity;
                    }
                    else
                    {
                        _logger.LogWarning($"{ApiMessages.NoSupplierFound} : {materialId}");
                        return BadRequest($"{ApiMessages.NoSupplierFound} : {materialId}");
                    }
                }

                var response = new
                {
                    TotalCost = totalCost,
                    Suppliers = optimizedSuppliers
                };


                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorOptimizeSuppliers);
                return StatusCode(500, ApiMessages.ErrorOptimizeSuppliers);
            }
        }
    }
}
