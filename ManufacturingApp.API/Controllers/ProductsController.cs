﻿using Azure.Messaging;
using ManufacturingApp.API.DTOs;
using ManufacturingApp.API.Extensions;
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
    public class ProductsController : ControllerBase
    {
        private readonly IManufacturingRepository<Product> _productsRepo;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IManufacturingRepository<Product> repo, ILogger<ProductsController> logger)
        {
            _productsRepo = repo;
            _logger = logger;
        }

        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var products = await _productsRepo.GetAllAsync();

                if (products.IsNullOrEmpty())
                {
                    return NoContent();
                }

                var productDtos = products.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    SellingPrice = p.SellingPrice
                }).ToList();
                
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingAll);
                return StatusCode(500, ApiMessages.ErrorGettingAll);
            }
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var product = await _productsRepo.GetAsync(id);
                if (product == null)
                {
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }

                var productDto = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    SellingPrice = product.SellingPrice
                };

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingById);
                return StatusCode(500, ApiMessages.ErrorGettingById);
            }
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"{ApiMessages.InvalidModelWarning} : {ModelState}");
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                SellingPrice = productDto.SellingPrice
            };

            try
            {
                await _productsRepo.CreateAsync(product);

                if (product.Id == 0)
                {
                    _logger.LogError(ApiMessages.ErrorAssigningId);
                    return StatusCode(500, ApiMessages.ErrorAssigningId);
                }

                var actionName = nameof(GetByIdAsync);
                var routeValues = new { id = product.Id };
                var uri = Url.Link(actionName, routeValues);

                var createdProductDto = new RawMaterialDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description
                };

                return Created(uri, createdProductDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorCreating);
                return StatusCode(500, ApiMessages.ErrorCreating);
            }
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(ApiMessages.InvalidModelWarning);
                return BadRequest(ModelState);
            }

            try
            {
                var existingProduct = await _productsRepo.GetAsync(id);
                if (existingProduct == null)
                {
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }

                existingProduct.Name = productDto.Name;
                existingProduct.Description = productDto.Description;
                existingProduct.SellingPrice = productDto.SellingPrice;

                await _productsRepo.UpdateAsync(existingProduct);
                return Ok(new { MessageContent = ApiMessages.SuccessUpdated });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorUpdating);
                return StatusCode(500, ApiMessages.ErrorUpdating);
            }
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var product = _productsRepo.GetAsync(id);
                if (product == null)
                {
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }
                await _productsRepo.DeleteAsync(id);
                return Ok(new { MessageContent = ApiMessages.SuccessDeleted });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorDeleting);
                return StatusCode(500, ApiMessages.ErrorDeleting); 
            }
        }
    }
}
