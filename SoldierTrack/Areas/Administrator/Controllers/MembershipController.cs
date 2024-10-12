namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Membership;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.WebConstants;

    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
    public class MembershipController : Controller
    {
        private readonly IMembershipService membershipService;

        public MembershipController(IMembershipService membershipService) => this.membershipService = membershipService;

        [HttpGet]
        public async Task<IActionResult> GetAllPending()
        {
            var models = await this.membershipService.GetAllPendingAsync();
            return this.View(models);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int athleteId)
        {
            await this.membershipService.DeleteByIdAsync(id);

            this.TempData["SuccessMessage"] = MembershipDeleted;
            return this.RedirectToAction("Details", "Athlete", new { id = athleteId, area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int membershipId, int athleteId)
        {
            await this.membershipService.ApproveAsync(membershipId);

            this.TempData["SuccessMessage"] = MembershipApproved;
            return this.RedirectToAction("Details", "Athlete", new { id = athleteId, area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int membershipId, int athleteId)
        {
            await this.membershipService.RejectAsync(membershipId);

            this.TempData["SuccessMessage"] = MembershipRejected;
            return this.RedirectToAction("Details", "Athlete", new { id = athleteId, area = "" });
        }
    }
}
