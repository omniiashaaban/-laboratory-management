using Chemistry_laboratory_management.Dtos;
using laboratory.DAL.Models;
using laboratory.DAL.Repository;
using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    #region MyRegion
    private readonly GenericRepository<Group> _groupRepository;
    private readonly GenericRepository<Department> _departmentRepository;
    private readonly GenericRepository<Doctor> _doctorRepository;

    public GroupController(GenericRepository<Group> groupRepository,
                           GenericRepository<Department> departmentRepository,
                           GenericRepository<Doctor> doctorRepository)
    {
        _groupRepository = groupRepository;
        _departmentRepository = departmentRepository;
        _doctorRepository = doctorRepository;
    } 
    #endregion


    [Authorize(Roles = "Admin")]    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupDTO>>> GetAllGroups()
    {
        var groups = await _groupRepository.GetAllWithIncludeAsync(
                                                    g => g.Doctor , g => g.Department

                                                              );
        var groupDTOs = groups.Select(group => new getGroupDTO
        {
            Id = group.Id,
            Name = group.Name,
            Level = group.Level,
            DepartmentId = group.DepartmentId,
            DepartmentName = group.Department.Name,
            DoctorId = group.DoctorId,
            DoctorName = group.Doctor.FirstName,
            NumberOfStudent=group.NumberOfStudent
        }).ToList();

        return Ok(groupDTOs);
    }

    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Doctor")]
    [HttpGet("{id}")]
    public async Task<ActionResult<GroupDTO>> GetGroupById(int id)
    {
        var group = await _groupRepository.GetByIdWithIncludeAsync(
                                                 g => g.Id == id,
                                                   g => g.Doctor,
                                                    g => g.Department
                                                                );
        if (group == null)
        {
            return NotFound(new ApiResponse(404, "Group not found."));
        }

        var groupDTO = new getGroupDTO
        {
            Id = group.Id,
            Name = group.Name,
            Level = group.Level,
            DepartmentId = group.DepartmentId,
            DepartmentName = group.Department.Name,
            DoctorId = group.DoctorId,
            DoctorName = group.Doctor.FirstName,
            NumberOfStudent=group.NumberOfStudent
        };

        return Ok(groupDTO);
    }

    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Doctor")]
    [HttpGet("bydoctorId/{doctorId}")]
    public async Task<IActionResult> GetGroupsByDoctor(int doctorId)
    {
        var Groups = await _groupRepository.GetAllWithIncludeAsync(g => g.Department);
        var groups =Groups.Where(g => g.DoctorId == doctorId).Select(g => new 
        {
             g.Id,
             g.Name,
             g.Level,
            g.DepartmentId,
            g.NumberOfStudent,
            DepartmentName = g.Department != null ? g.Department.Name : "No department"


        }).ToList();

        
        if (!groups.Any())
        {
            return NotFound(new ApiResponse(404,  "No groups for this doctor." ));
        }
       


        return Ok(groups);
    }



    [Authorize(Roles = "Admin")]  
    [HttpPost]
    public async Task<ActionResult<GroupDTO>> CreateGroup([FromBody] GroupDTO groupDto)
    {
        var department = await _departmentRepository.GetByIdAsync(groupDto.DepartmentId);
        if (department == null)
        {
            return NotFound(new ApiResponse(404, "Department not found."));
        }

        var doctor = await _doctorRepository.GetByIdAsync(groupDto.DoctorId);
        if (doctor == null)
        {
            return NotFound(new ApiResponse(404, "DoctorName not found."));
        }

        var group = new Group
        {
            Name = groupDto.Name,
            Level = groupDto.Level,
            DepartmentId = groupDto.DepartmentId,
            DoctorId = groupDto.DoctorId,
            NumberOfStudent=groupDto.NumberOfStudent
        };

        await _groupRepository.AddAsync(group);
        var response = new getGroupDTO
        {
            Id = group.Id,
            Name = group.Name,
            Level = group.Level,
            DepartmentId = group.DepartmentId,
            DepartmentName = group.Department.Name,
            DoctorId = group.DoctorId,
            DoctorName = group.Doctor.FirstName,
            NumberOfStudent=group.NumberOfStudent

        };

        return Ok(response);
    }



    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateGroup(int id, [FromBody] GroupDTO groupDto)
    {
        var existingGroup = await _groupRepository.GetByIdAsync(id);
        if (existingGroup == null)
        {
            return NotFound(new ApiResponse(404, "Group not found."));
        }

        var department = await _departmentRepository.GetByIdAsync(groupDto.DepartmentId);
        if (department == null)
        {
            return NotFound(new ApiResponse(404, "Department not found."));
        }

        var doctor = await _doctorRepository.GetByIdAsync(groupDto.DoctorId);
        if (doctor == null)
        {
            return NotFound(new ApiResponse(404, "DoctorName not found."));
        }

        existingGroup.Name = groupDto.Name;
        existingGroup.Level = groupDto.Level;
        existingGroup.DepartmentId = groupDto.DepartmentId;
        existingGroup.DoctorId = groupDto.DoctorId;
        existingGroup.NumberOfStudent = groupDto.NumberOfStudent;

        await _groupRepository.UpdateAsync(existingGroup);

        return Ok(new ApiResponse(200, "Group updated successfully."));
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteGroup(int id)
    {
        var existingGroup = await _groupRepository.GetByIdAsync(id);
        if (existingGroup == null)
        {
            return NotFound(new ApiResponse(404, "Group not found."));
        }

        await _groupRepository.DeleteAsync(id);

        return Ok(new ApiResponse(200, "Group deleted successfully."));
    }
}