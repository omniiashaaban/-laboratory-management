using Chemistry_laboratory_management.Dtos;
using laboratory.DAL.Data.context;
using laboratory.DAL.Models;
using laboratory.DAL.Repository;
using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chemistry_laboratory_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly GenericRepository<Section> _sectionRepository;
        private readonly GenericRepository<Doctor> _doctorRepository;
        private readonly GenericRepository<Group> _groupRepository;
        public DoctorController(GenericRepository<Doctor> doctorRepositor, GenericRepository<Section> sectionRepository, GenericRepository<Group> groupRepository)
        {
            _doctorRepository = doctorRepositor;
            _sectionRepository = sectionRepository;
            _groupRepository = groupRepository;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctorById(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound(new ApiResponse(404, "DoctorName not found."));
            }

            var doctorDTO = new getDoctorDTO
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Email = doctor.Email,
                //  GroupIds = doctor.Groups.Select(d => d.Id).ToList()
            };

            return Ok(doctorDTO);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetAllDoctors()
        {
            var doctors = await _doctorRepository.GetAllAsync();
            var doctorDTOs = new List<getDoctorDTO>();

            foreach (var doctor in doctors)
            {
                doctorDTOs.Add(new getDoctorDTO
                {
                    Id = doctor.Id,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    Email = doctor.Email,


                });
            }

            return Ok(doctorDTOs);
        }

            [HttpPost]
            public async Task<ActionResult<DoctorDTO>> CreateDoctor([FromBody] DoctorDTO doctorDTO)
            {
                if (doctorDTO == null)
                {
                    return BadRequest("Invalid doctor data.");
                }

                var doctor = new Doctor
                {

                    FirstName = doctorDTO.FirstName,
                    LastName = doctorDTO.LastName,
                    Email = doctorDTO.Email,

                };

                await _doctorRepository.AddAsync(doctor);

            var response = new getDoctorDTO
            {

               Id = doctor.Id,
                FirstName=doctor.FirstName,
                LastName=doctor.LastName,
                Email=doctor.Email

            };


                return Ok(response);
            }


        }
    }