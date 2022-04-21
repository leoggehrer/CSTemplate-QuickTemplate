//@BaseCode
//MdStart
using System.Reflection;

namespace CommonBase.Extensions
{
    public partial class PropertyItem
    {
        public PropertyItem(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            PropertyItems = new Dictionary<string, PropertyItem>();
        }

        public bool IsStringType => PropertyInfo.PropertyType == typeof(string);

        public bool IsArrayType => PropertyInfo.PropertyType.IsArray;

        public bool IsComplexType => PropertyInfo.PropertyType.GetTypeInfo().IsValueType == false;

        public bool CanRead => PropertyInfo.CanRead;

        public bool CanWrite => PropertyInfo.CanWrite;

        public PropertyInfo PropertyInfo { get; private set; }
        public Dictionary<string, PropertyItem> PropertyItems { get; private set; }
    }
}
//MdEnd