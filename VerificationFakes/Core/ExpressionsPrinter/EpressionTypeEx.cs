using System.Linq.Expressions;

namespace VerificationFakes.Core
{
    internal static class EpressionTypeEx
    {
         public static string GetName(this ExpressionType type)
         {
             switch (type)
             {
                 case ExpressionType.Add:
                     return "+";
                 case ExpressionType.AddChecked:
                     return "+";
                 case ExpressionType.And:
                     return "&";
                 case ExpressionType.AndAlso:
                     return "&&";
                 case ExpressionType.Coalesce:
                     return "??";
                 case ExpressionType.Divide:
                     return "/";
                 case ExpressionType.Equal:
                     return "==";
                 case ExpressionType.ExclusiveOr:
                     return "^";
                 case ExpressionType.GreaterThan:
                     return ">";
                 case ExpressionType.GreaterThanOrEqual:
                     return ">=";
                 case ExpressionType.LeftShift:
                     return "<<";
                 case ExpressionType.LessThan:
                     return "<";
                 case ExpressionType.LessThanOrEqual:
                     return "<=";
                 case ExpressionType.Modulo:
                     return "%";
                 case ExpressionType.Multiply:
                     return "*";
                 case ExpressionType.MultiplyChecked:
                     return "*";
                 case ExpressionType.Negate:
                     return "-";
                 case ExpressionType.UnaryPlus:
                     return "+";
                 case ExpressionType.NegateChecked:
                     return "-";
                 case ExpressionType.Not:
                     return "!";
                 case ExpressionType.NotEqual:
                     return "!=";
                 case ExpressionType.Or:
                     return "|";
                 case ExpressionType.OrElse:
                     return "||";
                 case ExpressionType.RightShift:
                     return ">>";
                 case ExpressionType.Subtract:
                 case ExpressionType.SubtractChecked:
                     return "-";
                 case ExpressionType.Assign:
                     return "=";
                 case ExpressionType.Decrement:
                     return "--";
                 case ExpressionType.Increment:
                     return "++";
                 case ExpressionType.AddAssign:
                     return "+=";
                 case ExpressionType.AndAssign:
                     return "&=";
                 case ExpressionType.DivideAssign:
                     return "/=";
                 case ExpressionType.ExclusiveOrAssign:
                     return "^=";
                 case ExpressionType.LeftShiftAssign:
                     return "<<=";
                 case ExpressionType.ModuloAssign:
                     return "%=";
                 case ExpressionType.MultiplyAssign:
                     return "*=";
                 case ExpressionType.OrAssign:
                     return "|=";
                 case ExpressionType.RightShiftAssign:
                     return ">>=";
                 case ExpressionType.SubtractAssign:
                     return "-=";
                 case ExpressionType.AddAssignChecked:
                     return "+=";
                 case ExpressionType.MultiplyAssignChecked:
                     return "*=";
                 case ExpressionType.SubtractAssignChecked:
                     return "-=";
                 case ExpressionType.PreIncrementAssign:
                     return "++";
                 case ExpressionType.PreDecrementAssign:
                     return "--";
                 case ExpressionType.PostIncrementAssign:
                     return "++";
                 case ExpressionType.PostDecrementAssign:
                     return "--";
                 default:
                     return type.ToString();
             }
         }
    }
}