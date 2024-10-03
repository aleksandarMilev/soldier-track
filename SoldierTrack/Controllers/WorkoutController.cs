namespace SoldierTrack.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Workout;
    using SoldierTrack.Services.Workout.Models;
    using SoldierTrack.Web.Common.Extensions;

    using static SoldierTrack.Web.Common.Constants.WebConstants;

    public class WorkoutController : Controller
    {
        private readonly IWorkoutService workoutService;
        private readonly IAthleteService athleteService;
        private readonly IMembershipService membershipService;

        public WorkoutController(
            IWorkoutService workoutService,
            IAthleteService athleteService,
            IMembershipService membershipService)
        {
            this.workoutService = workoutService;
            this.athleteService = athleteService;
            this.membershipService = membershipService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(DateTime? date = null, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var model = await this.workoutService.GetAllAsync(date, pageIndex, pageSize);
            return this.View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var model = await this.workoutService.GetDetailsByIdAsync(id);

            if (model == null)
            {
                return this.NotFound();
            }

            if (await this.athleteService.UserIsAthleteByUserIdAsync(this.User.GetId()!))
            {
                await this.SetViewModelButtons(id, model);
            }

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetArchive(int athleteId, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var currentAthleteId = await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!);
            if (currentAthleteId != athleteId)
            {
                return this.Unauthorized();
            }

            this.ViewBag.AthleteId = athleteId;
            var model = await this.workoutService.GetArchiveByAthleteIdAsync(athleteId, pageIndex, pageSize);
            return this.View(model);
        }

        private async Task SetViewModelButtons(int workoutId, WorkoutDetailsServiceModel model)
        {
            var athleteId = await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!);

            if (!await this.athleteService.AthleteHasMembershipByAthleteIdAsync(athleteId.Value))
            {
                return;
            }

            this.ViewBag.AthleteId = athleteId.Value;

            var athleteHasApprovedMembership = await this.athleteService.AthleteHasApprovedMembershipByAthleteIdAsync(athleteId.Value);
            var athleteMembershipIsExpried = await this.athleteService.AthleteMembershipIsExpiredByIdAsync(athleteId.Value);

            if (athleteMembershipIsExpried)
            {
                await this.membershipService.DeleteByAthleteIdAsync(athleteId.Value);
            }

            if (athleteHasApprovedMembership && !athleteMembershipIsExpried)
            {
                model.ShowJoinButton = true;
            }

            var athleteAlreadyJoined = await this.athleteService.AthleteAlreadyJoinedByIdAsync(athleteId.Value, workoutId);

            if (athleteAlreadyJoined)
            {
                model.ShowJoinButton = false;
                model.ShowLeaveButton = true;
            }

            var athleteHasMembership = await this.athleteService.AthleteHasMembershipByAthleteIdAsync(athleteId.Value);
            model.AthleteHasMembership = athleteHasMembership;
        }
    }
}
