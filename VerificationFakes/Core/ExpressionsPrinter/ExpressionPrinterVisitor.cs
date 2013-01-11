using System.Linq;
using System.Linq.Expressions;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Visitor for getting string representation for the expression.
    /// </summary>
    internal sealed class ExpressionPrinterVisitor : ExpressionVisitor
    {
        private readonly bool _isHighLevel;

        public string StringRepresentation
        {
            get { return _rep; }
        }

        private string _rep;

        public ExpressionPrinterVisitor(bool isHighLevel)
        {
            _isHighLevel = isHighLevel;
        }

        /// <summary>
        /// Prints specified <paramref name="node"/> and add parenthesis if
        /// <paramref name="isHighLevel"/> is false.
        /// </summary>
        public static string Print(Expression node, bool isHighLevel = false)
        {
            var visitor = new ExpressionPrinterVisitor(isHighLevel);
            visitor.Visit(node);
            return visitor.StringRepresentation;
        }

        public override Expression Visit(Expression node)
        {
            // If we're not processed expression by special method
            // lets use base ToString implementation.
            if (node != null)
                _rep = node.ToString();

            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            string operatorName;
            if (node.Method == null)
            {
                operatorName = string.Format(" {0} ", node.NodeType.GetName());
            }
            else
            {
                // VisitBinary should be called only for operators but its possible
                // that we don't know this special C# operators name

                var name = TypeNameConverter.GetCSharpOperatorName(node.Method);
                if (name != null)
                    operatorName = string.Format(" {0} ", name);
                else
                    operatorName = node.Method.Name;
            }
            // BinaryExpression consists of 3 parts: left operator right
            _rep = string.Format("{0}{1}{2}",
                Print(node.Left),
                operatorName,
                Print(node.Right));

            if (!_isHighLevel)
                _rep = string.Format("({0})", _rep);

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return base.VisitConstant(node);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return base.VisitInvocation(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.Operand is LambdaExpression)
                _rep = PrintEx(node.Operand as LambdaExpression);
            else
                _rep = Print(node.Operand);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            string callSite = node.Expression != null
                ? Print(node.Expression)
                : TypeNameConverter.GetTypeDefinition(node.Member.DeclaringType);

            _rep = string.Format("{0}.{1}", callSite, node.Member.Name);
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // We have two cases: generic and non-generic methods
            
            if (node.Method.IsGenericMethod)
            {
                // For generic methods we should add type name and generic arguments
                _rep = string.Format("{0}.{2}<{1}>({3})",
                    TypeNameConverter.GetGenericTypeBaseName(node.Method.DeclaringType),
                    string.Join(", ",
                        node.Method.GetGenericArguments().Select(TypeNameConverter.GetCSharpTypeName)),
                    node.Method.Name,
                    string.Join(", ",
                        node.Arguments.Select(e => PrintEx(e))));

                return node;
            }

            // For non-generic methods we'll evaluate call-site and all arguments

            string callSite = node.Object != null 
                ? Print(node.Object)
                : TypeNameConverter.GetTypeDefinition(node.Method.DeclaringType);

            _rep = string.Format("{0}.{1}({2})",
                callSite,
                node.Method.Name,
                string.Join(", ", node.Arguments.Select(e => PrintEx(e))));

            return node;
        }

        private string PrintEx(Expression expression)
        {
            if (expression is LambdaExpression)
            {
                var ss = new ExpressionsPrinter(expression as LambdaExpression).ToString();
                return ss;
            }

            string s = Print(expression);
            return s;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            // Treat generic construction differently
            if (node.Type.IsGenericType)
            {
                _rep = string.Format("new {0}<{1}>({2})", 
                    TypeNameConverter.GetGenericTypeBaseName(node.Type),
                    string.Join(", ",
                        node.Type.GenericTypeArguments.Select(TypeNameConverter.GetCSharpTypeName)),
                    string.Join(", ",
                        node.Arguments.Select(e => PrintEx(e))));
                return node;
            }

            return base.VisitNew(node);
        }
    }
}