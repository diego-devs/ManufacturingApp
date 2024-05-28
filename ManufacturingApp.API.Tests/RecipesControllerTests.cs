using ManufacturingApp.API;
using ManufacturingApp.API.Controllers;
using ManufacturingApp.API.DTOs;
using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;

namespace ManufacturingApp.Tests
{
    public class RecipesControllerTests
    {
        private readonly Mock<IManufacturingRepository<Recipe>> _mockRecipeRepo;
        private readonly Mock<IManufacturingRepository<RecipeRawMaterial>> _mockRecipeRawMaterialRepo;
        private readonly Mock<IManufacturingRepository<RawMaterial>> _mockRawMaterialRepo;
        private readonly Mock<IManufacturingRepository<RecipeProduct>> _mockRecipeProductRepo;
        private readonly Mock<IManufacturingRepository<Supplier>> _mockSupplierRepo;
        private readonly Mock<IManufacturingRepository<SupplierRawMaterial>> _mockSupplierRawMaterialRepo;
        private readonly Mock<IManufacturingRepository<Product>> _mockProductRepo;
        private readonly Mock<ILogger<RecipesController>> _mockLogger;
        private readonly RecipesController _controller;

        public RecipesControllerTests()
        {
            _mockRecipeRepo = new Mock<IManufacturingRepository<Recipe>>();
            _mockRecipeRawMaterialRepo = new Mock<IManufacturingRepository<RecipeRawMaterial>>();
            _mockRawMaterialRepo = new Mock<IManufacturingRepository<RawMaterial>>();
            _mockRecipeProductRepo = new Mock<IManufacturingRepository<RecipeProduct>>();
            _mockSupplierRepo = new Mock<IManufacturingRepository<Supplier>>();
            _mockSupplierRawMaterialRepo = new Mock<IManufacturingRepository<SupplierRawMaterial>>();
            _mockProductRepo = new Mock<IManufacturingRepository<Product>>();
            _mockLogger = new Mock<ILogger<RecipesController>>();
            _controller = new RecipesController(
                _mockRecipeRepo.Object,
                _mockLogger.Object,
                _mockRecipeRawMaterialRepo.Object,
                _mockRecipeProductRepo.Object,
                _mockSupplierRepo.Object,
                _mockSupplierRawMaterialRepo.Object,
                _mockRawMaterialRepo.Object,
                _mockProductRepo.Object
            );

            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            var actionDescriptor = new ControllerActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
            _controller.ControllerContext = new ControllerContext(actionContext);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost/api/recipes/1");
            _controller.Url = urlHelper.Object;
        }
        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithListOfRecipes()
        {
            // Arrange
            var recipes = new List<Recipe>
            {
                new Recipe { Id = 1, Name = "Recipe1", Description = "Description1", RecipeRawMaterials = new List<RecipeRawMaterial>
                {
                    new RecipeRawMaterial { RawMaterialId = 1, Quantity = 10, RawMaterial = new RawMaterial { Id = 1, Name = "Material1" } }
                },
                RecipeProducts = new List<RecipeProduct>
                {
                    new RecipeProduct { ProductId = 1, Quantity = 5, Product = new Product { Id = 1, Name = "Product1" } }
                }
            },
            new Recipe { Id = 2, Name = "Recipe2", Description = "Description2", RecipeRawMaterials = new List<RecipeRawMaterial>
            {
                new RecipeRawMaterial { RawMaterialId = 2, Quantity = 20, RawMaterial = new RawMaterial { Id = 2, Name = "Material2" } }
            },
            RecipeProducts = new List<RecipeProduct>
            {
                new RecipeProduct { ProductId = 2, Quantity = 10, Product = new Product { Id = 2, Name = "Product2" } }
            }
        }
    };
            var suppliers = new List<Supplier>
    {
        new Supplier { Id = 1, Name = "Supplier1", SupplierRawMaterials = new List<SupplierRawMaterial>
            {
                new SupplierRawMaterial { SupplierId = 1, RawMaterialId = 1, Price = 100, RawMaterial = new RawMaterial { Id = 1, Name = "Material1" } }
            }
        },
        new Supplier { Id = 2, Name = "Supplier2", SupplierRawMaterials = new List<SupplierRawMaterial>
            {
                new SupplierRawMaterial { SupplierId = 2, RawMaterialId = 2, Price = 200, RawMaterial = new RawMaterial { Id = 2, Name = "Material2" } }
            }
        }
    };

            _mockRecipeRepo.Setup(repo => repo.GetAllAsync(It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>())).ReturnsAsync(recipes);
            _mockSupplierRepo.Setup(repo => repo.GetAllAsync(It.IsAny<Func<IQueryable<Supplier>, IQueryable<Supplier>>>())).ReturnsAsync(suppliers);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var json = Assert.IsType<string>(okResult.Value);
            var returnRecipes = JsonSerializer.Deserialize<List<RecipeDTO>>(json);

            Assert.Equal(2, returnRecipes.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsOkResult_WithRecipe()
        {
            // Arrange
            var recipe = new Recipe
            {
                Id = 1,
                Name = "Recipe1",
                Description = "Description1",
                RecipeRawMaterials = new List<RecipeRawMaterial>
                {
                    new RecipeRawMaterial { RawMaterialId = 1, Quantity = 10, RawMaterial = new RawMaterial { Id = 1, Name = "Material1" } }
                },
                RecipeProducts = new List<RecipeProduct>
                {
                    new RecipeProduct { ProductId = 1, Quantity = 5, Product = new Product { Id = 1, Name = "Product1" } }
                }
            };

            var suppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "Supplier1", SupplierRawMaterials = new List<SupplierRawMaterial>
                    {
                        new SupplierRawMaterial { SupplierId = 1, RawMaterialId = 1, Price = 100, RawMaterial = new RawMaterial { Id = 1, Name = "Material1" } }
                    }
                }
            };

            _mockRecipeRepo.Setup(repo => repo.GetAsync(1, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>())).ReturnsAsync(recipe);
            _mockSupplierRepo.Setup(repo => repo.GetAllAsync(It.IsAny<Func<IQueryable<Supplier>, IQueryable<Supplier>>>())).ReturnsAsync(suppliers);

            // Act
            var result = await _controller.GetByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnRecipe = Assert.IsType<RecipeDTO>(okResult.Value);
            Assert.Equal(recipe.Id, returnRecipe.Id);
            Assert.Equal(recipe.Name, returnRecipe.Name);
            Assert.Equal(recipe.Description, returnRecipe.Description);
        }



