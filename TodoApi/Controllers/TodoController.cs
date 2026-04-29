using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/todos")]
    [Produces("application/json")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        // Retrieve all TODO items
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TodoResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var todos = await _todoService.GetAllTodosAsync();
            return Ok(todos);
        }

        // Retrieves a specific TODO item by its ID
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
                return NotFound(new { message = $"Todo with id {id} was not found." });

            return Ok(todo);
        }

        // Creates a new TODO item
        [HttpPost]
        [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTodoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _todoService.CreateTodoAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // Updates an existing TODO item by its ID
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _todoService.UpdateTodoAsync(id, request);
            if (updated == null)
                return NotFound(new { message = $"Todo with id {id} was not found." });

            return Ok(updated);
        }

        // Deletes a TODO item by its ID
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _todoService.DeleteTodoAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Todo with id {id} was not found." });

            return NoContent();
        }
    }    
}
