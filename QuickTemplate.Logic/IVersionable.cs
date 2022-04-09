//@BaseCode
//MdStart

namespace QuickTemplate.Logic
{
    public interface IVersionable : IIdentifyable
    {
        byte[]? RowVersion { get; }
    }
}
//MdEnd