using laboratory.DAL.Models;
using laboratory.DAL.Repository;

namespace Chemistry_laboratory_management.Helper
{
    public class MaterialService
    {
        private readonly GenericRepository<Material> _materialRepository;

        public MaterialService(GenericRepository<Material> materialRepository)
        {
            _materialRepository = materialRepository;
        }

        // Check if the material is available in the required quantity
        public async Task<bool> CheckMaterialAvailability(int materialId, double quantityRequired)
        {
            var material = await _materialRepository.GetByIdAsync(materialId);
            if (material == null || material.Quantity < quantityRequired)
            {
                return false;
            }
            return true;
        }

        // Decrease the stock of the material
        public async Task DecreaseMaterialStock(int materialId, int quantityRequired)
        {
            var material = await _materialRepository.GetByIdAsync(materialId);
            if (material != null)
            {
                material.Quantity -= quantityRequired;
                await _materialRepository.UpdateAsync(material);
            }
        }
    }

}
