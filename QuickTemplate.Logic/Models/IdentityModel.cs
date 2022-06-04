//@BaseCode
//MdStart

using QuickTemplate.Logic.Contracts;

namespace QuickTemplate.Logic.Models
{
    public abstract partial class IdentityModel : IIdentifyable
    {
        /// <summary>
        /// ID of the entity (primary key)
        /// </summary>
        public virtual int Id { get; set; }
    }
}
//MdEnd