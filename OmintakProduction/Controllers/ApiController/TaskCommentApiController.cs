using Microsoft.AspNetCore.Mvc;

namespace OmintakProduction.Controllers.ApiController
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskCommentApiController : ControllerBase
    {
        [HttpGet("{taskId}")]
        public IActionResult GetCommentsByTaskId(int taskId)
        {
            return Ok(new { message = $"Comments for Task {taskId}" });
        }

        [HttpPost]
        public IActionResult AddComment([FromBody] object comment)
        {
            return Ok(new { message = "Comment added" });
        }
    }
}