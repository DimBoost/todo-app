using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models
{
    public class Todo
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Titel ist Pflichtfeld")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Wird serverseitig aus den User-Claims gesetzt (nicht vom Client geliefert).
        // Daher: [BindNever] verhindert Binding, [ValidateNever] verhindert Validierungsfehler bei leerem Wert.
        [BindNever]
        [ValidateNever]
        public string UserId { get; set; } = default!;
        public ApplicationUser? User { get; set; }
    }
}
