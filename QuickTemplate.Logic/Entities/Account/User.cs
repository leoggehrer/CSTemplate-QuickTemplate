//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.Logic.Entities.Account
{
    [Table("Users", Schema = "Account")]
    public partial class User : VersionEntity
    {
        public int IdentityId { get; set; }
        [Required]
        [MaxLength(64)]
        public string Firstname { get; set; } = string.Empty;
        [Required]
        [MaxLength(64)]
        public string Lastname { get; set; } = string.Empty;
    }
}
#endif
//MdEnd