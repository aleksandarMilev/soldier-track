namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using Base;
    using Microsoft.AspNetCore.Mvc;
    using Services.Membership;

    using static Constants.MessageConstants;
    using static Constants.WebConstants;

    public class MembershipController(
        IMembershipService service) : BaseAdminController
    {
        private readonly IMembershipService service = service;

        [HttpPost]
        public async Task<IActionResult> Delete(
            int id,
            string athleteId)
        {
            await this.service.DeleteById(id);
            this.TempData["SuccessMessage"] = MembershipDeleted;

            return this.RedirectToAction(
                "Details",
                "Athlete",
                new { athleteId, area = AdminRoleName });
        }

        [HttpPost]
        public async Task<IActionResult> Approve(
            int id,
            string athleteId)
        {
            await this.service.Approve(id);
            this.TempData["SuccessMessage"] = MembershipApproved;

            return this.RedirectToAction(
                "Details",
                "Athlete",
                new { athleteId, area = AdminRoleName });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(
            int id,
            string athleteId)
        {
            await this.service.Reject(id);
            this.TempData["SuccessMessage"] = MembershipRejected;

            return this.RedirectToAction(
                "Details",
                "Athlete",
                new { athleteId, area = AdminRoleName });
        }

        [HttpGet]
        public async Task<IActionResult> GetArchive(
            string athleteId,
            int pageIndex = DefaultPageIndex,
            int pageSize = DefaultPageSize)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewBag.AthleteId = athleteId;
            var model = await this.service.GetArchiveByAthleteId(
                athleteId,
                pageIndex,
                pageSize);

            return this.View(
                "~/Views/Membership/GetArchive.cshtml",
                model);
        }
    }
}
