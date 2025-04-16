using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using laboratory.DAL.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics.CodeAnalysis;

namespace laboratory.DAL.Models
{
    public class Section
    {
        [Key]
        public int Id { get; set; }
        public string? AttendanceCode { get; set; }
        public DateTime? CodeExpiry { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int ExperimentId { get; set; }
        public Experiment Experiment { get; set; }

        public String status { get; set; } //rejected pending accepted


        //public ICollection<RequestMaterail> RequestChemicals { get; set; } = new List<RequestMaterail>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
      
        public string AttendanceJson { get; set; } = "{}";
        [NotMapped]
        public Dictionary<int, bool> AttendanceRecords
        {
            get => string.IsNullOrEmpty(AttendanceJson)
                ? new Dictionary<int, bool>()
                : JsonConvert.DeserializeObject<Dictionary<int, bool>>(AttendanceJson);

            set => AttendanceJson = JsonConvert.SerializeObject(value);
        }


    }
}

