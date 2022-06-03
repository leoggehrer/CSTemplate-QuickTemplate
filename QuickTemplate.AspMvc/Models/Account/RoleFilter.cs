//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.AspMvc.Models.Account
{
    public partial class RoleFilter : VersionModel
    {
        public string Designation { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
#endif
//MdEnd