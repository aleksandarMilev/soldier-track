namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Web.Areas.Administrator.Models.Membership;
    using SoldierTrack.Web.Common.MapTo;

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

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int athleteId)
        {
            var serviceModel = await this.membershipService.GetEditModelByIdAsync(id);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            //again, like in the other services, we map it because the partial view from the form accepts MembershipBaseFormModel or descendant
            var viewModel = serviceModel.MapToEditMembershipServiceModel();
            viewModel.AthleteId = athleteId;

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditMembershipViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var serviceModel = viewModel.MapToEditMembershipViewModel();
            await this.membershipService.EditAsync(serviceModel);

            this.TempData["SuccessMessage"] = MembershipEdited;
            return this.RedirectToAction("Details", "Athlete", new { id = viewModel.AthleteId, area = "" } );
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int athleteId)
        {
            await this.membershipService.DeleteAsync(id);

            this.TempData["SuccessMessage"] = MembershipDeleted;
            return this.RedirectToAction("Details", "Athlete", new { id = athleteId, area = "" });
        }

        [HttpPost]
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
