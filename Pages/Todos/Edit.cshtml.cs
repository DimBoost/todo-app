using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Pages.Todos
{
    [Authorize]
    public class EditModel : PageModel
    {
        [BindProperty]
        public Todo Todo { get; set; } = default!;

        private readonly AppDbContext _context;
        public EditModel(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);

                if (todo == null)
                {
                    return NotFound();
                }
                else
                {
                    Todo = todo;
                }

                return Page();
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToPage("/AccessDenied");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingTodo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == Todo.Id);

            try
            {

                if (existingTodo == null) return NotFound();

                // Nur relevante Felder aktualisieren
                existingTodo.Title = Todo.Title;
                existingTodo.Description = Todo.Description;
                existingTodo.IsCompleted = Todo.IsCompleted;

                await _context.SaveChangesAsync();

                TempData["ToastMessage"] = "Todo erfolgreich bearbeitet.";
                return RedirectToPage("./Index");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToPage("/AccessDenied");
            }
        }
    }
}
