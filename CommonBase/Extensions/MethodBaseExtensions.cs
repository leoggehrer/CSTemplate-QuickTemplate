//@BaseCode
//MdStart
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CommonBase.Extensions
{
    public static class MethodBaseExtensions
    {
        public static MethodBase GetAsyncOriginal(this MethodBase method)
        {
            var result = method;

            if (method != null 
                && method.DeclaringType != null
                && method.DeclaringType.GetInterfaces().Any(i => i == typeof(IAsyncStateMachine)))
            {
                var generatedType = method.DeclaringType;
                var originalType = generatedType.DeclaringType;

                if (originalType != null)
                {
                    result = originalType.GetMethods(BindingFlags.Instance
                                                 | BindingFlags.Static
                                                 | BindingFlags.Public
                                                 | BindingFlags.NonPublic
                                                 | BindingFlags.DeclaredOnly)
                                         .First(m => m.GetCustomAttribute<AsyncStateMachineAttribute>()?.StateMachineType == generatedType);
                }
            }
            return result;
        }
    }
}
//MdEnd