namespace SoldierTrack.Tests.Services
{
    using Data;
    using Microsoft.EntityFrameworkCore;

    public class TestDatabaseFixture : IDisposable
    {
        private readonly DbContextOptions<ApplicationDbContext> options;

        public TestDatabaseFixture()
        {
            this.options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            this.Data = new ApplicationDbContext(this.options);
        }

        public ApplicationDbContext Data { get; private set; }

        public void ResetDatabase()
        {
            foreach (var entry in this.Data.ChangeTracker.Entries().ToList())
            {
                entry.State = EntityState.Detached;
            }

            this.Data.Database.EnsureDeleted();
            this.Data.Database.EnsureCreated();
        }

        public void Dispose()
        {
            this.Data?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
