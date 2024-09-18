namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Membership;

    public class MembershipController : Controller
    {
        private readonly IMembershipService membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            this.membershipService = membershipService;
        }

        public async Task<IActionResult> GetAllPending()
        {
            var models = await this.membershipService.GetAllPendingAsync();
            return this.View(models);
        }
    }
}
