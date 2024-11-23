namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using Services.Workout.Models;
    using Areas.Administrator.Models.Workout;

    public class WorkoutProfile : Profile
    {
        public WorkoutProfile()
        {
            this.CreateMap<WorkoutFormModel, WorkoutServiceModel>()
                .ReverseMap();
        }
    }
}
