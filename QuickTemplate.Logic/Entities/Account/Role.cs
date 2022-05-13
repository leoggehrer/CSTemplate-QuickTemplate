//@BaseCode
//MdStart
#if ACCOUNT_ON
namespace QuickTemplate.Logic.Entities.Account
{
    [Table("Roles", Schema = "Account")]
    [Index(nameof(Designation), IsUnique = true)]
    public partial class Role : VersionEntity
    {
        [Required]
        [MaxLength(64)]
        public string Designation { get; set; } = string.Empty;
        [Required]
        [MaxLength(256)]
        public string Description { get; set; } = string.Empty;
    }
}
#endif
//MdEnd