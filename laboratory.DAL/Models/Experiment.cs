using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace laboratory.DAL.Models
{
    public class Experiment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string? SafetyInstruction { get; set; }
        public int Level { get; set; }
        public string PdfFilePath { get; set; } = "";

        public ICollection<ExperimentMaterial> ExperimentMaterials { get; set; } = new List<ExperimentMaterial>();

        public ICollection<Department> Departments { get; set; } = new HashSet<Department>();
        
        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
