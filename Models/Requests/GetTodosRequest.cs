namespace ToDoApp.Models.Requests
{
    public class GetTodosRequest
    {
        public string UserId { get; set; } 
        public bool IsAdmin { get; set; }
        public string? AdminUserQuery { get; set; }
        public string? SearchQuery { get; set; }
        public string? Filter { get; set; }
        public string? SortOrder { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
