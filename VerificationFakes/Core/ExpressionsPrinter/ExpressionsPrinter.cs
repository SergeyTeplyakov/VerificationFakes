using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Linq;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Creates string representation for the <see cref="LambdaExpression"/>.
    /// </summary>
    /// <remarks>
    /// Unfortunately build-in implementation of the ToString method is too rude.
    /// For example, we'll not see generic argument and type names, but to show
    /// more descriptive error messages we should provide more fine-tuned string
    /// representation.
    /// </remarks>
    internal class ExpressionsPrinter
    {
        private readonly string _stringParameters;
        private readonly string _stringBody;

        public ExpressionsPrinter(LambdaExpression expression)
        {
            Contract.Requires(expression != null, "expression should not be null.");

            _stringParameters = EvaluateParameters(expression.Parameters);
            _stringBody = EvaluateBody(expression.Body);
        }

        public string GetParameters()
        {
            return _stringParameters;
        }

        public string GetBody()
        {
            return _stringBody;
        }

        public override string ToString()
        {
            return string.Format("{0} => {1}", _stringParameters, _stringBody);
        }

        private string EvaluateParameters(ReadOnlyCollection<ParameterExpression> parameters)
        {
            if (parameters.Count == 0)
                return "()";
            
            if (parameters.Count == 1)
                return parameters[0].Name;
            
            return string.Format("({0})", string.Join(", ", parameters.Select(pe => pe.Name)));
        }

        private string EvaluateBody(Expression body)
        {
            return ExpressionPrinterVisitor.Print(body, true);
        }
    }
}