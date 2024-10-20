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

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string athleteId)
        {
            await this.membershipService.DeleteByIdAsync(id);

            this.TempData["SuccessMessage"] = MembershipDeleted;
            return this.RedirectToAction("Details", "Athlete", new { id = athleteId, area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id, string athleteId)
        {
            await this.membershipService.ApproveAsync(id);

            this.TempData["SuccessMessage"] = MembershipApproved;
            return this.RedirectToAction("Details", "Athlete", new { id = athleteId, area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string athleteId)
        {
            await this.membershipService.RejectAsync(id);

            this.TempData["SuccessMessage"] = MembershipRejected;
            return this.RedirectToAction("Details", "Athlete", new { id = athleteId, area = "" });
        }
    }
}
