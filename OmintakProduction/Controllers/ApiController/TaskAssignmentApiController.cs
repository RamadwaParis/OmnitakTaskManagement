using Microsoft.AspNetCore.Mvc;

namespace OmintakProduction.Controllers.ApiController
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskAssignmentApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllAssignments()
        {
            return Ok(new { message = "List of task assignments" });
        }

        [HttpPost]
        public IActionResult AssignTask([FromBody] object assignment)
        {
            return Ok(new { message = "Task assigned successfully" });
        }
    }
}