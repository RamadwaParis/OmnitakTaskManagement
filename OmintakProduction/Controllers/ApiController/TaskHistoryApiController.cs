using Microsoft.AspNetCore.Mvc;

namespace OmintakProduction.Controllers.ApiController
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskHistoryApiController : ControllerBase
    {
        [HttpGet("{taskId}")]
        public IActionResult GetTaskHistory(int taskId)
        {
            return Ok(new { message = $"History for Task {taskId}" });
        }
    }
}