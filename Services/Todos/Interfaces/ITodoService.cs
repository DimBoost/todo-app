using System.Security.Claims;
using ToDoApp.Models.Requests;
using ToDoApp.Models;

namespace ToDoApp.Services.Todos.Interfaces
{
    public interface ITodoService
    {
        Task<(List<Todo> Todos, int TotalCount)> GetTodosAsync(GetTodosRequest request);

        Task<bool> ToggleCompleteAsync(int id, string userId, bool isAdmin);
    }
}
