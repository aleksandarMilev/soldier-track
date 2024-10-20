namespace SoldierTrack.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Models;

    public class HomeController : Controller
    {
        public async Task<IActionResult> Index() => this.View();

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            var model = new ErrorViewModel()
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return this.View(model);
        }
    }
}
