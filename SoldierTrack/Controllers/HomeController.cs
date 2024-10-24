namespace SoldierTrack.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Web.Controllers.Base;

    [AllowAnonymous]
    public class HomeController : BaseController
    {
        public IActionResult Index() => this.View();

        public IActionResult FAQ() => this.View();

        public IActionResult Error400() => this.View();

        public IActionResult Error401() => this.View();

        public IActionResult Error404() => this.View();

        public IActionResult Error500() => this.View();
    }
}
