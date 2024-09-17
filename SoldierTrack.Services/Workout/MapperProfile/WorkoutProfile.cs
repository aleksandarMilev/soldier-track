namespace SoldierTrack.Services.Workout.MapperProfile
{
    using AutoMapper;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Workout.Models;

    public class WorkoutProfile : Profile
    {
        public WorkoutProfile()
        {
            this.CreateMap<WorkoutServiceModel, Workout>();
            this.CreateMap<Workout, WorkoutIdServiceModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName.Name));
        }
    }
}
