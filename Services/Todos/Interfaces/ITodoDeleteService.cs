using System.Security.Claims;

namespace ToDoApp.Services.Todos.Interfaces
{
    public interface ITodoDeleteService
    {

        Task<bool> DeleteTodoAsync(int id,string userId, bool isAdmin);
    }
}
