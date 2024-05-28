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
    public class SuppliersControllerTests
    {
        private readonly Mock<IManufacturingRepository<Supplier>> _mockSupplierRepo;
        private readonly Mock<IManufacturingRepository<RawMaterial>> _mockRawMaterialRepo;
        private readonly Mock<ILogger<SuppliersController>> _mockLogger;
        private readonly SuppliersController _controller;

        public SuppliersControllerTests()
        {
            _mockSupplierRepo = new Mock<IManufacturingRepository<Supplier>>();
            _mockRawMaterialRepo = new Mock<IManufacturingRepository<RawMaterial>>();
            _mockLogger = new Mock<ILogger<SuppliersController>>();
            _controller = new SuppliersController(_mockSupplierRepo.Object, _mockLogger.Object, _mockRawMaterialRepo.Object);

            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            var actionDescriptor = new ControllerActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
            _controller.ControllerContext = new ControllerContext(actionContext);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost/api/suppliers/1");
            _controller.Url = urlHelper.Object;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithListOfSuppliers()
        {
            var suppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "Supplier1", Description = "Description1" },
                new Supplier { Id = 2, Name = "Supplier2", Description = "Description2" }
            };

            _mockSupplierRepo.Setup(repo => repo.GetAllAsync(It.IsAny<Func<IQueryable<Supplier>, IQueryable<Supplier>>>())).ReturnsAsync(suppliers);

            var result = await _controller.GetAllAsync();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnSuppliers = Assert.IsType<List<SupplierDTO>>(okResult.Value);
            Assert.Equal(2, returnSuppliers.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsOkResult_WithSupplier()
        {
            var supplier = new Supplier { Id = 1, Name = "Supplier1", Description = "Description1" };
            _mockSupplierRepo.Setup(repo => repo.GetAsync(1, It.IsAny<Func<IQueryable<Supplier>, IQueryable<Supplier>>>())).ReturnsAsync(supplier);

            var result = await _controller.GetByIdAsync(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnSupplier = Assert.IsType<SupplierDTO>(okResult.Value);
            Assert.Equal(supplier.Id, returnSupplier.Id);
        }

        [Fact]
        public async Task CreateAsync_ValidObject_ReturnsCreated()
        {
            var supplierDto = new SupplierDTO { Name = "Supplier1", Description = "Description1" };
            var supplier = new Supplier { Id = 1, Name = "Supplier1", Description = "Description1" };

            _mockSupplierRepo.Setup(repo => repo.CreateAsync(It.IsAny<Supplier>())).Callback<Supplier>(s => s.Id = 1).Returns(Task.CompletedTask);
            _mockSupplierRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync(supplier);

            var result = await _controller.CreateAsync(supplierDto);

            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var returnSupplier = Assert.IsType<SupplierDTO>(createdResult.Value);
            Assert.Equal(supplierDto.Name, returnSupplier.Name);
        }

        [Fact]
        public async Task UpdateAsync_ValidObject_ReturnsOk()
        {
            var supplierDto = new SupplierDTO { Name = "UpdatedSupplier", Description = "UpdatedDescription" };
            var supplier = new Supplier { Id = 1, Name = "Supplier1", Description = "Description1" };

            _mockSupplierRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(supplier);
            _mockSupplierRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Supplier>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateAsync(1, supplierDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value;
            var messageProperty = responseObject.GetType().GetProperty("MessageContent");
            var messageValue = messageProperty.GetValue(responseObject, null).ToString();
            Assert.Equal(ApiMessages.SuccessUpdated, messageValue);
        }

        [Fact]
        public async Task DeleteAsync_ExistingId_ReturnsOk()
        {
            var supplier = new Supplier { Id = 1, Name = "Supplier1", Description = "Description1" };
            _mockSupplierRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(supplier);
            _mockSupplierRepo.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsync(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = okResult.Value.GetType().GetProperty("MessageContent")?.GetValue(okResult.Value, null).ToString();
            Assert.Equal(ApiMessages.SuccessDeleted, message);
        }
    }
}
