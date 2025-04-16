namespace LinkDev.Facial_Recognition.BLL.Helper.Errors
{
    public class ApiResponse
    {
        public int StutusCode { get; set; }
        public string? Message { get; set; }

        public ApiResponse(int statuseCode, string message = null, object data = null)
        {
            StutusCode = statuseCode;
            Message = message ?? GetDefaultMessageForStatusCode(statuseCode);
        }

        private string GetDefaultMessageForStatusCode(int statuseCode)
        {
            return statuseCode switch
            {
                200 => "Request completed successfully.",
                400 => "Bad Request, the request was invalid.",
                401 => "Unauthorized, you are not authorized to access this resource.",
                404 => "Resource not found.",
                500 => "Internal server error. Please try again later.",
                _ => "An unexpected error occurred."
            };
        }
    }
}