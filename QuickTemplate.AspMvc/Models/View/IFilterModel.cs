//@BaseCode
//MdStart
namespace QuickTemplate.AspMvc.Models.View
{
    public partial interface IFilterModel
    {
        bool Show { get; }
        bool HasEntityValue { get; }
        string CreateEntityPredicate();
    }
}
//MdEnd