        [Fact]
        public async Task CreateAsync_ValidObject_ReturnsCreated()
        {
            // Arrange
            var recipeDto = new RecipeDTO { Name = "Recipe1", Description = "Description1" };
            var recipe = new Recipe { Id = 1, Name = "Recipe1", Description = "Description1" };
            var rawMaterials = new List<RecipeRawMaterial>
            {
                new RecipeRawMaterial { RecipeId = 1, RawMaterialId = 1, Quantity = 10, RawMaterial = new RawMaterial { Id = 1, Name = "Material1" } }
            };
                    var products = new List<RecipeProduct>
            {
                new RecipeProduct { RecipeId = 1, ProductId = 1, Quantity = 5, Product = new Product { Id = 1, Name = "Product1" } }
            };
                    var supplierRawMaterials = new List<SupplierRawMaterial>
            {
                new SupplierRawMaterial { SupplierId = 1, RawMaterialId = 1, Price = 100, RawMaterial = new RawMaterial { Id = 1, Name = "Material1" } }
            };
                    var suppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "Supplier1", SupplierRawMaterials = supplierRawMaterials }
            };

            _mockRecipeRepo.Setup(repo => repo.CreateAsync(It.IsAny<Recipe>())).Callback<Recipe>(r => r.Id = 1).Returns(Task.CompletedTask);
            _mockRecipeRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync(recipe);
            _mockRecipeRawMaterialRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(rawMaterials);
            _mockRecipeProductRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);
            _mockSupplierRawMaterialRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(supplierRawMaterials);
            _mockSupplierRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(suppliers);

            // Act
            var result = await _controller.CreateAsync(recipeDto);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var returnRecipe = Assert.IsType<RecipeDTO>(createdResult.Value);
            Assert.Equal(recipeDto.Name, returnRecipe.Name);
        }


        [Fact]
        public async Task UpdateAsync_ValidObject_ReturnsOk()
        {
            var recipeDto = new RecipeDTO { Name = "UpdatedRecipe", Description = "UpdatedDescription" };
            var recipe = new Recipe { Id = 1, Name = "Recipe1", Description = "Description1" };

            _mockRecipeRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(recipe);
            _mockRecipeRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Recipe>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateAsync(1, recipeDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value;
            var messageProperty = responseObject.GetType().GetProperty("MessageContent");
            var messageValue = messageProperty.GetValue(responseObject, null).ToString();
            Assert.Equal(ApiMessages.SuccessUpdated, messageValue);
        }

        [Fact]
        public async Task DeleteAsync_ExistingId_ReturnsOk()
        {
            var recipe = new Recipe { Id = 1, Name = "Recipe1", Description = "Description1" };
            _mockRecipeRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(recipe);
            _mockRecipeRepo.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsync(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = okResult.Value.GetType().GetProperty("MessageContent")?.GetValue(okResult.Value, null).ToString();
            Assert.Equal(ApiMessages.SuccessDeleted, message);
        }
    }
}
