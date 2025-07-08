using Microsoft.AspNetCore.Mvc;

namespace OmintakProduction.Controllers.ApiController
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllTasks()
        {
            return Ok(new { message = "List of tasks" });
        }

        [HttpGet("{id}")]
        public IActionResult GetTaskById(int id)
        {
            return Ok(new { message = $"Task with ID {id}" });
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] object task)
        {
            return CreatedAtAction(nameof(GetTaskById), new { id = 1 }, task);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] object task)
        {
            return Ok(new { message = $"Task {id} updated" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            return Ok(new { message = $"Task {id} deleted" });
        }
    }
}