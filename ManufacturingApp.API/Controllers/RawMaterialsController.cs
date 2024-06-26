﻿using Azure.Messaging;
using ManufacturingApp.API.DTOs;
using ManufacturingApp.API.Extensions;
using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Data;
using ManufacturingApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ManufacturingApp.API.Controllers
{
    /// <summary>
    /// Controller for managing raw materials.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RawMaterialsController : ControllerBase
    {
        private readonly IManufacturingRepository<RawMaterial> _rawMaterialsRepo;
        private readonly ILogger<RawMaterialsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawMaterialsController"/> class.
        /// </summary>
        /// <param name="repo">Repository for managing raw materials.</param>
        /// <param name="logger">Logger instance for logging errors and information.</param>
        public RawMaterialsController(IManufacturingRepository<RawMaterial> repo, 
                                        ILogger<RawMaterialsController> logger)
        {
            _rawMaterialsRepo = repo;
            _logger = logger;
        }

        // GET: api/<RawMaterialController>
        /// <summary>
        /// Retrieves all raw materials from the repository.
        /// </summary>
        /// <returns>An action result containing a list of all raw materials.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var rawMaterials = await _rawMaterialsRepo.GetAllAsync();
                if (rawMaterials.IsNullOrEmpty())
                {
                    return NoContent();
                }
                var rawMaterialsDtos = rawMaterials.Select(rm => new RawMaterialDTO
                {
                    Id = rm.Id,
                    Name = rm.Name,
                    Description = rm.Description
                }).ToList();

                return Ok(rawMaterialsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingAll);
                return StatusCode(500, ApiMessages.ErrorGettingAll);
            }
        }

        // GET api/<RawMaterialController>/5
        /// <summary>
        /// Retrieves a specific raw material by its ID.
        /// </summary>
        /// <param name="id">The ID of the raw material to retrieve.</param>
        /// <returns>An action result containing the raw material with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var rawMat = await _rawMaterialsRepo.GetAsync(id);
                if (rawMat == null)
                {
                    return NotFound(new {MessageContent = ApiMessages.ItemNotfound});
                }
                var rawMaterialDto = new RawMaterialDTO
                {
                    Id = rawMat.Id,
                    Name = rawMat.Name,
                    Description = rawMat.Description
                };
                return Ok(rawMaterialDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingById);
                return StatusCode(500, ApiMessages.ErrorGettingById);
            }
        }

        // POST api/<RawMaterialController>
        /// <summary>
        /// Creates a new raw material.
        /// </summary>
        /// <param name="rawMaterialDto">The raw material data transfer object containing the details of the raw material to create.</param>
        /// <returns>An action result containing the created raw material.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RawMaterialDTO rawMaterialDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"{ApiMessages.InvalidModelWarning} : {ModelState}");
                return BadRequest(ModelState);
            }
            var rawMaterial = new RawMaterial
            {
                Id = rawMaterialDto.Id,
                Name = rawMaterialDto.Name,
                Description = rawMaterialDto.Description
            };
            try
            {
                await _rawMaterialsRepo.CreateAsync(rawMaterial);
                if (rawMaterial.Id == 0)
                {
                    _logger.LogError(ApiMessages.ErrorAssigningId);
                    return StatusCode(500, ApiMessages.ErrorAssigningId);
                }

                var actionName = nameof(GetByIdAsync);
                var routeValues = new { id = rawMaterial.Id };
                var uri = Url.Link(actionName, routeValues);

                var createdRawMaterialDto = new RawMaterialDTO
                {
                    Id = rawMaterial.Id,
                    Name = rawMaterial.Name,
                    Description = rawMaterial.Description
                };

                return Created(uri, createdRawMaterialDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorCreating);
                return StatusCode(500, ApiMessages.ErrorCreating); 
            }
        }

        // PUT api/<RawMaterialController>/5
        /// <summary>
        /// Updates an existing raw material.
        /// </summary>
        /// <param name="id">The ID of the raw material to update.</param>
        /// <param name="rawMaterialDto">The raw material data transfer object containing the updated details of the raw material.</param>
        /// <returns>An action result indicating the result of the update operation.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] RawMaterialDTO rawMaterialDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"{ApiMessages.InvalidModelWarning} : {ModelState}");
                return BadRequest(ModelState);
            }
            try
            {
                var rawMaterial = await _rawMaterialsRepo.GetAsync(id);
                if (rawMaterial == null)
                {
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }
                rawMaterial.Name = rawMaterialDto.Name;
                rawMaterial.Description = rawMaterialDto.Description;

                await _rawMaterialsRepo.UpdateAsync(rawMaterial);
                return Ok(new { MessageContent = ApiMessages.SuccessUpdated });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorUpdating);
                return StatusCode(500, ApiMessages.ErrorUpdating);
            }
        }

        // DELETE api/<RawMaterialController>/5
        /// <summary>
        /// Deletes a specific raw material by its ID.
        /// </summary>
        /// <param name="id">The ID of the raw material to delete.</param>
        /// <returns>An action result indicating the result of the deletion operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var existingRawMaterial = await _rawMaterialsRepo.GetAsync(id);
                if (existingRawMaterial == null)
                {
                    _logger.LogWarning($"{ApiMessages.InvalidModelWarning} : {ModelState}");
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }

                await _rawMaterialsRepo.DeleteAsync(id);
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
