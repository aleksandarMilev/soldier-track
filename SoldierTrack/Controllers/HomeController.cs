namespace SoldierTrack.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Models;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Web.Common.Extensions;

    public class HomeController : Controller
    {
        private readonly IAthleteService athleteService;

        public HomeController(IAthleteService athleteService) => this.athleteService = athleteService;

        public async Task<IActionResult> Index()
        {
            if (this.User?.Identity?.IsAuthenticated ?? false)
            {
                this.ViewBag.AthleteId = await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!);
            }

            return this.View();
        }

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
