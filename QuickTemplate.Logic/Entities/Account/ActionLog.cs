//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.Logic.Entities.Account
{
    [Table("ActionLogs", Schema = "Account")]
    public partial class ActionLog : VersionEntity
    {
        public int IdentityId { get; internal set; }
        public DateTime Time { get; internal set; }
        [Required]
        [MaxLength(256)]
        public string Subject { get; internal set; } = string.Empty;
        [Required]
        [MaxLength(128)]
        public string Action { get; internal set; } = string.Empty;
        [Required]
        public string Info { get; internal set; } = string.Empty;
    }
}
#endif
//MdEnd