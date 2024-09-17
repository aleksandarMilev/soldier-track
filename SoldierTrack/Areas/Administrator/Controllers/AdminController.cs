namespace SoldierTrack.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Area("Administrator")]
    public class AdminController : Controller
    {
        public IActionResult Index() => this.View();
    }
}
