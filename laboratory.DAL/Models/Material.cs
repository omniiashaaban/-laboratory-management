using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laboratory.DAL.Models
{
    public class Material
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } // Unique identifier for the material

        [Required, MaxLength(50)]
        public string Type { get; set; } // Material type/category

        [Required]
        public double Quantity { get; set; } // Available stock quantity

        [Required]
        public DateTime ProductionDate { get; set; } // Manufacturing date

        [Required]
        public DateTime ExpirationDate { get; set; } // Expiry date
    }
}
