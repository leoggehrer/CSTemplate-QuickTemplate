//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.Logic.Entities.Account
{
    [Table("ActionLogs", Schema = "Account")]
    public partial class ActionLog : VersionEntity
    {
        public int IdentityId { get; set; }
        public DateTime Time { get; set; }
        [Required]
        [MaxLength(256)]
        public string Subject { get; set; } = string.Empty;
        [Required]
        [MaxLength(128)]
        public string Action { get; set; } = string.Empty;
        [Required]
        public string Info { get; set; } = string.Empty;
    }
}
#endif
//MdEnd