using Microsoft.AspNetCore.Mvc;

namespace ToDoApp.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        /// <summary>
        /// Übergibt Razor-View zum Rendern der UI
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(); 
        }
    }
}
