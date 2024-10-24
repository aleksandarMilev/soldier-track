namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Web.Areas.Administrator.Controllers.Base;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.WebConstants;

    public class MembershipController : BaseController
    {
        private readonly IMembershipService membershipService;

        public MembershipController(IMembershipService membershipService) => this.membershipService = membershipService;

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string athleteId)
        {
            await this.membershipService.DeleteByIdAsync(id);

            this.TempData["SuccessMessage"] = MembershipDeleted;
            return this.RedirectToAction("Details", "Athlete", new { athleteId, area = AdminRoleName });
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id, string athleteId)
        {
            await this.membershipService.ApproveAsync(id);

            this.TempData["SuccessMessage"] = MembershipApproved;
            return this.RedirectToAction("Details", "Athlete", new { athleteId, area = AdminRoleName });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string athleteId)
        {
            await this.membershipService.RejectAsync(id);

            this.TempData["SuccessMessage"] = MembershipRejected;
            return this.RedirectToAction("Details", "Athlete", new { athleteId, area = AdminRoleName });
        }

        [HttpGet]
        public async Task<IActionResult> GetArchive(string athleteId, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewBag.AthleteId = athleteId;
            var model = await this.membershipService.GetArchiveByAthleteIdAsync(athleteId, pageIndex, pageSize);

            return this.View("~/Views/Membership/GetArchive.cshtml", model);
        }
    }
}
