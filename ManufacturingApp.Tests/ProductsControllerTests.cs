using ManufacturingApp.API.Controllers;
using ManufacturingApp.API.DTOs;
using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ManufacturingApp.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IManufacturingRepository<Product>> _mockRepo;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockRepo = new Mock<IManufacturingRepository<Product>>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Description = "Description1", SellingPrice = 10 },
                new Product { Id = 2, Name = "Product2", Description = "Description2", SellingPrice = 20 }
            };

            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProducts = Assert.IsType<List<ProductDTO>>(okResult.Value);
            Assert.Equal(2, returnProducts.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product1", Description = "Description1", SellingPrice = 10 };
            _mockRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsType<ProductDTO>(okResult.Value);
            Assert.Equal(product.Id, returnProduct.Id);
        }

        [Fact]
        public async Task CreateAsync_ValidObject_ReturnsCreated()
        {
            // Arrange
            var productDto = new ProductDTO { Name = "Product1", Description = "Description1", SellingPrice = 10 };
            var product = new Product { Id = 1, Name = "Product1", Description = "Description1", SellingPrice = 10 };

            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateAsync(productDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode); // Check if the status code is 201 Created
            var returnProduct = Assert.IsType<ProductDTO>(objectResult.Value);
            Assert.Equal(product.Name, returnProduct.Name);
        }



    }
}
