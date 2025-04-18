﻿using Chemistry_laboratory_management.Dtos;
using laboratory.DAL.Models;
using laboratory.DAL.Repository;
using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    #region MyRegion
    private readonly GenericRepository<Student> _studentRepository;
    private readonly GenericRepository<Department> _departmentRepository;
    private readonly GenericRepository<Group> _groupRepository;

    public StudentController(GenericRepository<Student> studentRepository,
        GenericRepository<Department> departmentRepository,
        GenericRepository<Group> groupRepository)
    {
        _studentRepository = studentRepository;
        _departmentRepository = departmentRepository;
        _groupRepository = groupRepository;
    } 
    #endregion


  
    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Doctor")]

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAllStudents()
    {
        var students = await _studentRepository.GetAllWithIncludeAsync(
                                                   s =>s.Group,
                                                   s => s.Group.Department

                                                             );

        var studentDTOs = students.Select(student => new getStudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            GroupId = student.GroupId,
            GroupName = student.Group.Name,
            Level = student.Group.Level,
            DepartmentName=student.Group.Department.Name
        }).ToList();

        return Ok(studentDTOs);
    }


    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Doctor")]
 
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDto>> GetStudentById(int id)
    {
        var student = await _studentRepository.GetByIdWithIncludeAsync(
                                                 g => g.Id == id,
                                                   g => g.Group,
                                                   s => s.Group.Department
                                                                );
        if (student == null)
        {
            return NotFound();
        }

        var studentDTO = new getStudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            GroupId = student.GroupId,
            GroupName=student.Group.Name,
            Level = student.Group.Level,
            DepartmentName = student.Group.Department.Name

        };

        return Ok(studentDTO);
    }


   
    [Authorize(Roles = "Student")]   
    [HttpPost]
    public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] StudentDto studentDto)
    {
        var existingStudentWithEmail = await _studentRepository.GetAllAsync();
        if (existingStudentWithEmail.Any(s => s.Email == studentDto.Email))
        {
            return BadRequest(new ApiResponse(400, "Email is already in use."));
        }

        var group = await _groupRepository.GetByIdAsync(studentDto.GroupId);
        if (group == null)
        {
            return NotFound(new ApiResponse(404, "Group not found."));
        }

        var student = new Student
        {
            Name = studentDto.Name,
            Email = studentDto.Email,
            GroupId = studentDto.GroupId
        };

        await _studentRepository.AddAsync(student);
        var response = new getStudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            GroupId = student.GroupId
        };

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Student")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateStudent(int id, [FromBody] StudentDto studentDto)
    {
        if (string.IsNullOrWhiteSpace(studentDto.Name) || string.IsNullOrWhiteSpace(studentDto.Email))
        {
            return BadRequest(new ApiResponse(400, "Name and Email are required fields."));
        }

        var existingStudent = await _studentRepository.GetByIdAsync(id);
        if (existingStudent == null)
        {
            return NotFound(new ApiResponse(404, "Student not found."));
        }

        var existingGroup = await _groupRepository.GetByIdAsync(studentDto.GroupId);
        if (existingGroup == null)
        {
            return NotFound(new ApiResponse(404, "Group not found."));
        }

        existingStudent.Name = studentDto.Name;
        existingStudent.Email = studentDto.Email;
        existingStudent.GroupId = studentDto.GroupId;

        await _studentRepository.UpdateAsync(existingStudent);
        return Ok(new ApiResponse(200, "Student updated successfully."));
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteStudent(int id)
    {
        var existingStudent = await _studentRepository.GetByIdAsync(id);
        if (existingStudent == null)
            return NotFound(new ApiResponse(404, "Student not found."));

        await _studentRepository.DeleteAsync(id);

        return Ok(new ApiResponse(200, "Student deleted successfully."));
    }



}
