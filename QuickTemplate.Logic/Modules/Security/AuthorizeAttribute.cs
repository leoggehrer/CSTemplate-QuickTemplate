//@BaseCode
//MdStart
#if ACCOUNT_ON

namespace QuickTemplate.Logic.Modules.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal partial class AuthorizeAttribute : Attribute
    {
        public bool Required { get; }
        public bool AllowModify { get; set; } = false;
        public IEnumerable<string> Roles { get; }
        public AuthorizeAttribute()
        {
            Required = true;
            Roles = Array.Empty<string>();
        }
        public AuthorizeAttribute(params string[] roles)
        {
            Required = true;
            Roles = roles ?? Array.Empty<string>();
        }
        protected AuthorizeAttribute(bool required, params string[] roles)
        {
            Required = required;
            Roles = roles ?? Array.Empty<string>();
        }
    }
}
#endif
//MdEnd