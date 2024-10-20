namespace SoldierTrack.Services.Common
{
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Data.Models.Base;

    using static SoldierTrack.Services.Common.Constants;

    public static class Extensions
    {
        public static IQueryable<T> AllDeletable<T>(this ApplicationDbContext data) 
            where T : class, IDeletableEntity
        {
            return data
                .Set<T>()
                .Where(e => !e.IsDeleted);
        }

        public static IQueryable<T> AllDeletableAsNoTracking<T>(this ApplicationDbContext data)
           where T : class, IDeletableEntity
        {
            return data
                .Set<T>()
                .AsNoTracking()
                .Where(e => !e.IsDeleted);
        }

        public static IQueryable<Athlete> AllAthletes(this ApplicationDbContext data)
        {
            return data
               .Set<Athlete>()
               .Where(a => !a.IsDeleted && a.Email != AdminEmail);
        }

        public static IQueryable<Athlete> AllAthletesAsNoTracking(this ApplicationDbContext data)
        { 
             return data
                .Set<Athlete>()
                .AsNoTracking()
                .Where(a => !a.IsDeleted && a.Email != AdminEmail);
        }
    }
}
