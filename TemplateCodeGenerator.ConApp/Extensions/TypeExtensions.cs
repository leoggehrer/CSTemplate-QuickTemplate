//@BaseCode
//MdStart
using System.Text;

namespace TemplateCodeGenerator.ConApp.Extensions
{
    internal static partial class TypeExtensions
    {
        private const char GenericSpecialChar = '`';
        private const string GenericSeparator = ", ";

        public static string GetCleanName(this Type type)
        {
            var name = type.Name;

            if (type.IsGenericType)
            {
                name = name.Remove(name.IndexOf(GenericSpecialChar));
            }
            return name;
        }
        public static string GetCodeDefinition(this Type type)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{0}.{1}", type.Namespace, type.GetCleanName());
            if (type.IsGenericType)
            {
                var names = from genericArg in type.GetGenericArguments()
                            select GetCodeDefinition(genericArg);
                sb.Append('<');
                sb.Append(string.Join(GenericSeparator, names.ToArray()));
                sb.Append('>');
            }
            if (sb.ToString().StartsWith("System.Nullable<")
                && sb.ToString().EndsWith(">"))
            {
                sb.Replace("System.Nullable<", string.Empty);
                sb.Replace(">", string.Empty);

                sb.Append('?');
            }
            return sb.ToString();
        }
    }
}
//MdEnd