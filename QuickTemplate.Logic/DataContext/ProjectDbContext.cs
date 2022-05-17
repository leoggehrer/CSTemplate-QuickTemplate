//@BaseCode
//MdStart
using Microsoft.Extensions.Configuration;

namespace QuickTemplate.Logic.DataContext
{
    internal partial class ProjectDbContext : DbContext
    {
        private static readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=QuickTemplateDb;Integrated Security=True";
        static ProjectDbContext()
        {
            BeforeClassInitialize();
            try
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var connectionString = configuration["ConnectionStrings:DefaultConnection"];

                if (string.IsNullOrEmpty(connectionString) == false)
                {
                    ConnectionString = connectionString;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(message: $"Error in {System.Reflection.MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
            }
            AfterClassInitialize();
        }
        static partial void BeforeClassInitialize();
        static partial void AfterClassInitialize();

        public ProjectDbContext()
        {
            Constructing();
            Constructed();
        }
        partial void Constructing();
        partial void Constructed();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var handled = false;

            BeforeOnConfiguring(optionsBuilder, ref handled);
            if (handled == false)
            {
                optionsBuilder.UseSqlServer(ConnectionString);
            }
            AfterOnConfiguring(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }
        static partial void BeforeOnConfiguring(DbContextOptionsBuilder optionsBuilder, ref bool handled);
        static partial void AfterOnConfiguring(DbContextOptionsBuilder optionsBuilder);

        public DbSet<E> GetDbSet<E>() where E : Entities.IdentityEntity
        {
            var handled = false;
            var result = default(DbSet<E>);

            GetDbSet(ref result, ref handled);
            if (handled == false || result == null)
            {
            }
            return result ?? Set<E>();
        }
        partial void GetDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : Entities.IdentityEntity;
    }
}
//MdEnd