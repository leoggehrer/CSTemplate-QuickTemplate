//@BaseCode
//MdStart
#if ACCOUNT_ON
using QuickTemplate.Logic.Modules.Common;

namespace QuickTemplate.Logic.Entities.Account
{
    [Table("Identities", Schema = "Account")]
    [Index(nameof(Email), IsUnique = true)]
    internal partial class Identity : VersionEntity
    {
        [MaxLength(36)]
        public string Guid { get; internal set; } = string.Empty;
        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(128)]
        public string Email { get; set; } = string.Empty;
        public int TimeOutInMinutes { get; set; } = 30;
        public bool EnableJwtAuth { get; set; }
        public int AccessFailedCount { get; set; }
        public State State { get; set; } = State.Active;

        [Required]
        [MaxLength(512)]
        internal byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        [Required]
        [MaxLength(512)]
        internal byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        #region transient properties
        [NotMapped]
        public string Password { get; set; } = string.Empty;
        #endregion transient properties

        // Navigation properties
        public List<IdentityXRole> IdentityXRoles { get; set; } = new();
    }
}
#endif
//MdEnd