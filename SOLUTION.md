# Solution Documentation
Candidate Name: LB Roshan
Completion Date: 29-04-2026
---

## Problems Identified

_Describe the issues you found in the original implementation. Consider aspects like:_
- Architecture and design patterns
- Code quality and maintainability
- Security vulnerabilities
- Performance concerns
- Testing gaps

### 1. Critical Security Issue — SQL Injection
### 2. No Dependency Injection — Tight Coupling
### 3. No Repository Pattern — Mixed Concerns
### 4. Non-RESTful API Design
### 5. No Input Validation
### 6. Synchronous Database Calls
### 7. No Global Error Handling — Internal Details Leaked
### 8. Hardcoded Connection String
### 9. Missing `UpdatedAt` Tracking
### 10. Meaningless Tests
### 11. Request/Response Models Mixed with Domain Models



## Architectural Decisions

_Explain the architecture you chose and why. Consider:_

Q: Layered Architecture (Controller → Service → Repository)
HTTP Request
|
TodoController             HTTP concerns: routing, validation, status codes
│  (ITodoService)
TodoService                Business logic: orchestration, mapping, logging
│  (ITodoRepository)
SqliteTodoRepository       Data access: SQL queries, connection management
│
SQLite Database

Q: Design patterns applied
A:
- Repository Pattern
- Dependency Injection
- DTO Pattern
- Middleware Pattern
- Project structure changes
- Technology choices
- Separation of concerns
---

## Trade-offs

_Discuss compromises you made and the reasoning behind them. Consider:_
Q: What did you prioritize?
A:
- Security first
- Testability
- Correctness

Q: What did you defer or simplify?
A:
- No EF Core
- No pagination
- No authentication

Q: What alternatives did you consider?
A:
- Minimal API vs Controllers
- In-memory dictionary vs SQLite

---

## How to Run

### Prerequisites
- [.NET 8.0 SDK] (https://dotnet.microsoft.com/download/dotnet/8.0)

### Build
```bash
dotnet build
```
### Run
```bash
cd TodoApi
dotnet run
```
API starts at: `http://localhost:5164`.  
Swagger UI available at: `http://localhost:5164/swagger`.

### Test
```bash
dotnet test
#detailed o/p
dotnet test --logger "console;verbosity=detailed"
#code cov
dotnet test --collect:"XPlat Code Coverage"
```
---

## API Documentation
### Base URL: http://localhost:5164
### Endpoints

#### Create TODO
POST /api/todos
Content-Type: application/json
{
  "title": "Engineer",       
  "description": "Anna University",  
  "isCompleted": false            
}
Response:
- 201-Created —> body contains the created todo with its assigned id
- 400-Bad Request —> validation failed
```

#### Get TODO 
GET /api/todos
Response:
- 200 OK — array of todo obj
### Get a Single TODO
GET /api/todos/{id}
Response:
- 200 OK — todo obj
- 404 Not Found — no todo with that id
```

#### Update TODO
PUT /api/todos/{id}
Content-Type: application/json
{
  "title": "Updated title",      
  "description": "New details",   
  "isCompleted": true
}
Response:
- 200 OK — the updated todo object
- 400 Bad Request — validation failed
- 404 Not Found — no todo with that id

#### Delete TODO
DELETE /api/todos/{id}
Responses:
- 204 No Content — deleted successfully
- 404 Not Found — no todo with that id
{
  "id": 1,
  "title": "Buy groceries",
  "description": "Milk and eggs",
  "isCompleted": false,
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": null
}
---

## Future Improvements

_What would you do if you had more time? Consider:_
- Additional features
- Performance optimizations
- Enhanced testing
- Better documentation
- Deployment considerations

### Pagination
`GET /api/todos` currently returns all records. With a large dataset this becomes
a performance problem. The endpoint should accept `?page=1&pageSize=20` query
parameters and return a paginated response with total count.

### Filtering and Sorting
Add `?isCompleted=true` and `?sort=createdAt:desc` query parameters so clients can retrieve filtered/sorted views without fetching everything.

### Authentication & Authorization
All endpoints currently have no auth. In a real application, JWT bearer tokens would be required. User-scoped todos (each user only sees their own records) need a `UserId` column and filtering at the repository level.

### Migrate to EF Core
Replace `SqliteTodoRepository` with an EF Core `DbContext`. This gives migrations, change tracking, LINQ queries, and a much simpler repository implementation with zero changes to the service layer.

### Structured Logging with Serilog
Replace the default logger with Serilog for structured JSON logs, correlation IDs, and sink configuration (e.g. to Seq or CloudWatch).

### Docker Support
Add a `Dockerfile` and `docker-compose.yml` so the API can be run reproducibly in any environment with a single command.

### CI/CD Pipeline
Add a GitHub Actions workflow that runs `dotnet build` and `dotnet test` on every push and pull request, with coverage reports published automatically.