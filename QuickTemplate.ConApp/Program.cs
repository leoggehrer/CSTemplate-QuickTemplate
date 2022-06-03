//@BaseCode
//MdStart
namespace QuickTemplate.ConApp
{
    public partial class Program
    {
        #region Class-Constructors
        static Program()
        {
            ClassConstructing();
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();
        #endregion Class-Constructors
        public static void Main(string[] args)
        {
            Console.WriteLine(nameof(QuickTemplate));
            Console.WriteLine(DateTime.Now);
            BeforeRun();
#if DEBUG && DEVELOP_ON
            Task.Run(async () =>
            {
                await Logic.Modules.Database.DbManager.DeleteDatabaseAsync();
                await Logic.Modules.Database.DbManager.CreateDatabaseAsync();
            }).Wait();
#endif
#if ACCOUNT_ON
            CreateAccount();
#endif
            CreateImport();
            AfterRun();
            Console.WriteLine(DateTime.Now);
        }
        static partial void BeforeRun();
        static partial void AfterRun();
#if ACCOUNT_ON
        static partial void CreateAccount();
#endif
        static partial void CreateImport();
    }
}
//MdEnd