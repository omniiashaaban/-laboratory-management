using System;
using System.ComponentModel.DataAnnotations;

namespace laboratory.DAL.DTOs
{
    public class postMaterialDTO
    {
       

        [Required, MaxLength(100)]
        public string Name { get; set; }

       

        [Required, MaxLength(50)]
        public string Type { get; set; }

        [Required]
        public double Quantity { get; set; }

        [Required]
        public DateTime ProductionDate { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }
    }
    public class MaterialDTO
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

       

        [Required, MaxLength(50)]
        public string Type { get; set; }

        [Required]
        public double Quantity { get; set; }

        [Required]
        public DateTime ProductionDate { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }
    }
}
