namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Membership;

    using static SoldierTrack.Web.Common.Constants.WebConstants;
    using static SoldierTrack.Web.Common.Constants.MessageConstants;

    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
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

        [Authorize(Roles = AdminRoleName)]
        public async Task<IActionResult> Approve(int id)
        {
            await this.membershipService.ApproveAsync(id);

            this.TempData["SuccessMessage"] = MembershipApproved;
            return this.RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            await this.membershipService.RejectAsync(id);

            this.TempData["SuccessMessage"] = MembershipRejected;
            return this.RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
