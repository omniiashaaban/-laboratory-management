using Chemistry_laboratory_management.Dtos;
using laboratory.DAL.Models;
using laboratory.DAL.Repository;
using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Chemistry_laboratory_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly GenericRepository<Section> _sectionRepository;
        private readonly GenericRepository<Doctor> _doctorRepository;
        private readonly GenericRepository<Experiment> _experimentRepository;
        private readonly GenericRepository<Student> _studentRepository;
        private readonly GenericRepository<Group> _groupRepository;
        private readonly GenericRepository<Material> _materialRepository;


        public SectionController(
            GenericRepository<Section> sectionRepository,
            GenericRepository<Doctor> doctorRepository,
            GenericRepository<Experiment> experimentRepository,
            GenericRepository<Student> studentRepository,
            GenericRepository<Group> groupRepository,
            GenericRepository<Material> materialRepository
            )

        {
            _sectionRepository = sectionRepository;
            _doctorRepository = doctorRepository;
            _experimentRepository = experimentRepository;
            _studentRepository = studentRepository;
            _groupRepository = groupRepository;
            _materialRepository = materialRepository;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllSections()
        {
            var sections = await _sectionRepository.GetAllWithIncludeAsync(
                                                             g => g.Doctor,
                                                             g => g.Group,
                                                             g => g.Experiment
                                                              );


            var sectionDTOs = sections.Select(section => new sectionDTO
            {
                Id = section.Id,
                DoctorId = section.DoctorId,
                DoctorName = section.Doctor.FirstName,
                ExperimentId = section.ExperimentId,
                ExperimentName = section.Experiment.Name,
                GroupId = section.GroupId,
                GroupName = section.Group.Name,
                status = section.status

            }).ToList();

            return Ok(sectionDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSectionById(int id)
        {
            var section = await _sectionRepository.GetByIdWithIncludeAsync(
                                                 g => g.Id == id,
                                                   g => g.Doctor,
                                                             g => g.Group,
                                                             g => g.Experiment
                                                                );

            if (section == null)
                return NotFound(new ApiResponse(404, "Section not found."));

            var sectionDTO = new sectionDTO
            {
                Id = section.Id,
                DoctorId = section.DoctorId,
                DoctorName = section.Doctor.FirstName,
                ExperimentId = section.ExperimentId,
                ExperimentName = section.Experiment.Name,
                GroupId = section.GroupId,
                GroupName = section.Group.Name,
                status = section.status

            };

            return Ok(sectionDTO);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSection([FromBody] requestsectionDTO sectionDTO)
        {
            if (sectionDTO == null)
                return BadRequest(new ApiResponse(400, "Invalid request data."));

            var doctorExists = (await _doctorRepository.GetAllAsync()).Any(d => d.Id == sectionDTO.DoctorId);
            if (!doctorExists)
                return NotFound(new ApiResponse(404, "Doctor not found."));

            var experimentExists = (await _experimentRepository.GetAllAsync()).Any(e => e.Id == sectionDTO.ExperimentId);
            if (!experimentExists)
                return NotFound(new ApiResponse(404, "Experiment not found."));

            var groupsExists = (await _groupRepository.GetAllAsync()).Any(d => d.Id == sectionDTO.GroupId);
            if (!groupsExists)
                return NotFound(new ApiResponse(404, "Group not found."));

            var section = new Section
            {
                DoctorId = sectionDTO.DoctorId,
                GroupId = sectionDTO.GroupId,
                ExperimentId = sectionDTO.ExperimentId,
                status = "Pending"
            };

            await _sectionRepository.AddAsync(section);

            var createdSection = new sectionDTO
            {
                Id = section.Id,
                status = section.status,
                DoctorId = section.DoctorId,
                ExperimentId = section.Experiment.Id,
                GroupName = section.Group.Name,
                DoctorName = section.Doctor.FirstName,
                ExperimentName = section.Experiment.Name,
                GroupId = section.GroupId
            };


            return Ok(createdSection);
        }



        [HttpPost("generate-code/{sectionId}")]
        public async Task<IActionResult> GenerateAttendanceCode(int sectionId)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return NotFound(new ApiResponse(404, "Section not found."));

            section.AttendanceCode = new Random().Next(100000, 999999).ToString();
            section.CodeExpiry = DateTime.UtcNow.AddHours(2).AddSeconds(10);

            // Ensure AttendanceJson is updated with the latest AttendanceRecords
            section.AttendanceJson = JsonConvert.SerializeObject(section.AttendanceRecords);

            await _sectionRepository.UpdateAsync(section);

            return Ok(new { Code = section.AttendanceCode, Expiry = section.CodeExpiry });
        }

        [HttpPost("{sectionId}/attendance/{studentId}")]
        public async Task<IActionResult> MarkAttendance(int sectionId, int studentId, [FromBody] string attendanceCode)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return NotFound(new ApiResponse(404, "Section not found."));

            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                return NotFound(new ApiResponse(404, "Student not found."));

            if (section.AttendanceCode != attendanceCode)
                return BadRequest(new ApiResponse(400, "Invalid attendance code."));

            if (DateTime.UtcNow > section.CodeExpiry)
                return BadRequest(new ApiResponse(400, "Attendance code has expired."));

            if (student.GroupId != section.GroupId)
                return BadRequest(new ApiResponse(400, "Student is not in the same group as the section."));

            var attendance = section.AttendanceRecords;

            if (attendance.ContainsKey(studentId))
                return BadRequest(new ApiResponse(400, "Attendance already recorded for this student."));

            if (student.GroupId != section.GroupId)
                return BadRequest(new ApiResponse(400, "Student is not in the same group as the section."));

            // تسجيل الحضور
            attendance[studentId] = true;

            // تحديث JSON
            section.AttendanceRecords = attendance;

            await _sectionRepository.UpdateAsync(section);

            return Ok(new ApiResponse(200, "Attendance recorded successfully."));
        }


        [HttpPatch("{sectionId}/evaluate")]
        public async Task<IActionResult> EvaluateSection(int sectionId)
        {
            var section = await _sectionRepository.GetByIdWithIncludeAsync(
                s => s.Id == sectionId,
                s => s.Group.Students,
                s => s.Experiment.ExperimentMaterials
            );

            if (section == null)
                return NotFound(new ApiResponse(404, "Section not found."));

            int studentCount = section.Group.Students.Count;
            bool allMaterialsSufficient = true;
            var materialUsage = new List<(Material material, double requiredQuantity)>();

            foreach (var expMaterial in section.Experiment.ExperimentMaterials)
            {
                var material = await _materialRepository.GetByIdAsync(expMaterial.MaterialId);
                if (material == null)
                    return NotFound(new ApiResponse(404, $"Material with ID {expMaterial.MaterialId} not found."));

                double requiredQuantity = expMaterial.QuantityRequired * studentCount;

                if (material.Quantity < requiredQuantity)
                {
                    allMaterialsSufficient = false;
                    break;
                }

                materialUsage.Add((material, requiredQuantity));
            }

            string newStatus = allMaterialsSufficient ? "Accepted" : "Rejected";
            section.status = newStatus;

            if (allMaterialsSufficient)
            {
                foreach (var (material, requiredQty) in materialUsage)
                {
                    material.Quantity -= requiredQty;
                    await _materialRepository.UpdateAsync(material);
                }
            }

            await _sectionRepository.UpdateAsync(section);

            return Ok(new
            {
                SectionId = section.Id,
                Status = newStatus,
                Message = allMaterialsSufficient
                    ? "Section accepted. Materials deducted successfully."
                    : "Section rejected. Not enough materials in stock."
            });
        }
        [HttpGet("{sectionId}/attendance")]
        public async Task<IActionResult> GetAttendance(int sectionId)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return NotFound(new ApiResponse(404, "Section not found."));

            if (string.IsNullOrWhiteSpace(section.AttendanceJson))
                return Ok(new List<object>());

           
            try
            {
                var records = JsonConvert.DeserializeObject<Dictionary<int, bool>>(section.AttendanceJson);

                if (records == null || records.Count == 0)
                    return Ok(new List<object>());

                var attendanceList = records
                    .Select(a => new { StudentId = a.Key, IsPresent = a.Value })
                    .ToList();

                return Ok(attendanceList);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(500, "Error parsing attendance JSON: " + ex.Message));
            }
        }
        [HttpGet("accepted")]
        public async Task<IActionResult> GetAcceptedSections()
        {
            var acceptedSections = await _sectionRepository.GetAllWithIncludeAsync(
                s => s.status == "Accepted",
                s => s.Doctor,
                s => s.Group,
                s => s.Experiment
            );

            var sectionDTOs = acceptedSections.Select(section => new sectionDTO
            {
                Id = section.Id,
                DoctorId = section.DoctorId,
                DoctorName = section.Doctor.FirstName,
                ExperimentId = section.ExperimentId,
                ExperimentName = section.Experiment.Name,
                GroupId = section.GroupId,
                GroupName = section.Group.Name,
                status = section.status
            }).ToList();

            return Ok(sectionDTOs);
        }

        [HttpGet("rejected")]
        public async Task<IActionResult> GetRejectedSections()
        {
            var rejectedSections = await _sectionRepository.GetAllWithIncludeAsync(
                s => s.status == "Rejected",
                s => s.Doctor,
                s => s.Group,
                s => s.Experiment
            );

            var sectionDTOs = rejectedSections.Select(section => new sectionDTO
            {
                Id = section.Id,
                DoctorId = section.DoctorId,
                DoctorName = section.Doctor.FirstName,
                ExperimentId = section.ExperimentId,
                ExperimentName = section.Experiment.Name,
                GroupId = section.GroupId,
                GroupName = section.Group.Name,
                status = section.status
            }).ToList();

            return Ok(sectionDTOs);
        }
    }
}