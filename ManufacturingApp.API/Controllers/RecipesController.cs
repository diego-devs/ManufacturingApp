﻿using Azure.Messaging;
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
    /// <summary>
    /// Controller for managing recipes and related entities.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IManufacturingRepository<Recipe> _recipeRepo;
        private readonly IManufacturingRepository<RecipeRawMaterial> _recipeRawMaterialRepo;
        private readonly IManufacturingRepository<RawMaterial> _rawMaterialRepo;
        private readonly IManufacturingRepository<RecipeProduct> _recipeProductRepo;
        private readonly IManufacturingRepository<Supplier> _supplierRepo;
        private readonly IManufacturingRepository<SupplierRawMaterial> _supplierRawMaterialRepo;
        private readonly IManufacturingRepository<Product> _productRepo;
        private readonly ILogger<RecipesController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipesController"/> class.
        /// </summary>
        /// <param name="recipeRepo">Repository for managing recipes.</param>
        /// <param name="logger">Logger instance for logging errors and information.</param>
        /// <param name="recipeRawMaterialRepo">Repository for managing recipe raw materials.</param>
        /// <param name="recipeProductRepo">Repository for managing recipe products.</param>
        /// <param name="supplierRepo">Repository for managing suppliers.</param>
        /// <param name="supplierRawMaterialRepo">Repository for managing supplier raw materials.</param>
        /// <param name="rawMaterialRepo">Repository for managing raw materials.</param>
        /// <param name="productRepo">Repository for managing products.</param>
        public RecipesController(
            IManufacturingRepository<Recipe> recipeRepo, 
            ILogger<RecipesController> logger,
            IManufacturingRepository<RecipeRawMaterial> recipeRawMaterialRepo,
            IManufacturingRepository<RecipeProduct> recipeProductRepo,
            IManufacturingRepository<Supplier> supplierRepo,
            IManufacturingRepository<SupplierRawMaterial> supplierRawMaterialRepo,
            IManufacturingRepository<RawMaterial> rawMaterialRepo,
            IManufacturingRepository<Product> productRepo)
        {
            _recipeRepo = recipeRepo;
            _logger = logger;
            _recipeRawMaterialRepo = recipeRawMaterialRepo;
            _recipeProductRepo = recipeProductRepo;
            _supplierRepo = supplierRepo;
            _supplierRawMaterialRepo = supplierRawMaterialRepo;
            _rawMaterialRepo = rawMaterialRepo;
            _productRepo = productRepo;
        }

        // GET: api/<RecipesController>
        /// <summary>
        /// Retrieves all recipes from the repository.
        /// </summary>
        /// <returns>An action result containing a list of all recipes.</returns>
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
                        Id = s.Id,
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
        /// <summary>
        /// Retrieves a specific recipe by its ID.
        /// </summary>
        /// <param name="id">The ID of the recipe to retrieve.</param>
        /// <returns>An action result containing the recipe with the specified ID.</returns>
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
                        Id = s.Id,
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
        /// <summary>
        /// Creates a new recipe.
        /// </summary>
        /// <param name="recipeDto">The recipe data transfer object containing the details of the recipe to create.</param>
        /// <returns>An action result containing the created recipe.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RecipeDTO recipeDto)
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"{ApiMessages.InvalidModelWarning} : {ModelState}");
                return BadRequest(ModelState);
            }
            // Validate existence of raw materials 
            List<RecipeRawMaterial> recipeRawMaterials = new List<RecipeRawMaterial>();

            foreach (var rawMaterialDto in recipeDto.RecipeRawMaterials)
            {
                var existingRawMaterial = await _rawMaterialRepo.GetAsync(rawMaterialDto.RawMaterialId);
                if (existingRawMaterial == null)
                {
                    var badRequest = $"Raw material with ID {rawMaterialDto.RawMaterialId} does not exist.";
                    _logger.LogWarning(badRequest);
                    return BadRequest(badRequest);
                }

                recipeRawMaterials.Add(new RecipeRawMaterial
                {
                      RawMaterialId = rawMaterialDto.RawMaterialId,
                      RawMaterial = existingRawMaterial,
                      Quantity = rawMaterialDto.Quantity
                });
            }
            // Validate existence of Products 
            List<RecipeProduct> recipeProducts = new List<RecipeProduct>();

            foreach (var productDto in recipeDto.RecipeProducts)
            {
                var existingProduct = await _productRepo.GetAsync(productDto.ProductId);
                if (existingProduct == null)
                {
                    var badRequest = $"Product with ID {productDto.ProductId} does not exist.";
                    _logger.LogWarning(badRequest);
                    return BadRequest(badRequest);
                }

                recipeProducts.Add(new RecipeProduct
                {
                    Product = existingProduct,
                    RecipeId = recipeDto.Id,
                    ProductId = existingProduct.Id,
                    Quantity = productDto.Quantity
                    
                });
            }

            var recipe = new Recipe
            {
                Name = recipeDto.Name,
                Description = recipeDto.Description,
                RecipeRawMaterials = recipeRawMaterials,
                RecipeProducts = recipeProducts,
            };

            try
            {
                await _recipeRepo.CreateAsync(recipe);

                if (recipe.Id == 0)
                {
                    _logger.LogError(ApiMessages.ErrorAssigningId);
                    return StatusCode(500, ApiMessages.ErrorAssigningId);
                }

                var rawMaterials = await _recipeRawMaterialRepo.GetAllAsync();
                var products = await _recipeProductRepo.GetAllAsync();
                var supplierRawMaterials = await _supplierRawMaterialRepo.GetAllAsync();
                var suppliers = await _supplierRepo.GetAllAsync();

                var createdRecipeDto = new RecipeDTO
                {
                    Id = recipe.Id,
                    Name = recipe.Name,
                    Description = recipe.Description,
                    RecipeRawMaterials = recipe.RecipeRawMaterials.Select(rm => new RecipeRawMaterialDTO
                    {
                        RawMaterialId = rm.RawMaterialId,
                        RawMaterialName = rawMaterials.First(r => r.RawMaterialId == rm.RawMaterialId).RawMaterial.Name,
                        Quantity = rm.Quantity
                    }).ToList(),
                    RecipeProducts = recipe.RecipeProducts.Select(rp => new RecipeProductDTO
                    {
                        ProductId = rp.ProductId,
                        ProductName = products.First(p => p.ProductId == rp.ProductId).Product.Name,
                        Quantity = rp.Quantity
                    }).ToList(),
                    RecipeSuppliers = suppliers.Select(s => new RecipeSupplierDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Pricing = supplierRawMaterials
                            .Where(srm => srm.SupplierId == s.Id && recipe.RecipeRawMaterials.Any(rrm => rrm.RawMaterialId == srm.RawMaterialId))
                            .ToDictionary(srm => srm.RawMaterial.Name, srm => srm.Price)
                    }).Where(s => s.Pricing.Any()).ToList()
                };

                var actionName = nameof(GetByIdAsync);
                var routeValues = new { id = recipe.Id };
                var uri = Url.Link(actionName, routeValues);

                return Created(uri, createdRecipeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorCreating);
                return StatusCode(500, ApiMessages.ErrorCreating);
            }
        }

        // PUT api/<RecipesController>
        /// <summary>
        /// Updates an existing recipe.
        /// </summary>
        /// <param name="id">The ID of the recipe to update.</param>
        /// <param name="recipeDto">The recipe data transfer object containing the updated details of the recipe.</param>
        /// <returns>An action result indicating the result of the update operation.</returns>
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
                // Check if recipe exists and include related entities
                var existingRecipe = await _recipeRepo.GetAsync(id, query => query
                    .Include(r => r.RecipeRawMaterials)
                        .ThenInclude(rrm => rrm.RawMaterial)
                    .Include(r => r.RecipeProducts)
                        .ThenInclude(rp => rp.Product));

                if (existingRecipe == null)
                {
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }

                // Validate existence of raw materials
                var recipeRawMaterials = new List<RecipeRawMaterial>();
                foreach (var rawMaterialDto in recipeDto.RecipeRawMaterials)
                {
                    var existingRawMaterial = await _rawMaterialRepo.GetAsync(rawMaterialDto.RawMaterialId);
                    if (existingRawMaterial == null)
                    {
                        var badRequest = $"Raw material with ID {rawMaterialDto.RawMaterialId} does not exist.";
                        _logger.LogWarning(badRequest);
                        return BadRequest(badRequest);
                    }

                    recipeRawMaterials.Add(new RecipeRawMaterial
                    {
                        RawMaterialId = rawMaterialDto.RawMaterialId,
                        RawMaterial = existingRawMaterial,
                        Quantity = rawMaterialDto.Quantity
                    });
                }

                // Validate existence of products
                var recipeProducts = new List<RecipeProduct>();
                foreach (var productDto in recipeDto.RecipeProducts)
                {
                    var existingProduct = await _productRepo.GetAsync(productDto.ProductId);
                    if (existingProduct == null)
                    {
                        var badRequest = $"Product with ID {productDto.ProductId} does not exist.";
                        _logger.LogWarning(badRequest);
                        return BadRequest(badRequest);
                    }

                    recipeProducts.Add(new RecipeProduct
                    {
                        Product = existingProduct,
                        RecipeId = existingRecipe.Id,
                        ProductId = existingProduct.Id,
                        Quantity = productDto.Quantity
                    });
                }

                // Update recipe with new values
                existingRecipe.Name = recipeDto.Name;
                existingRecipe.Description = recipeDto.Description;
                existingRecipe.RecipeRawMaterials = recipeRawMaterials;
                existingRecipe.RecipeProducts = recipeProducts;

                // Update recipe in the repository
                await _recipeRepo.UpdateAsync(existingRecipe);

                // Fetch related entities for creating the response DTO
                var suppliers = await _supplierRepo.GetAllAsync();
                var supplierRawMaterials = await _supplierRawMaterialRepo.GetAllAsync();
                var updatedRecipe = await _recipeRepo.GetAsync(existingRecipe.Id, query => query
                    .Include(r => r.RecipeRawMaterials)
                        .ThenInclude(rrm => rrm.RawMaterial)
                    .Include(r => r.RecipeProducts)
                        .ThenInclude(rp => rp.Product));

                // Create DTO for the response
                var updatedRecipeDto = new RecipeDTO
                {
                    Id = updatedRecipe.Id,
                    Name = updatedRecipe.Name,
                    Description = updatedRecipe.Description,
                    RecipeRawMaterials = updatedRecipe.RecipeRawMaterials.Select(rm => new RecipeRawMaterialDTO
                    {
                        RawMaterialId = rm.RawMaterialId,
                        RawMaterialName = rm.RawMaterial?.Name ?? "Unknown", // Handle potential null values
                        Quantity = rm.Quantity
                    }).ToList(),
                    RecipeProducts = updatedRecipe.RecipeProducts.Select(rp => new RecipeProductDTO
                    {
                        ProductId = rp.ProductId,
                        ProductName = rp.Product?.Name ?? "Unknown", // Handle potential null values
                        Quantity = rp.Quantity
                    }).ToList(),
                    RecipeSuppliers = suppliers.Select(s => new RecipeSupplierDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Pricing = supplierRawMaterials
                            .Where(srm => srm.SupplierId == s.Id && updatedRecipe.RecipeRawMaterials.Any(rrm => rrm.RawMaterialId == srm.RawMaterialId))
                            .ToDictionary(srm => srm.RawMaterial?.Name ?? "Unknown", srm => srm.Price) // Handle potential null values
                    }).Where(s => s.Pricing.Any()).ToList()
                };

                return Ok(updatedRecipeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorUpdating);
                return StatusCode(500, ApiMessages.ErrorUpdating);
            }
        }



        // DELETE api/Recipes/
        /// <summary>
        /// Deletes a specific recipe by its ID.
        /// </summary>
        /// <param name="id">The ID of the recipe to delete.</param>
        /// <returns>An action result indicating the result of the deletion operation.</returns>
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
        // POST api/Recipes/optimizeSuppliers/
        /// <summary>
        /// Optimizes the suppliers for a given recipe based on the best prices.
        /// </summary>
        /// <param name="recipeId">The ID of the recipe to optimize suppliers for.</param>
        /// <returns>An action result containing the optimized suppliers and total cost.</returns>
        [HttpPost("optimizeSuppliers")]
        public async Task<IActionResult> OptimizeSuppliers(int recipeId)
        {
            try
            {
                // Load the recipe with its related entities
                var recipe = await _recipeRepo.GetAsync(recipeId, query => query
                    .Include(r => r.RecipeRawMaterials)
                        .ThenInclude(rrm => rrm.RawMaterial)
                    .Include(r => r.RecipeProducts)
                        .ThenInclude(rp => rp.Product));

                if (recipe == null)
                {
                    _logger.LogWarning(ApiMessages.ItemNotfound);
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }

                // Ensure the raw materials and products are loaded
                var rawMaterials = recipe.RecipeRawMaterials;
                if (!rawMaterials.Any())
                {
                    _logger.LogWarning($"{ApiMessages.NoRawMaterialsFound} for Recipe ID: {recipeId}");
                    return BadRequest($"{ApiMessages.NoRawMaterialsFound} for Recipe ID: {recipeId}");
                }

                // Load all suppliers and their raw material prices
                var supplierRawMaterials = await _supplierRawMaterialRepo.GetAllAsync(query => query
                    .Include(srm => srm.RawMaterial)
                    .Include(srm => srm.Supplier));

                if (!supplierRawMaterials.Any())
                {
                    _logger.LogWarning(ApiMessages.ItemNotfound);
                    return BadRequest(ApiMessages.ItemNotfound);
                }

                // Organize supplier prices by raw material ID
                var supplierPrices = supplierRawMaterials
                    .GroupBy(srm => srm.RawMaterialId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToDictionary(
                            srm => srm.SupplierId,
                            srm => srm.Price
                        )
                    );

                // Calculate the optimal combination of suppliers
                var optimizedSuppliers = new List<OptimizedSupplierDTO>();
                decimal totalCost = 0;

                foreach (var rawMaterial in rawMaterials)
                {
                    var materialId = rawMaterial.RawMaterialId;
                    if (supplierPrices.ContainsKey(materialId))
                    {
                        var bestPrice = supplierPrices[materialId].OrderBy(p => p.Value).First();
                        var supplier = supplierRawMaterials.First(srm => srm.SupplierId == bestPrice.Key).Supplier;

                        optimizedSuppliers.Add(new OptimizedSupplierDTO
                        {
                            SupplierId = supplier.Id,
                            SupplierName = supplier.Name,
                            RawMaterialPrices = new Dictionary<string, decimal> { { rawMaterial.RawMaterial.Name, bestPrice.Value } },
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
                    RecipeName = recipe.Name,
                    RecipeId = recipeId,
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
