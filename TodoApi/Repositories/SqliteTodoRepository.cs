using Microsoft.Data.Sqlite;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class SqliteTodoRepository : ITodoRepository
    {
        private readonly string _connectionString;

        public SqliteTodoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=todos.db";
        }

        public async Task<Todo> CreateAsync(Todo todo)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Todos (Title, Description, IsCompleted, CreatedAt, UpdatedAt)
                VALUES ($title, $description, $isCompleted, $createdAt, $updatedAt);
                SELECT last_insert_rowid();
            ";

            var now = DateTime.UtcNow;
            command.Parameters.AddWithValue("$title", todo.Title);
            command.Parameters.AddWithValue("$description", (object?)todo.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("$isCompleted", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("$createdAt", now.ToString("o"));
            command.Parameters.AddWithValue("$updatedAt", DBNull.Value);

            var id = Convert.ToInt32(await command.ExecuteScalarAsync());

            todo.Id = id;
            todo.CreatedAt = now;
            todo.UpdatedAt = null;
            return todo;
        }

        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            var todos = new List<Todo>();
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt, UpdatedAt FROM Todos ORDER BY CreatedAt DESC";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                todos.Add(MapToTodo(reader));
            }

            return todos;
        }

        public async Task<Todo?> GetByIdAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt, UpdatedAt FROM Todos WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToTodo(reader);
            }

            return null;
        }

        public async Task<Todo?> UpdateAsync(int id, Todo todo)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Todos
                SET Title = $title, Description = $description, IsCompleted = $isCompleted, UpdatedAt = $updatedAt
                WHERE Id = $id
            ";

            var now = DateTime.UtcNow;
            command.Parameters.AddWithValue("$title", todo.Title);
            command.Parameters.AddWithValue("$description", (object?)todo.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("$isCompleted", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("$updatedAt", now.ToString("o"));
            command.Parameters.AddWithValue("$id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0) return null;

            todo.Id = id;
            todo.UpdatedAt = now;
            return todo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Todos WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        private static Todo MapToTodo(SqliteDataReader reader)
        {
            return new Todo
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                IsCompleted = reader.GetInt32(3) == 1,
                CreatedAt = DateTime.Parse(reader.GetString(4)),
                UpdatedAt = reader.IsDBNull(5) ? (DateTime?)null : DateTime.Parse(reader.GetString(5))
            };
        }
    }
}
