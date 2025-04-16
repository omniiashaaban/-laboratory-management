using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laboratory.DAL.Models
{
    public class StudentRequest
    {
        [Key]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected

        [ForeignKey("StudentId")]
        public Student Student { get; set; }

    }
}