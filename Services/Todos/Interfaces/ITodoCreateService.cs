using ToDoApp.Models;

namespace ToDoApp.Services.Todos.Interfaces
{
    public interface ITodoCreateService
    {
        Task<bool> CreateTodoAsync(Todo todo, string userId);
    }
}
