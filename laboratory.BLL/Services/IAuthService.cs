using laboratory.DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laboratory.BLL.Services
{
    public interface IAuthServices
    {
        Task<string> CreateTokenAsync(AppUser appUSer, UserManager<AppUser> userManager);
    }
}
