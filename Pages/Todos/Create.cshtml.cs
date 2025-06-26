using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ToDoApp.Models;
using ToDoApp.Services.Todos.Interfaces;

namespace ToDoApp.Pages.Todos
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public Todo Todo { get; set; } = new();

        private readonly ITodoCreateService _todoCreateService;

        public CreateModel(ITodoCreateService todoCreateService)
        {
            _todoCreateService = todoCreateService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var created = await _todoCreateService.CreateTodoAsync(Todo, userId);

            TempData["ToastMessage"] = created ? "Todo erstellt" : "Zu viele Todos in der Liste";
            TempData["IsError"] = !created;

            return RedirectToPage("./Index");
        }
    }
}