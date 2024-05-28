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
    public class RawMaterialsControllerTests
    {
        private readonly Mock<IManufacturingRepository<RawMaterial>> _mockRepo;
        private readonly Mock<ILogger<RawMaterialsController>> _mockLogger;
        private readonly RawMaterialsController _controller;

        public RawMaterialsControllerTests()
        {
            _mockRepo = new Mock<IManufacturingRepository<RawMaterial>>();
            _mockLogger = new Mock<ILogger<RawMaterialsController>>();
            _controller = new RawMaterialsController(_mockRepo.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            var actionDescriptor = new ControllerActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
            _controller.ControllerContext = new ControllerContext(actionContext);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost/api/rawmaterials/1");
            _controller.Url = urlHelper.Object;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithListOfRawMaterials()
        {
            var rawMaterials = new List<RawMaterial>
            {
                new RawMaterial { Id = 1, Name = "Material1", Description = "Description1" },
                new RawMaterial { Id = 2, Name = "Material2", Description = "Description2" }
            };

            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(rawMaterials);

            var result = await _controller.GetAllAsync();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnRawMaterials = Assert.IsType<List<RawMaterialDTO>>(okResult.Value);
            Assert.Equal(2, returnRawMaterials.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsOkResult_WithRawMaterial()
        {
            var rawMaterial = new RawMaterial { Id = 1, Name = "Material1", Description = "Description1" };
            _mockRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(rawMaterial);

            var result = await _controller.GetByIdAsync(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnRawMaterial = Assert.IsType<RawMaterialDTO>(okResult.Value);
            Assert.Equal(rawMaterial.Id, returnRawMaterial.Id);
        }

        [Fact]
        public async Task CreateAsync_ValidObject_ReturnsCreated()
        {
            var rawMaterialDto = new RawMaterialDTO { Name = "Material1", Description = "Description1" };
            var rawMaterial = new RawMaterial { Id = 1, Name = "Material1", Description = "Description1" };

            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<RawMaterial>())).Callback<RawMaterial>(r => r.Id = 1).Returns(Task.CompletedTask);
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync(rawMaterial);

            var result = await _controller.CreateAsync(rawMaterialDto);

            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var returnRawMaterial = Assert.IsType<RawMaterialDTO>(createdResult.Value);
            Assert.Equal(rawMaterialDto.Name, returnRawMaterial.Name);
        }

        [Fact]
        public async Task UpdateAsync_ValidObject_ReturnsOk()
        {
            var rawMaterialDto = new RawMaterialDTO { Name = "UpdatedMaterial", Description = "UpdatedDescription" };
            var rawMaterial = new RawMaterial { Id = 1, Name = "Material1", Description = "Description1" };

            _mockRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(rawMaterial);
            _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<RawMaterial>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateAsync(1, rawMaterialDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value;
            var messageProperty = responseObject.GetType().GetProperty("MessageContent");
            var messageValue = messageProperty.GetValue(responseObject, null).ToString();
            Assert.Equal(ApiMessages.SuccessUpdated, messageValue);
        }

        [Fact]
        public async Task DeleteAsync_ExistingId_ReturnsOk()
        {
            var rawMaterial = new RawMaterial { Id = 1, Name = "Material1", Description = "Description1" };
            _mockRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(rawMaterial);
            _mockRepo.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsync(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = okResult.Value.GetType().GetProperty("MessageContent")?.GetValue(okResult.Value, null).ToString();
            Assert.Equal(ApiMessages.SuccessDeleted, message);
        }
    }
}
