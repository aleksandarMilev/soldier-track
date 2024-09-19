namespace SoldierTrack.Data.Repositories.Base
{
    public interface IRepository<T> 
        where T : class
    {
        IQueryable<T> All();

        IQueryable<T> AllAsNoTracking();

        void Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task SaveChangesAsync();
    }
}
