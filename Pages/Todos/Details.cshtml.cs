using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Pages.Todos
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;
        public Todo? Todo { get; set; }

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Todo = await _context.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

            if (Todo == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
