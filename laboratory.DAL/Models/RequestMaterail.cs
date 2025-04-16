using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace laboratory.DAL.Models
{
    public class RequestMaterail
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("RequestId")]
        public int RequestId { get; set; }

        public Request Request { get; set; }
        [ForeignKey("MaterialId ")]
        public int MaterialId { get; set; }

        public Material Material { get; set; }

        public double RequestedQuantity { get; set; }
    }
}