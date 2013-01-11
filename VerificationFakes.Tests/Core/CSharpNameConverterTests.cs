using System;
using System.Data.SqlTypes;
using NUnit.Framework;
using VerificationFakes.Core;

namespace VerificationFakesTests
{
    [TestFixture]
    public class CSharpNameConverterTests
    {
        [TestCase("Int32", Result = "int")]
        [TestCase("Int64", Result = "long")]
        [TestCase("String", Result = "string")]
        [TestCase("Double", Result = "double")]
        [TestCase("Single", Result = "float")]
        [TestCase("Object", Result = "object")]
        public string Test_GetTypeName(string netType)
        {
            var type = Type.GetType("System." + netType);
            return TypeNameConverter.GetCSharpTypeName(type);
        }

        [TestCase("op_Equality", Result = "==")]
        [TestCase("op_Inequality", Result = "!=")]

        [TestCase("op_Addition", Result = "+")]
        [TestCase("op_Subtraction", Result = "-")]
        [TestCase("op_Multiply", Result = "*")]
        
        [TestCase("op_Division", Result = "/")]
        [TestCase("op_ExclusiveOr", Result = "^")]
        [TestCase("op_BitwiseAnd", Result = "&")]

        [TestCase("op_BitwiseOr", Result = "|")]
        [TestCase("op_GreaterThan", Result = ">")]
        [TestCase("op_LessThan", Result = "<")]
        [TestCase("op_GreaterThanOrEqual", Result = ">=")]
        [TestCase("op_LessThanOrEqual", Result = "<=")]
        public string Test_GetOperatorName(string @operator)
        {
            var methodInfo = typeof(SqlInt32).GetMethod(@operator);
            
            Assert.IsNotNull(methodInfo, 
                string.Format("Specified type should have operator {0}", @operator));

            return TypeNameConverter.GetCSharpOperatorName(methodInfo);
        }
    }
}