using laboratory.DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace laboratory.DAL.Models
{
    public class Student 
    {
        [Key]
        public int Id { get; set; }
       
        [Required]

        public string Name { get; set; }
        public string Email { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }
        public ICollection<Section> Sections { get; set; } = new List<Section>();

        public string ?AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUser? AppUser { get; set; }
    }

}
