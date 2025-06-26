using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDoApp.Data;
using ToDoApp.Models;
using ToDoApp.Models.Requests;
using ToDoApp.Services.Todos.Interfaces;



namespace ToDoApp.Services.Todos
{
    public class TodoService : ITodoService
    {
        private readonly ILogger<TodoService> _logger;
        private readonly AppDbContext _context;
        // Konstruktor: Holt sich den Datenbankkontext per Dependency Injection
        public TodoService(AppDbContext context, ILogger<TodoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Holt eine Liste von Todos mit Gesamtanzahl nach verschiedenen Kriterien (User, Rolle, Filter, Suche, Sortierung, Pagination).
        /// </summary>
        public async Task<(List<Todo> Todos, int TotalCount)> GetTodosAsync(GetTodosRequest request)
        {
            _logger.LogInformation("GetTodosAsync is started for User {UserId}", request.UserId);
            try
            {
                // Basierend auf Rolle (Admin oder User) wird eine unterschiedliche Abfrage erstellt
                var query = BuildBaseQuery(request.UserId, request.IsAdmin, request.AdminUserQuery);

                // Suche, Filter & Sortierung anwenden
                query = ApplyFilter(query, request.Filter);
                query = ApplySearch(query, request.SearchQuery);
                query = ApplySorting(query, request.SortOrder);

                // Gesamtanzahl aller passenden Todos
                var total = await query.CountAsync();

                // Pagination (z. B. Seite 2 = Skip(5), Take(5) → Todos 6–10)
                var todos = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                return (todos, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GetTodosAsync");
                throw;
            }
        }

        // Grundlage der Abfrage: entweder Admin mit User-Suche oder normaler User
        private IQueryable<Todo> BuildBaseQuery(string? userId, bool isAdmin, string? adminQuery)
        {
            if (isAdmin)
            {
                var query = _context.Todos.Include(t => t.User).AsQueryable();

                if (!string.IsNullOrEmpty(adminQuery))
                {
                    query = query.Where(t => t.User.UserName.Contains(adminQuery) || t.User.Email.Contains(adminQuery));
                }

                return query;
            }
            else
            {
                return _context.Todos.Where(t => t.UserId == userId);
            }
        }

        // Filter: z. B. nur offene Todos anzeigen
        private IQueryable<Todo> ApplyFilter(IQueryable<Todo> query, string? filter)
        {
            //return filter == "open" ? query.Where(t => !t.IsCompleted) : query;
            if (string.IsNullOrWhiteSpace(filter))
                return query;

            return filter.ToLower() switch
            {
                "open" => query.Where(t => !t.IsCompleted),
                "completed" => query.Where(t => t.IsCompleted),
                _ => query
            };
        }
        // Suche nach Begriffen im Titel oder in der Beschreibung
        private IQueryable<Todo> ApplySearch(IQueryable<Todo> query, string? searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery)) return query;

            var lowered = searchQuery.ToLower();
            return query.Where(t => t.Title.ToLower().Contains(lowered) || t.Description.ToLower().Contains(lowered));
        }

        // Sortierung basierend auf ausgewähltem Kriterium
        private IQueryable<Todo> ApplySorting(IQueryable<Todo> query, string? sortOrder)
        {
            // Sortiert die Todos nach gewählten Kriterien
            return sortOrder switch
            {
                "title_asc" => query.OrderBy(t => t.Title),
                "title_desc" => query.OrderByDescending(t => t.Title),
                "descript_asc" => query.OrderBy(t => t.Description),
                "descript_desc" => query.OrderByDescending(t => t.Description),
                "date_asc" => query.OrderBy(t => t.CreatedAt),
                "date_desc" => query.OrderByDescending(t => t.CreatedAt),
                "status_asc" => query.OrderBy(t => t.IsCompleted),
                "status_desc" => query.OrderByDescending(t => t.IsCompleted),
                _ => query.OrderByDescending(t => t.CreatedAt)
            };
        }

        /// <summary>
        ///  Toggle(Wecheln/Wechselt) Wechselt den Status "Erledigt" für ein Todo, sofern der User berechtigt ist.
        /// </summary>
        public async Task<bool> ToggleCompleteAsync(int id, string userId, bool isAdmin)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);

            if (todo == null || todo.UserId != userId && !isAdmin) 
                return false;

            todo.IsCompleted = !todo.IsCompleted;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}