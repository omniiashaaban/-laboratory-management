using laboratory.DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laboratory.BLL.Services
{
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace laboratory.BLL.Services
    {
        public interface IExperimentService
        {           
            Task<bool> UploadPdfAsync(int experimentId, IFormFile file);
        }
    }



}
