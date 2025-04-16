using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laboratory.DAL.Models
{
    public class ExperimentMaterial
    {
        public int Id { get; set; }
        public int ExperimentId { get; set; }
        public Experiment Experiment { get; set; }
        public int MaterialId { get; set; } 
        public Material Material { get; set; }

        public double QuantityRequired { get; set; } 
    }



}
