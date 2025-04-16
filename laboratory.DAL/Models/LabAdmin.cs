using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using laboratory.DAL.Models.Identity;

namespace laboratory.DAL.Models
{
    public class LabAdmin 
    {
        public int Id { get; set; }
        [Required]
        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }


        [Required]
        public string Specialty { get; set; }

        public string Email { get; set; }
        public ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}
