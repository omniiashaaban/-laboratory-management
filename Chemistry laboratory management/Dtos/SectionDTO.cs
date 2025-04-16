using laboratory.DAL.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chemistry_laboratory_management.Dtos
{
    public class sectionDTO
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public String DoctorName { get; set; }
       
        public int GroupId { get; set; }
        public String GroupName { get; set; }
        public int ExperimentId { get; set; }
        public String ExperimentName { get; set; }

        public string status { get; set; }

    }
    public class requestsectionDTO
    {
        public int DoctorId { get; set; }
        public int ExperimentId { get; set; }
        public int GroupId { get; set; }
        public string status { get; set; }
    }
    public class SectionEvaluationDTO
    {
        public int DoctorId { get; set; }
        public int GroupId { get; set; }
        public int ExperimentId { get; set; }
    }
}
