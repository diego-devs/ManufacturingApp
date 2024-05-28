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

            // Setting up the ControllerContext to avoid NullReferenceException on Url.Link
            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            var actionDescriptor = new ControllerActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
            _controller.ControllerContext = new ControllerContext(actionContext);

            // Mock UrlHelper
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost/api/products/1");
            _controller.Url = urlHelper.Object;
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

            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Product>())).Callback<Product>(p => p.Id = 1).Returns(Task.CompletedTask);
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync(product);

            // Mock UrlHelper to generate a proper link
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost/api/products/1");
            _controller.Url = urlHelper.Object;

            // Act
            var result = await _controller.CreateAsync(productDto);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, createdResult.StatusCode); // Check if the status code is 201 Created
            var returnProduct = Assert.IsType<ProductDTO>(createdResult.Value);
            Assert.Equal(productDto.Name, returnProduct.Name);
        }

        [Fact]
        public async Task UpdateAsync_ValidObject_ReturnsOk()
        {
            // Arrange
            var productDto = new ProductDTO { Name = "UpdatedProduct", Description = "UpdatedDescription", SellingPrice = 20 };
            var product = new Product { Id = 1, Name = "Product1", Description = "Description1", SellingPrice = 10 };

            _mockRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(product);
            _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateAsync(1, productDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value;
            var messageProperty = responseObject.GetType().GetProperty("MessageContent");
            var messageValue = messageProperty.GetValue(responseObject, null).ToString();
            Assert.Equal(ApiMessages.SuccessUpdated, messageValue);
        }



        [Fact]
        public async Task DeleteAsync_ExistingId_ReturnsOk()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product1", Description = "Description1", SellingPrice = 10 };
            _mockRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(product);
            _mockRepo.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = okResult.Value.GetType().GetProperty("MessageContent")?.GetValue(okResult.Value, null).ToString();
            Assert.Equal(ApiMessages.SuccessDeleted, message);
        }
    }
}
