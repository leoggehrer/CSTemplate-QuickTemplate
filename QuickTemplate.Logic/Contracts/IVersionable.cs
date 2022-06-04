//@BaseCode
//MdStart

namespace QuickTemplate.Logic.Contracts
{
    public partial interface IVersionable : IIdentifyable
    {
        byte[]? RowVersion { get; }
    }
}
//MdEnd