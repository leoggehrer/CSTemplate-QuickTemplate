//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.Logic.Entities.Account
{
    [Table("LoginSessions", Schema = "Account")]
    internal partial class LoginSession : VersionEntity
    {
        private DateTime? _logoutTime;
        private Identity? identity;

        public int IdentityId { get; internal set; }
        public int TimeOutInMinutes { get; private set; }
        [Required]
        [MaxLength(128)]
        public string SessionToken { get; internal set; } = string.Empty;
        public DateTime LoginTime { get; internal set; } = DateTime.UtcNow;
        public DateTime LastAccess { get; internal set; } = DateTime.UtcNow;
        public DateTime? LogoutTime
        {
            get
            {
                OnLogoutTimeReading();
                return _logoutTime;
            }
            internal set
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
        partial void OnLogoutTimeChanging(ref bool handled, DateTime? value, ref DateTime? _logoutTime);
        partial void OnLogoutTimeChanged();
        [MaxLength(4096)]
        public string? OptionalInfo { get; internal set; }

        #region transient properties
        [NotMapped]
        public byte[] PasswordHash { get; private set; } = Array.Empty<byte>();
        [NotMapped]
        public byte[] PasswordSalt { get; private set; } = Array.Empty<byte>();
        [NotMapped]
        public bool IsRemoteAuth { get; internal set; }
        [NotMapped]
        public string Origin { get; internal set; } = string.Empty;
        [NotMapped]
        public string Name { get; internal set; } = string.Empty;
        [NotMapped]
        public string Email { get; internal set; } = string.Empty;
        [NotMapped]
        public string JsonWebToken { get; internal set; } = string.Empty;

        public bool IsActive => IsTimeout == false;
        [NotMapped]
        public bool IsTimeout
        {
            get
            {
                TimeSpan ts = DateTime.UtcNow - LastAccess;

                return LogoutTime.HasValue || ts.TotalSeconds > TimeOutInMinutes * 60;
            }
        }
        [NotMapped]
        public bool HasChanged { get; set; }
        [NotMapped]
        public List<Role> Roles { get; } = new();
        #endregion transient properties

        // Navigation properties
        public Identity? Identity
        {
            get => identity;
            set
            {
                identity = value;
                TimeOutInMinutes = identity != null ? identity.TimeOutInMinutes : 0;
                PasswordHash = identity != null ? identity.PasswordHash : Array.Empty<byte>();
                PasswordSalt = identity != null ? identity.PasswordSalt : Array.Empty<byte>();
            }
        }
        public LoginSession Clone()
        {
            var result = new LoginSession();

            result.CopyFrom(this);
            foreach (var role in Roles)
            {
                var cRole = new Role();

                cRole.CopyFrom(role);
                result.Roles.Add(cRole);
            }
            return result;
        }
    }
}
#endif
//MdEnd