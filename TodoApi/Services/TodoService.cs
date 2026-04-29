using TodoApi.DTOs;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;
        private readonly ILogger<TodoService> _logger;

        public TodoService(ITodoRepository repository, ILogger<TodoService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

//CreateTodo
        public async Task<TodoResponse> CreateTodoAsync(CreateTodoRequest request)
        {
            _logger.LogInformation("Creating new todo with title: {Title}", request.Title);

            var todo = new Todo
            {
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted
            };

            var created = await _repository.CreateAsync(todo);
            return MapToResponse(created);
        }

        public async Task<IEnumerable<TodoResponse>> GetAllTodosAsync()
        {
            _logger.LogInformation("Retrieving all todos");
            var todos = await _repository.GetAllAsync();
            return todos.Select(MapToResponse);
        }

//GetTodoByID
        public async Task<TodoResponse?> GetTodoByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving todo with id: {Id}", id);
            var todo = await _repository.GetByIdAsync(id);
            return todo == null ? null : MapToResponse(todo);
        }

//UpdateTodo
        public async Task<TodoResponse?> UpdateTodoAsync(int id, UpdateTodoRequest request)
        {
            _logger.LogInformation("Updating todo with id: {Id}", id);

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Todo with id {Id} not found for update", id);
                return null;
            }

            var updatedTodo = new Todo
            {
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted,
                CreatedAt = existing.CreatedAt
            };

            var result = await _repository.UpdateAsync(id, updatedTodo);
            return result == null ? null : MapToResponse(result);
        }

//DeleteTodo
        public async Task<bool> DeleteTodoAsync(int id)
        {
            _logger.LogInformation("Deleting todo with id: {Id}", id);
            return await _repository.DeleteAsync(id);
        }

        private static TodoResponse MapToResponse(Todo todo) => new()
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        };
    }
}
