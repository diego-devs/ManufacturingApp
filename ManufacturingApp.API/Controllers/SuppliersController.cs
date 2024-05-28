using Azure.Messaging;
using ManufacturingApp.API.Data;
using ManufacturingApp.API.DTOs;
using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ManufacturingApp.API.Controllers
{
    /// <summary>
    /// Controller for managing suppliers.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly IManufacturingRepository<Supplier> _supplierRepo;
        private readonly IManufacturingRepository<RawMaterial> _rawMaterialRepo;
        private readonly ILogger<SuppliersController> _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="SuppliersController"/> class.
        /// </summary>
        /// <param name="repo">Repository for managing suppliers.</param>
        /// <param name="logger">Logger instance for logging errors and information.</param>
        /// <param name="rawMaterialRepo">Repository for managing raw materials.</param>
        public SuppliersController(IManufacturingRepository<Supplier> repo,
                                   ILogger<SuppliersController> logger,
                                   IManufacturingRepository<RawMaterial> rawMaterialRepo)
        {
            _supplierRepo = repo;
            _logger = logger;
            _rawMaterialRepo = rawMaterialRepo;
        }

        // GET: api/<SuppliersController>
        /// <summary>
        /// Retrieves all suppliers from the repository.
        /// </summary>
        /// <returns>An action result containing a list of all suppliers.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var suppliers = await _supplierRepo.GetAllAsync(query => query.Include(s => s.SupplierRawMaterials).ThenInclude(srm => srm.RawMaterial));
                var supplierDtos = suppliers.Select(s => new SupplierDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    SupplierRawMaterials = s.SupplierRawMaterials.Select(srm => new SupplierRawMaterialDTO
                    {
                        RawMaterialId = srm.RawMaterialId,
                        RawMaterialName = srm.RawMaterial.Name,
                        Price = srm.Price
                    }).ToList()
                }).ToList();

                return Ok(supplierDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingAll);
                return StatusCode(500, ApiMessages.ErrorGettingAll);
            }
        }

        // GET api/<SuppliersController>/5
        /// <summary>
        /// Retrieves a specific supplier by its ID.
        /// </summary>
        /// <param name="id">The ID of the supplier to retrieve.</param>
        /// <returns>An action result containing the supplier with the specified ID.</returns>
        [HttpGet("{id}", Name = "GetByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var supplier = await _supplierRepo.GetAsync(id, query => query.Include(s => s.SupplierRawMaterials).ThenInclude(srm => srm.RawMaterial));
                if (supplier == null)
                {
                    _logger.LogWarning(ApiMessages.ItemNotfound);
                    return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
                }

                var supplierDto = new SupplierDTO
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    Description = supplier.Description,
                    SupplierRawMaterials = supplier.SupplierRawMaterials.Select(srm => new SupplierRawMaterialDTO
                    {
                        RawMaterialId = srm.RawMaterialId,
                        RawMaterialName = srm.RawMaterial.Name,
                        Price = srm.Price
                    }).ToList()
                };

                return Ok(supplierDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingById);
                return StatusCode(500, ApiMessages.ErrorGettingById);
            }
        }

        // POST api/<SuppliersController>
        /// <summary>
        /// Creates a new supplier.
        /// </summary>
        /// <param name="supplierDto">The supplier data transfer object containing the details of the supplier to create.</param>
        /// <returns>An action result containing the created supplier.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] SupplierDTO supplierDto)
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ApiMessages.InvalidModelWarning);
                return BadRequest(ModelState);
            }

            // Validate existence of raw materials and map them
            var supplierRawMaterials = new List<SupplierRawMaterial>();
            foreach (var rawMaterialDto in supplierDto.SupplierRawMaterials)
            {
                var existingRawMaterial = await _rawMaterialRepo.GetAsync(rawMaterialDto.RawMaterialId);
                if (existingRawMaterial == null)
                {
                    var badRequest = $"Raw material with ID {rawMaterialDto.RawMaterialId} does not exist.";
                    _logger.LogWarning(badRequest);
                    return BadRequest(badRequest);
                }

                supplierRawMaterials.Add(new SupplierRawMaterial
                {
                    RawMaterial = existingRawMaterial, // Ensure RawMaterial entity is mapped
                    RawMaterialId = rawMaterialDto.RawMaterialId,
                    Price = rawMaterialDto.Price
                });
            }
            // Create the object model to create
            var supplier = new Supplier
            {
                Name = supplierDto.Name,
                Description = supplierDto.Description,
                SupplierRawMaterials = supplierRawMaterials
            };

            try
            {
                await _supplierRepo.CreateAsync(supplier);

                if (supplier.Id == 0)
                {
                    _logger.LogError(ApiMessages.ErrorAssigningId);
                    return StatusCode(500, ApiMessages.ErrorAssigningId);
                }

                var actionName = nameof(GetByIdAsync);
                var routeValues = new { id = supplier.Id };
                //var uri = Url.Action(actionName, routeValues);

                // Use Url.Link to generate the URI
                var uri = Url.Link(actionName, routeValues);

                var createdSupplierDto = new SupplierDTO
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    Description = supplier.Description,
                    SupplierRawMaterials = supplier.SupplierRawMaterials.Select(srm => new SupplierRawMaterialDTO
                    {
                        RawMaterialId = srm.RawMaterialId,
                        RawMaterialName = srm.RawMaterial.Name,
                        Price = srm.Price
                    }).ToList()
                };

                return Created(uri, createdSupplierDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorCreating);
                return StatusCode(500, ApiMessages.ErrorCreating);
            }
        }

        // PUT api/<SuppliersController>/5
        /// <summary>
        /// Updates an existing supplier.
        /// </summary>
        /// <param name="id">The ID of the supplier to update.</param>
        /// <param name="supplierDto">The supplier data transfer object containing the updated details of the supplier.</param>
        /// <returns>An action result indicating the result of the update operation.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] SupplierDTO supplierDto)
        {
            var supplier = await _supplierRepo.GetAsync(id);
            if (supplier == null)
            {
                return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
            }
            supplier.Name = supplierDto.Name;
            supplier.Description = supplierDto.Description;
            supplier.SupplierRawMaterials = supplierDto.SupplierRawMaterials.Select(srm => new SupplierRawMaterial
            {
                SupplierId = supplier.Id,
                RawMaterialId = srm.RawMaterialId,
                Price = srm.Price
            }).ToList();
            try
            {
                await _supplierRepo.UpdateAsync(supplier);
                return Ok(new { MessageContent = ApiMessages.SuccessUpdated });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorUpdating);
                return StatusCode(500, ApiMessages.ErrorUpdating);
            }
        }

        // DELETE api/<SuppliersController>/5
        /// <summary>
        /// Deletes a specific supplier by its ID.
        /// </summary>
        /// <param name="id">The ID of the supplier to delete.</param>
        /// <returns>An action result indicating the result of the deletion operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var supplier = await _supplierRepo.GetAsync(id);
            if (supplier == null)
            {
                return NotFound(new { MessageContent = ApiMessages.ItemNotfound });
            }
            try
            {
                await _supplierRepo.DeleteAsync(id);
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
