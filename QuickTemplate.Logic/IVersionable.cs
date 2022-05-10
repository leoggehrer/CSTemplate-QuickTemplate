//@BaseCode
//MdStart

namespace QuickTemplate.Logic
{
    public partial interface IVersionable : IIdentifyable
    {
        byte[]? RowVersion { get; }
    }
}
//MdEnd