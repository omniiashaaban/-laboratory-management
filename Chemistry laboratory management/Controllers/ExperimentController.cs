using Chemistry_laboratory_management.Dtos;
using laboratory.DAL.Models;
using laboratory.DAL.Repository;
using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ExperimentController : ControllerBase
{
    private readonly GenericRepository<Experiment> _experimentRepository;
    private readonly GenericRepository<Department> _departmentRepository;
    private readonly GenericRepository<Material> _materialRepository;

    public ExperimentController(
        GenericRepository<Experiment> experimentRepository,
        GenericRepository<Department> departmentRepository,
        GenericRepository<Material> materialRepository)
    {
        _experimentRepository = experimentRepository;
        _departmentRepository = departmentRepository;
        _materialRepository = materialRepository;
    }

    [HttpPost]
    public async Task<IActionResult> AddExperiment([FromBody] AddExperimentDTO dto)
    {
        // التأكد من الأقسام
        var departments = await _departmentRepository.GetAllAsync();
        var selectedDepartments = departments
            .Where(d => dto.DepartmentIds.Contains(d.Id))
            .ToList();

        if (!selectedDepartments.Any())
        {
            return NotFound(new ApiResponse(404, "Departments not found."));
        }

      
        var experiment = new Experiment
        {
            Name = dto.Name,
            Type = dto.Type,
            SafetyInstruction = dto.SafetyInstruction,
            Level = dto.Level,
            PdfFilePath = "", 
            Departments = selectedDepartments
        };

        experiment.ExperimentMaterials = dto.Materials.Select(m => new ExperimentMaterial
        {
            MaterialId = m.MaterialId,
            QuantityRequired = m.QuantityRequired
        }).ToList();

        await _experimentRepository.AddAsync(experiment);

        var response = new ExperimentDTO
        {
            Id = experiment.Id,
            Name = experiment.Name,          
        };

        return Ok(response);
    }
}