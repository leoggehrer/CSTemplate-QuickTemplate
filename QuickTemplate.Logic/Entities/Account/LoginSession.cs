//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.Logic.Entities.Account
{
    [Table("LoginSessions", Schema = "Account")]
    internal partial class LoginSession : VersionEntity
    {
        private DateTime? _logoutTime;

        public int IdentityId { get; set; }
        public int TimeOutInMinutes { get; set; }
        [Required]
        [MaxLength(128)]
        public string SessionToken { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public DateTime LastAccess { get; set; }
        public DateTime? LogoutTime
        {
            get
            {
                OnLogoutTimeReading();
                return _logoutTime;
            }
            set
            {
                bool handled = false;
                OnLogoutTimeChanging(ref handled, value, ref _logoutTime);
                if (handled == false)
                {
                    _logoutTime = value;
                }
                OnLogoutTimeChanged();
            }
        }
        partial void OnLogoutTimeReading();
        partial void OnLogoutTimeChanging(ref bool handled, System.DateTime? value, ref System.DateTime? _logoutTime);
        partial void OnLogoutTimeChanged();
        [MaxLength(4096)]
        public string? OptionalInfo { get; set; }

        #region transient properties
        [NotMapped]
        internal byte[] PasswordHash
        {
            get => Identity != null ? Identity.PasswordHash : Array.Empty<byte>();
            set
            {
                if (Identity != null)
                    Identity.PasswordHash = value;
            }
        }
        [NotMapped]
        internal byte[] PasswordSalt
        {
            get => Identity != null ? Identity.PasswordSalt : Array.Empty<byte>();
            set
            {
                if (Identity != null)
                    Identity.PasswordSalt = value;
            }
        }
        [NotMapped]
        public bool IsRemoteAuth { get; set;  }
        [NotMapped]
        public string Origin { get; set; } = string.Empty;
        [NotMapped]
        public string Name { get; set; } = string.Empty;
        [NotMapped]
        public string Email { get; set; } = string.Empty;
        [NotMapped]
        public string JsonWebToken { get; set; } = string.Empty;

        internal bool IsActive => IsTimeout == false;
        [NotMapped]
        internal bool IsTimeout
        {
            get
            {
                TimeSpan ts = DateTime.UtcNow - LastAccess;

                return LogoutTime.HasValue || ts.TotalSeconds > TimeOutInMinutes * 60;
            }
        }
        [NotMapped]
        internal bool HasChanged { get; set; }
        [NotMapped]
        internal List<Role> Roles { get; } = new();
        #endregion transient properties

        // Navigation properties
        public Identity? Identity { get; set; }
    }
}
#endif
//MdEnd