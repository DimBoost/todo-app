using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ToDoApp.Data;
using ToDoApp.Models;
using ToDoApp.Models.Requests;
using ToDoApp.Services.Todos.Interfaces;

namespace ToDoApp.Pages.Todos
{
    [Authorize] 
    public class TodosModel : PageModel //ProtectedPageModel
    {
        public int PageSize = 5; 
        public int TotalTodosCount { get; set; } = 0;

        // Liste der Todos für die Anzeige im View
        public List<Todo> Todos { get; set; } = new();

        // Optional: Wenn Admin, kann nach User gesucht werden
        [BindProperty(SupportsGet = true)]
        public string? AdminUserQuery { get; set; }

        // Volltextsuche (z.?B. im Titel oder Beschreibung)
        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        // Seiten-Navigation (Pagination)
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 0;

        // Service zur Auslagerung der Business-Logik
        private readonly ITodoService _todoService;
        private readonly ITodoDeleteService _todoDeleteService;

        private readonly ILogger<TodosModel> _logger;

        // Konstruktor mit Dependency Injection für DbContext und Service
        public TodosModel(ITodoService todoService, ITodoDeleteService todoDeleteService, ILogger<TodosModel> logger) 
        {
            _todoService = todoService;
            _todoDeleteService = todoDeleteService; 
            _logger = logger;
        }

        // Wird ausgeführt, wenn die Seite per GET geladen wird (z.B. Initialanzeige, Filter- oder Suchänderung)

        public async Task OnGetAsync(string? sortOrder, string? filter)
        {
           _logger.LogInformation("OnGetAsync gestartet mit sortOrder={SortOrder} und filter={Filter}", sortOrder, filter);

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var isAdmin = User.IsInRole("Admin");

                // Falls kein Benutzer eingeloggt ist (UserId null), leere Todo-Liste anzeigen und Methode beenden
                if (userId == null)
                {
                    Todos = new List<Todo>();
                    return;
                }

                var request = new GetTodosRequest
                {
                    UserId = userId,
                    IsAdmin = isAdmin,
                    AdminUserQuery = AdminUserQuery,
                    SearchQuery = SearchQuery,
                    SortOrder = sortOrder,
                    Filter = filter,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };

                // Holt die Todo-Daten mit den aktuellen Parametern (Sortierung, Filter, Suche, Pagination)
                var result = await _todoService.GetTodosAsync(request);

                // Ergebnis speichern, um es im View anzuzeigen
                Todos = result.Todos;
                TotalTodosCount = result.TotalCount;

                // Falls keine Todos gefunden wurden, eine Meldung setzen (z.B. für Toast)
                if (!Todos.Any())
                {
                    TempData["ToastMessage"] = "Keine Todos gefunden.";
                    TempData["IsError"] = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in OnGetAsync");
                throw; 
            }
        }


        // Handler-Methode, wenn der Benutzer den Status "Erledigt/Unerledigt" eines Todos toggelt
        public async Task<IActionResult> OnPostToggleCompleteAsync(int id, int PageNumber)
        {
            if (id == null) 
                return Page();
 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            // Service-Methode aufrufen, die den Status umschaltet und Zugriffsrechte prüft
            var success = await _todoService.ToggleCompleteAsync(id, userId, isAdmin /*User*/);

            // Bei Erfolg: Seite neu laden, sonst 404 NotFound zurückgeben
            return success ? RedirectToPage(new {PageNumber}) : NotFound();
        }


        // Handler-Methode für das Löschen eines Todos
        public async Task<IActionResult> OnPostDeleteAsync(int id,int PageNumber)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToPage("/Index");
                }

                // IDelete Service 
                var deleted = await _todoDeleteService.DeleteTodoAsync(id, userId, isAdmin);

                // Erfolgsmeldung für den Nutzer setzen (über TempData)
                TempData["ToastMessage"] = deleted ? "Todo gelöscht" : "Todo nicht gefunden";
                TempData["IsError"] = !deleted;

                return RedirectToPage(new { PageNumber });
            }
            catch (UnauthorizedAccessException)
            {
                // Wenn der Nutzer nicht autorisiert ist, zur AccessDenied-Seite umleiten
                return RedirectToPage("/AccessDenied");
            }
        }
    }
}
