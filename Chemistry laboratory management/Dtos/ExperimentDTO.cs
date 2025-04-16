namespace Chemistry_laboratory_management.Dtos
{
    public class ExperimentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string SafetyInstruction { get; set; }
        public string PdfFilePath { get; set; }
        public int Level { get; set; }
        public List<int> DepartmentIds { get; internal set; }
    }

    public class AddExperimentDTO
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string SafetyInstruction { get; set; }

        public int Level { get; set; }
        public List<ExperimentMaterialDTO> Materials { get; set; }
        public List<int> DepartmentIds { get; set; }
    }

    public class ExperimentMaterialDTO
    {
        public int MaterialId { get; set; }
        public int QuantityRequired { get; set; }
    }
    public class ResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


}
