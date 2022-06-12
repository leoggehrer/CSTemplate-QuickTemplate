//@BaseCode
//MdStart

using QuickTemplate.Logic.Contracts;

namespace QuickTemplate.Logic.Models
{
    public abstract partial class VersionModel : IdentityModel, IVersionable
    {
        new internal virtual Entities.VersionEntity Source
        {
            get => (Entities.VersionEntity)_source!;
            set => _source = value;
        }
        /// <summary>
        /// Row version of the entity.
        /// </summary>
        public virtual byte[]? RowVersion 
        {
            get => Source.RowVersion; 
            set => Source.RowVersion = value; 
        }
    }
}
//MdEnd