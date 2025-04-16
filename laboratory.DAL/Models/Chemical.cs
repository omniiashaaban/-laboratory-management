using System.ComponentModel.DataAnnotations;

namespace laboratory.DAL.Models
{
    public class Chemical
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public double Quantity { get; set; } // الكمية المتاحة

        [Required]
        public string Unit { get; set; } // مثل: ml, g, kg

        //public ICollection<ExperimentChemical> ExperimentChemicals { get; set; } = new List<ExperimentChemical>();
    }

}
