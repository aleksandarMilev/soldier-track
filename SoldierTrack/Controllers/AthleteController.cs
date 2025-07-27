namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Controllers.Base;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Athlete;
    using Services.Athlete;
    using Services.Athlete.Models;
    using Web.Extensions;

    using static Constants.MessageConstants;

    public class AthleteController : BaseController
    {
        private readonly IAthleteService athleteService;
        private readonly SignInManager<Athlete> signInManager;
        private readonly IMapper mapper;

        public AthleteController(
            IAthleteService athleteService,
            SignInManager<Athlete> signInManager,
            IMapper mapper)
        {
            this.athleteService = athleteService;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var model = await this.athleteService.GetDetailsModelByIdAsync(this.User.GetId()!);

            if (model == null)
            {
                return this.NotFound();
            }

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var serviceModel = await this.athleteService.GetModelByIdAsync(this.User.GetId()!);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            var viewModel = this.mapper.Map<AthleteFormModel>(serviceModel);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AthleteFormModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            if (await this.athleteService.AthleteWithSameNumberExistsAsync(viewModel.PhoneNumber, viewModel.Id))
            {
                this.ModelState.AddModelError(nameof(viewModel.PhoneNumber), string.Format(PhoneDuplicate, viewModel.PhoneNumber));

                return this.View(viewModel);
            }

            if (await this.athleteService.AthleteWithSameEmailExistsAsync(viewModel.Email, viewModel.Id))
            {
                this.ModelState.AddModelError(nameof(viewModel.Email), string.Format(EmailDuplicate, viewModel.Email));

                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<AthleteServiceModel>(viewModel);
            await this.athleteService.EditAsync(serviceModel);

            this.TempData["SuccessMessage"] = AthleteEditHimself;

            return this.RedirectToAction(nameof(Details), new { id = serviceModel.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            await this.signInManager.SignOutAsync();
            await this.athleteService.DeleteAsync(this.User.GetId()!);

            this.TempData["SuccessMessage"] = AthleteDeleteHimself;

            return this.RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Join(int workoutId)
        {
            await this.athleteService.JoinAsync(this.User.GetId()!, workoutId);

            this.TempData["SuccessMessage"] = JoinSuccess;

            return this.RedirectToAction("Details", "Workout", new { id = workoutId });
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int workoutId)
        {
            await this.athleteService.LeaveAsync(this.User.GetId()!, workoutId);

            this.TempData["SuccessMessage"] = AthleteLeaveSuccess;

            return this.RedirectToAction("Details", "Workout", new { id = workoutId });
        }
    }
}
