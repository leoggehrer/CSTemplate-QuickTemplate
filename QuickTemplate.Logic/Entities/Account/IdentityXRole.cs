//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.Logic.Entities.Account
{
    [Table("IdentityXRoles", Schema = "Account")]
    internal partial class IdentityXRole : VersionEntity
    {
        public int IdentityId { get; set; }
        public int RoleId { get; set; }

        // Navigation properties
        public Identity? Identity { get; set; }
        public Role? Role { get; set; }
    }
}
#endif
//MdEnd