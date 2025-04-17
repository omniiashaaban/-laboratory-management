using laboratory.DAL.DTOs;
using laboratory.DAL.Models;
using laboratory.DAL.Repository;
using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace laboratory.API.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly GenericRepository<Material> _materialRepository;

        public MaterialController(GenericRepository<Material> materialRepository)
        {
            _materialRepository = materialRepository;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialDTO>>> GetMaterials()
        {
            var materials = await _materialRepository.GetAllAsync();
            var materialDTOs = materials.Select(MapToDTO).ToList();
            return Ok(materialDTOs);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialDTO>> GetMaterial(int id)
        {
            var material = await _materialRepository.GetByIdAsync(id);
            if (material == null) return NotFound();
            return Ok(MapToDTO(material));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddMaterial([FromBody] MaterialDTO materialDTO)
        {
            var material = MapToEntity(materialDTO);
            await _materialRepository.AddAsync(material);
            return CreatedAtAction(nameof(GetMaterial), new { id = material.Id }, MapToDTO(material));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMaterial(int id, [FromBody] MaterialDTO materialDTO)
        {
            if (id != materialDTO.Id)
                return BadRequest(new ApiResponse(400, "The provided ID does not match the material ID."));

            if (string.IsNullOrWhiteSpace(materialDTO.Name) ||
                
                string.IsNullOrWhiteSpace(materialDTO.Type))
            {
                return BadRequest(new ApiResponse(400, "Name, Code, and Type are required fields."));
            }

            if (materialDTO.Quantity < 0)
                return BadRequest(new ApiResponse(400, "Quantity cannot be negative."));

            if (materialDTO.ProductionDate >= materialDTO.ExpirationDate)
                return BadRequest(new ApiResponse(400, "Production date must be before expiration date."));

            var existingMaterial = await _materialRepository.GetByIdAsync(id);
            if (existingMaterial == null)
                return NotFound(new ApiResponse(404, "Material not found."));

            existingMaterial.Name = materialDTO.Name;
          
            existingMaterial.Type = materialDTO.Type;
            existingMaterial.Quantity = materialDTO.Quantity;
            existingMaterial.ProductionDate = materialDTO.ProductionDate;
            existingMaterial.ExpirationDate = materialDTO.ExpirationDate;

            await _materialRepository.UpdateAsync(existingMaterial);

            return Ok(new ApiResponse(200, "Material updated successfully."));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMaterial(int id)
        {
            var existingMaterial = await _materialRepository.GetByIdAsync(id);
            if (existingMaterial == null)
                return NotFound(new ApiResponse(404, "Material not found."));

            await _materialRepository.DeleteAsync(id);

            return Ok(new ApiResponse(200, "Material deleted successfully."));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Materials/expiring")]
        public async Task<ActionResult<IEnumerable<MaterialDTO>>> GetExpiringChemicals()
        {
            var today = DateTime.UtcNow;
            var thresholdDate = today.AddMonths(1); // اعتبري المواد التي ستنتهي خلال شهر

            var expiringChemicals = await _materialRepository.GetAllAsync();

            var expiring = expiringChemicals.Where(c => c.ExpirationDate <= thresholdDate);

            var chemicalDTOs = expiringChemicals.Select(c => new MaterialDTO
            {
                Id = c.Id,
                Name = c.Name,
                ExpirationDate = c.ExpirationDate
            }).ToList();

            return Ok(chemicalDTOs);
        }

        // Manual mapping 
        private MaterialDTO MapToDTO(Material material)
        {
            return new MaterialDTO
            {
                Id = material.Id,
                Name = material.Name,
           
                Type = material.Type,
                Quantity = material.Quantity,
                ProductionDate = material.ProductionDate,
                ExpirationDate = material.ExpirationDate
            };
        }

        private Material MapToEntity(MaterialDTO materialDTO)
        {
            return new Material
            {
                Id = materialDTO.Id,
                Name = materialDTO.Name,
                
                Type = materialDTO.Type,
                Quantity = materialDTO.Quantity,
                ProductionDate = materialDTO.ProductionDate,
                ExpirationDate = materialDTO.ExpirationDate
            };
        }
    }
}
