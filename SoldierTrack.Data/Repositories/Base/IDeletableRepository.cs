namespace SoldierTrack.Data.Repositories.Base
{
    using SoldierTrack.Data.Models.Base;

    public interface IDeletableRepository<T> : IRepository<T>
        where T : class, IDeletableEntity
    {
        public IQueryable<T> AllWithDeleted();

        public IQueryable<T> AllWithDeletedAsNoTracking();

        public void SoftDelete(T entity);

        public void Restore(T entity);
    }
}
