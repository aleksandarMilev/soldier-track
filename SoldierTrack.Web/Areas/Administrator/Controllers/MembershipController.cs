namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using Base;
    using Microsoft.AspNetCore.Mvc;
    using Services.Membership;

    using static Constants.MessageConstants;
    using static Constants.WebConstants;

    public class MembershipController : BaseAdminController
    {
        private readonly IMembershipService membershipService;

        public MembershipController(IMembershipService membershipService) 
            => this.membershipService = membershipService;

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
        public async Task<IActionResult> GetArchive(string athleteId, int pageIndex = DefaultPageIndex, int pageSize = DefaultPageSize)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewBag.AthleteId = athleteId;
            var model = await this.membershipService.GetArchiveByAthleteIdAsync(athleteId, pageIndex, pageSize);

            return this.View("~/Views/Membership/GetArchive.cshtml", model);
        }
    }
}
