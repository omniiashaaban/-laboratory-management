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
        var departments = await _departmentRepository.GetAllAsync();
        var selectedDepartments = departments
            .Where(d => dto.DepartmentIds.Contains(d.Id))
            .ToList();

        if (!selectedDepartments.Any())
        {
            return NotFound(new ApiResponse(404, "Departments not found."));
        }

        var allMaterials = await _materialRepository.GetAllAsync();
        var selectedMaterials = dto.Materials
            .Where(m => allMaterials.Any(am => am.Id == m.MaterialId))
            .ToList();

        if (selectedMaterials.Count != dto.Materials.Count)
        {
            return NotFound(new ApiResponse(404, "One or more materials not found."));
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

        experiment.ExperimentMaterials = selectedMaterials.Select(m => new ExperimentMaterial
        {
            MaterialId = m.MaterialId,
            QuantityRequired = m.QuantityRequired
        }).ToList();

        await _experimentRepository.AddAsync(experiment);

        var response = new ResponseDTO
        {
            Id = experiment.Id,
            Name = experiment.Name,
        };

        return Ok(response);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllExperiments()
    {
        var experiments = await _experimentRepository.GetAllWithIncludeAsync(
            e => e.ExperimentMaterials,
            e => e.Departments
        );

        var result = experiments.Select(e => new ExperimentDTO
        {
            Id = e.Id,
            Name = e.Name,
            Type = e.Type,
            SafetyInstruction = e.SafetyInstruction,
            PdfFilePath = e.PdfFilePath,
            Level = e.Level,
            DepartmentIds = e.Departments.Select(d => d.Id).ToList()
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetExperimentById(int id)
    {
        var experiment = await _experimentRepository.GetByIdWithIncludeAsync(
            e => e.Id == id,
            e => e.ExperimentMaterials,
            e => e.Departments
        );

        if (experiment == null)
        {
            return NotFound(new ApiResponse(404, "Experiment not found."));
        }

        var result = new ExperimentDTO
        {
            Id = experiment.Id,
            Name = experiment.Name,
            Type = experiment.Type,
            SafetyInstruction = experiment.SafetyInstruction,
            PdfFilePath = experiment.PdfFilePath,
            Level = experiment.Level,
            DepartmentIds = experiment.Departments.Select(d => d.Id).ToList()
        };

        return Ok(result);
    }

    // PUT: api/experiment/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExperiment(int id, [FromBody] AddExperimentDTO dto)
    {
        var experiment = await _experimentRepository.GetByIdWithIncludeAsync(
            e => e.Id == id,
            e => e.ExperimentMaterials,
            e => e.Departments
        );

        if (experiment == null)
        {
            return NotFound(new ApiResponse(404, "Experiment not found."));
        }

        var departments = await _departmentRepository.GetAllAsync();
        var selectedDepartments = departments
            .Where(d => dto.DepartmentIds.Contains(d.Id))
            .ToList();

        if (!selectedDepartments.Any())
        {
            return NotFound(new ApiResponse(404, "Departments not found."));
        }

        experiment.Name = dto.Name;
        experiment.Type = dto.Type;
        experiment.SafetyInstruction = dto.SafetyInstruction;
        experiment.Level = dto.Level;
        experiment.Departments = selectedDepartments;

        experiment.ExperimentMaterials = dto.Materials.Select(m => new ExperimentMaterial
        {
            MaterialId = m.MaterialId,
            QuantityRequired = m.QuantityRequired
        }).ToList();

        await _experimentRepository.UpdateAsync(experiment);

        return Ok(new ApiResponse(200, "Experiment updated successfully."));
    }
    // DELETE: api/experiment/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExperiment(int id)
    {
        var experiment = await _experimentRepository.GetByIdAsync(id);
        if (experiment == null)
        {
            return NotFound(new ApiResponse(404, "Experiment not found."));
        }

        await _experimentRepository.DeleteAsync(id);

        return Ok(new ApiResponse(200, "Experiment deleted successfully."));
    }

}