//@BaseCode
//MdStart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommonBase.Extensions
{
    public static partial class TypeExtensions
    {
        public static void CheckInterface(this Type? type, string argName)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsInterface == false)
                throw new ArgumentException($"The parameter '{argName}' must be an interface.");
        }
        public static bool IsNullableType(this Type type)
        {
            type.CheckArgument(nameof(type));

            var result = type.IsValueType == false;

            if (result == false
                && type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// Determine whether a type is simple (String, Decimal, DateTime, etc) 
        /// or complex (i.e. custom class with public properties and methods).
        /// </summary>
        public static bool IsSimpleType(this Type type)
        {
            return
               type.IsValueType ||
               type.IsPrimitive ||
               new[]
               {
               typeof(String),
               typeof(Decimal),
               typeof(DateTime),
               typeof(DateTimeOffset),
               typeof(TimeSpan),
               typeof(Guid)
               }.Contains(type) ||
               (Convert.GetTypeCode(type) != TypeCode.Object);
        }

        public static Type? GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                       "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }
        public static bool IsNumericType(this Type type)
        {
            type.CheckArgument(nameof(type));

            var result = false;
            var checkType = type;

            if (type.IsNullableType())
            {
                checkType = Nullable.GetUnderlyingType(type);
            }

            switch (Type.GetTypeCode(checkType))
            {
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    result = true;
                    break;
            }
            return result;
        }
        public static bool IsFloatingPointType(this Type type)
        {
            type.CheckArgument(nameof(type));

            var result = false;
            var checkType = type;

            if (type.IsNullableType())
            {
                checkType = Nullable.GetUnderlyingType(type);
            }

            switch (Type.GetTypeCode(checkType))
            {
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    result = true;
                    break;
            }
            return result;
        }

        public static IEnumerable<Type> GetBaseClasses(this Type type)
        {
            type.CheckArgument(nameof(type));

            static void GetBaseClassesRec(Type type, List<Type> baseClasses)
            {
                if (type.BaseType != null)
                {
                    if (baseClasses.Contains(type.BaseType) == false)
                    {
                        baseClasses.Add(type.BaseType);
                    }
                    GetBaseClassesRec(type.BaseType, baseClasses);
                }
            }
            var result = new List<Type>();

            GetBaseClassesRec(type, result);
            return result;
        }
        public static IEnumerable<Type> GetBaseInterfaces(this Type type)
        {
            type.CheckArgument(nameof(type));

            static void GetBaseInterfaces(Type type, List<Type> interfaces)
            {
                foreach (var item in type.GetInterfaces())
                {
                    if (interfaces.Contains(item) == false)
                    {
                        interfaces.Add(item);
                    }
                    GetBaseInterfaces(item, interfaces);
                }
            }
            var result = new List<Type>();

            GetBaseInterfaces(type, result);
            return result;
        }

        public static PropertyInfo? GetInterfaceProperty(this Type? type, string name)
        {
            type.CheckArgument(nameof(type));

            return type?.GetAllInterfacePropertyInfos()
                        .SingleOrDefault(p => p.Name.Equals(name));
        }

        public static Dictionary<string, PropertyItem> GetAllTypeProperties(this Type typeObject)
        {
            typeObject.CheckArgument(nameof(typeObject));

            var propertyItems = new Dictionary<string, PropertyItem>();

            GetAllTypePropertiesRec(typeObject, propertyItems);
            return propertyItems;
        }
        private static void GetAllTypePropertiesRec(Type typeObject, Dictionary<string, PropertyItem> propertyItems)
        {
            typeObject.CheckArgument(nameof(typeObject));
            propertyItems.CheckArgument(nameof(propertyItems));

            if (typeObject.BaseType != null
                && typeObject.BaseType != typeof(object))
            {
                GetAllTypePropertiesRec(typeObject.BaseType, propertyItems);
            }

            foreach (var property in typeObject.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                               .OrderBy(p => p.MetadataToken))
            {
                var propertyItem = new PropertyItem(property);

                if (propertyItems.ContainsKey(property.Name) == false)
                {
                    propertyItems.Add(property.Name, propertyItem);
                }
                else
                {
                    propertyItems[property.Name] = propertyItem;
                }
                // Bei rekursiven Strukturen kommt es hier zu einem Absturz!!!
                //if (propertyItem.IsComplexType)
                //{
                //    GetAllTypePropertiesRec(propertyItem.PropertyInfo.PropertyType, propertyItem.PropertyItems);
                //}
            }
        }

        public static IEnumerable<PropertyInfo> GetAllPropertyInfos(this Type type)
        {
            type.CheckArgument(nameof(type));

            return type.IsInterface == false ? type.GetAllTypePropertyInfos() : type.GetAllInterfacePropertyInfos();
        }
        public static IEnumerable<PropertyInfo> GetAllTypePropertyInfos(this Type type)
        {
            type.CheckArgument(nameof(type));

            var result = new List<PropertyInfo>();

            foreach (var item in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                result.Add(item);
            }
            return result;
        }

        public static IEnumerable<PropertyInfo> GetAllInterfacePropertyInfos(this Type type)
        {
            type.CheckArgument(nameof(type));

            var result = new List<PropertyInfo>();

            if (type.GetTypeInfo().IsInterface)
            {
                GetAllInterfacePropertyInfosRec(type, result);
            }
            else
            {
                foreach (var item in type.GetInterfaces())
                {
                    GetAllInterfacePropertyInfosRec(item, result);
                }
            }
            return result;
        }
        private static void GetAllInterfacePropertyInfosRec(Type type, List<PropertyInfo> properties)
        {
            type.CheckArgument(nameof(type));
            properties.CheckArgument(nameof(properties));

            foreach (var item in type.GetProperties())
            {
                if (properties.Find(p => p.Name.Equals(item.Name)) == null)
                {
                    properties.Add(item);
                }
            }
            foreach (var item in type.GetInterfaces())
            {
                GetAllInterfacePropertyInfosRec(item, properties);
            }
        }

        public static bool IsGenericTypeOf(this Type type, Type genericType)
        {
            genericType.CheckArgument(nameof(genericType));

            Type? instanceType = type;

            while (instanceType != null)
            {
                if (instanceType.IsGenericType &&
                    instanceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
                instanceType = instanceType.BaseType;
            }
            return false;
        }
    }
}
//MdEnd