using laboratory.DAL.Models;
using laboratory.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laboratory.BLL.Services
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    namespace laboratory.BLL.Services
    {
        public class ExperimentService : IExperimentService
        {
            private readonly GenericRepository<Experiment> _experimentRepository;
            private readonly GenericRepository<Material> _materialRepository; // Assuming you have a material repository
            private readonly IWebHostEnvironment _env;

            public ExperimentService(
                GenericRepository<Experiment> experimentRepository,
                GenericRepository<Material> materialRepository,
                IWebHostEnvironment env)
            {
                _experimentRepository = experimentRepository;
                _materialRepository = materialRepository;
                _env = env;
            }

          

            public async Task<bool> UploadPdfAsync(int experimentId, IFormFile file)
            {
                var experiment = await _experimentRepository.GetByIdAsync(experimentId);
                if (experiment == null) return false;

                // Validate that the file is a PDF and not empty
                if (file == null || file.Length == 0 || Path.GetExtension(file.FileName).ToLower() != ".pdf")
                    return false;

                // Generate a unique file name
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var folderPath = Path.Combine(_env.WebRootPath, "Experiments");

                // Create the directory if it doesn't exist
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Define the file path
                var filePath = Path.Combine(folderPath, fileName);

                // Save the file to the disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update the experiment with the file path
                experiment.PdfFilePath = $"/Experiments/{fileName}";
                await _experimentRepository.UpdateAsync(experiment);

                return true;
            }

           
            }
        }
    }

