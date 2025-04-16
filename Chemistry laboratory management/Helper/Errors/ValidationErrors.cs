
namespace LinkDev.Facial_Recognition.BLL.Helper.Errors
{
    public class ValidationErrors : ApiResponse
    {

        public IEnumerable<string> Errors { get; set; }
        public ValidationErrors() : base(400)
        {
            Errors = new List<string>();
        }



    }

}