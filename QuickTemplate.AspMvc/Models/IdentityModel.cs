//@BaseCode
//MdStart
using System.ComponentModel.DataAnnotations;

namespace QuickTemplate.AspMvc.Models
{
    public abstract partial class IdentityModel
    {
        /// <summary>
        /// ID of the entity (primary key)
        /// </summary>
        [Key]
        public int Id { get; set; }
    }
}
//MdEnd