using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TaskManager.Application.Interfaces;
using TaskManager.Application.ViewModels;

namespace TaskManagerAPI.Controllers
{
    //Recebe as requisições HTTP e chama a camada de aplicação
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("")]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentAppService _assignmentAppService;

        public AssignmentController(IAssignmentAppService assignmentAppService)
        {
            _assignmentAppService = assignmentAppService;
        }

        [HttpGet("todos")]
        public IActionResult GetAllAssignments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? title = null,
            [FromQuery] string? sort = null,
            [FromQuery] string? order = null)
        {
            List<AssignmentViewModel> assignments;
            assignments = _assignmentAppService.GetAllAssignments(page, pageSize, title, sort, order);
            return assignments.Any() != false ? Ok(assignments) : NotFound();
        }

        [HttpGet("todos/{id}")]
        public IActionResult GetById(int id)
        {
            var assignment = _assignmentAppService.GetAssignment(id);
            return assignment != null ? Ok(assignment) : NotFound();
        }

        [HttpPut("todos/{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, [FromBody] AssignmentViewModel model)
        {
            try
            {
                bool updated = await _assignmentAppService.UpdateAssignmentStatusAsync(id, model.Completed);
                return updated ? Ok() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno: " + ex.Message });
            }
        }


        [HttpPost("sync")]
        public async Task<IActionResult> SyncAssignment()
        {
            await _assignmentAppService.SyncAssignments();
            return Ok();
        }

    }
}
