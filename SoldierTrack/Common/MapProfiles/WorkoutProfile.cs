namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using SoldierTrack.Services.Workout.Models;
    using SoldierTrack.Web.Areas.Administrator.Models.Workout;

    public class WorkoutProfile : Profile
    {
        public WorkoutProfile()
        {
            this.CreateMap<WorkoutFormModel, WorkoutServiceModel>()
                .ReverseMap();
        }
    }
}
