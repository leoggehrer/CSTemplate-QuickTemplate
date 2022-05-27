//@BaseCode
//MdStart
#if ACCOUNT_ON && REVISION_ON
namespace QuickTemplate.Logic.Entities.Revision
{
    [Table("Histories", Schema = "Revision")]
    internal partial class History : IdentityEntity
    {
        public int IdentityId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public DateTime ActionTime { get; set; }
        [Required]
        [MaxLength(128)]
        public string SubjectName { get; set; } = String.Empty;
        public int SubjectId { get; set; }
        public string JsonData { get; set; } = String.Empty;
    }
}
#endif
//MdEnd