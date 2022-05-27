//@BaseCode
//MdStart
#if DEBUG && DEVELOP_ON
using QuickTemplate.Logic.DataContext;

namespace QuickTemplate.Logic.Modules.Database
{
    public static class DbManager
    {
        public static async Task DeleteDatabaseAsync()
        {
            using var dbContext = new ProjectDbContext();

            await dbContext.Database.EnsureDeletedAsync();
        }
        public static async Task MigrateDatabaseAsync()
        {
            using var dbContext = new ProjectDbContext();

            await dbContext.Database.MigrateAsync();
        }
        public static async Task CreateDatabaseAsync()
        {
            using var dbContext = new ProjectDbContext();

            await dbContext.Database.EnsureCreatedAsync();
        }
        public static async Task ExecuteRawSqlAsync(string sql)
        {
            using var dbContext = new ProjectDbContext();

            await dbContext.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
#endif
//MdEnd