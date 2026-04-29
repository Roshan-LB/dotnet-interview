using TodoApi.DTOs;

namespace TodoApi.Services
{
    public interface ITodoService
    {
        Task<TodoResponse> CreateTodoAsync(CreateTodoRequest request);
        Task<IEnumerable<TodoResponse>> GetAllTodosAsync();
        Task<TodoResponse> GetTodoByIdAsync(int id);
        Task<TodoResponse> UpdateTodoAsync(int id, UpdateTodoRequest request);
        Task<bool> DeleteTodoAsync(int id);
    }
}
