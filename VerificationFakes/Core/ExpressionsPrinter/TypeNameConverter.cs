using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using Microsoft.CSharp;
using System.Linq;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Helper class for converting type to C#-friendly names.
    /// </summary>
    internal static class TypeNameConverter
    {
        private readonly static Lazy<OperatorConverter> _converter = 
            new Lazy<OperatorConverter>(() => new OperatorConverter());

        /// <summary>
        /// Returns string representation for the specified <paramref name="type"/>.
        /// </summary>
        /// <remarks>
        /// This method produces different result for generic and non-generic types.
        /// For generic types resulting string would contains all generic arguments
        /// as well.
        /// </remarks>
        public static string GetTypeDefinition(Type type)
        {
            Contract.Requires(type != null, "type should not be null.");
            return type.IsGenericType ? GetGenericTypeDefinition(type) : type.Name;
        }

        /// <summary>
        /// Returns string representation for specified generic type.
        /// </summary>
        public static string GetGenericTypeDefinition(Type type)
        {
            Contract.Requires(type != null, "type should not be null.");
            Contract.Requires(type.IsGenericType, "specified type should be generic type.");

            return string.Format("{0}<{1}>", 
                GetGenericTypeBaseName(type),
                string.Join(", ", type.GetGenericArguments().Select(GetCSharpTypeName)));
        }

        /// <summary>
        /// Returns base name for generic type.
        /// </summary>
        public static string GetGenericTypeBaseName(Type type)
        {
            // For generic types type.Name returns something like
            // Generic`1 or Generic`3 but we need to obtain
            // generics base name like Genric or List

            if (type.IsGenericType)
            {
                string name = type.Name;
                return name.Substring(0, name.IndexOf("`", StringComparison.InvariantCulture));
            }
            return type.Name;
        }

        /// <summary>
        /// Returns C# alias for specified <paramref name="type"/> for build-in types.
        /// </summary>
        public static string GetCSharpTypeName(Type type)
        {
            Contract.Requires(type != null, "type should not be null.");
            Contract.Ensures(Contract.Result<string>() != null);

            using (var provider = new CSharpCodeProvider())
            {
                var typeRef = new CodeTypeReference(type);
                return provider.GetTypeOutput(typeRef);
            }
        }

        /// <summary>
        /// Returns C# operator's name or null, if <paramref name="methodInfo"/> is not
        /// representing known C# operator.
        /// </summary>
        public static string GetCSharpOperatorName(MethodInfo methodInfo)
        {
            Contract.Requires(methodInfo != null, "method should not be null.");

            return _converter.Value.GetOperatorName(methodInfo.Name);
        }

        class OperatorConverter
        {
            private readonly Dictionary<string, string> _map = new Dictionary<string, string>();

            public OperatorConverter()
            {
                _map["op_Equality"] = "==";
                _map["op_Inequality"] = "!=";

                _map["op_Addition"] = "+";
                _map["op_Subtraction"] = "-";
                _map["op_Multiply"] = "*";
                _map["op_Division"] = "/";

                _map["op_ExclusiveOr"] = "^";
                _map["op_BitwiseAnd"] = "&";
                _map["op_BitwiseOr"] = "|";

                _map["op_GreaterThan"] = ">";
                _map["op_LessThan"] = "<";
                _map["op_GreaterThanOrEqual"] = ">=";
                _map["op_LessThanOrEqual"] = "<=";
            }

            public string GetOperatorName(string methodName)
            {
                Contract.Requires(methodName != null, "methodName should not be null.");
                
                string result;
                _map.TryGetValue(methodName, out result);
                return result;
            }
        }
    }
}