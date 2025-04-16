using System.ComponentModel.DataAnnotations;

namespace Chemistry_laboratory_management.Dtos
{
    public class getDepartmentDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
      
    }
    public class DepartmentDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

    }
}
