using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Threading.Tasks;

public class ApiExpetionResponse : ApiResponse
{

    public string? Detailes { get; set; }
    public ApiExpetionResponse(int statuseCode, string message = null, string? detailes = null) : base(statuseCode, message)
    {
        Detailes = detailes;
    }



}