//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.Logic.Models.Account
{
    public partial class Role
    {
        public string Designation { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public static Role Create(Entities.Account.Role role)
        {
            var result = new Role();

            result.CopyFrom(role);
            return result;
        }
    }
}
#endif
//MdEnd