using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Parses expressions for getting <see cref="ExpectedCall"/>.
    /// </summary>
    internal class ExpressionsParser
    {
        /// <summary>
        /// Parses specified <paramref name="expression"/> and produces expected call as a
        /// <see cref="ExpectedCall"/> object.
        /// </summary>
        public ExpectedCall Parse<T>(Expression<Action<T>> expression)
        {
            Contract.Requires(expression != null, "expression should not be null.");
            Contract.Ensures(Contract.Result<ExpectedCall>() != null,
                "Returning value should not be null.");

            var methodCall = expression.Body as MethodCallExpression;
            Contract.Assert(methodCall != null,
                "ProcessMethodCallExpression supports only method call expressions");

            var expectedCall = ProcessExpression<T>(methodCall);
            expectedCall.CallExpression = new ExpressionsPrinter(expression).ToString();
            return expectedCall;
        }

        /// <summary>
        /// Parses specified <paramref name="expression"/> and produces expected call as a
        /// <see cref="ExpectedCall"/> object.
        /// </summary>
        public ExpectedCall Parse<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            Contract.Requires(expression != null, "expression should not be null.");
            Contract.Ensures(Contract.Result<ExpectedCall>() != null,
                "Returning value should not be null.");

            var methodCall = expression.Body as MethodCallExpression;

            // For some expressions we could have not MethodCallExpression as a Body
            // (for example: (ILogWriter lw) => lw.ToString().Length
            if (methodCall == null)
            {
                string error = string.Format("Expected method call expression but was:{0}{1}",
                                             Environment.NewLine, expression);
                throw new InvalidOperationException(error);
            }

            Contract.Assert(methodCall != null,
                "ProcessMethodCallExpression supports only method call expressions");

            var expectedCall = ProcessExpression<T>(methodCall);
            expectedCall.CallExpression = new ExpressionsPrinter(expression).ToString();
            return expectedCall;
        }

        private ExpectedCall ProcessExpression<T>(MethodCallExpression methodCall)
        {
            // Check few corner cases
            var methodInfo = methodCall.Method;

            // Converting or evaluating all arguments
            var arguments = from arg in methodCall.Arguments
                            select ProcessExpression(arg);

            var parameters = arguments.ToArray();


            var expectedCall = new ExpectedCall();
            expectedCall.Method = methodInfo;
            expectedCall.Arguments = parameters;

            // Overload without additional Times argument should be called Once
            expectedCall.Times = Times.Once();

            return expectedCall;
        }

        private static object EvaluateArgument(Expression expression)
        {
            var visitor = new ArgumentsEvaluatorVisitor();
            visitor.Visit(expression);

            if (visitor.Processed)
                return visitor.Value;

            // Can't process this expression with our custom visitor.
            // Using heavy-weight compilation

            var argAsObj = Expression.Convert(expression, typeof(object));

            var value = Expression.Lambda<Func<object>>(argAsObj, null).Compile()();
            return value;
        }

        private static ArgumentValue ProcessExpression(Expression expression)
        {
            var visitor = new ExpressionArgumentsVisitor();
            visitor.Visit(expression);

            if (visitor.Processed)
                return visitor.ArgumentValue;

            // Can't process this expression with our custom visitor.
            // Using heavy-weight compilation

            var argAsObj = Expression.Convert(expression, typeof(object));

            var value = Expression.Lambda<Func<object>>(argAsObj, null).Compile()();
            return new SingleArgumentValue(value);
        }

        /// <summary>
        /// Helper class for processing parameters from the specified expected expressions.
        /// </summary>
        private class ExpressionArgumentsVisitor : ExpressionVisitor
        {
            private ArgumentValue _argumentValue;
            public bool Processed { get; private set; }

            public ArgumentValue ArgumentValue
            {
                get { return _argumentValue; }
                private set
                {
                    Processed = true;
                    _argumentValue = value;
                }
            }

            private void SetSingleArgumentValue(object value)
            {
                Processed = true;
                ArgumentValue = new SingleArgumentValue(value);
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                ArgumentValue = new SingleArgumentValue(node.Value);
                return node;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                // Member expression could be in form of simple fields or
                // properties

                if (node.Expression is ConstantExpression)
                {
                    var ci = (ConstantExpression)node.Expression;
                    var mi = (FieldInfo)node.Member;
                    SetSingleArgumentValue(ci.Value);
                }
                else if (node.Member is PropertyInfo)
                {
                    var pi = (PropertyInfo)node.Member;

                    // Visiting subexpression recursivly to obtain 
                    // property objects value (for example, for expression () => foo.X
                    // pi will contain "X" and value - "foo"
                    var visitor = new ExpressionArgumentsVisitor();
                    visitor.Visit(node.Expression);
                    var value = visitor.ArgumentValue;

                    if (visitor.Processed)
                        SetSingleArgumentValue(pi.GetValue(value));
                }

                return node;
            }

            protected override Expression VisitInvocation(InvocationExpression node)
            {
                SetSingleArgumentValue(Expression.Lambda(node).Compile().DynamicInvoke());
                return node;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var methodInfo = node.Method;

                // Its possible to use special class "It" with the set of methods
                // to specify special behavior

                if (methodInfo.DeclaringType == typeof(It))
                {
                    ArgumentValue = ProcessItType(node);
                    return node;
                }

                // We're dealing with ordinar method call.
                // Lets evaluate it deeper using the same visitor

                var visitor = new ExpressionArgumentsVisitor();
                visitor.Visit(node.Object);
                if (visitor.Processed)
                {
                    SetSingleArgumentValue(methodInfo.Invoke(visitor.ArgumentValue, null));
                }
                else
                {
                    // We can't process this method call using our custom visitor.
                    // Evaluating it dynamically (this is slower but we'll have an answer)
                    SetSingleArgumentValue(Expression.Lambda(node).Compile().DynamicInvoke());
                }
                return node;
            }

            private ArgumentValue ProcessItType(MethodCallExpression node)
            {
                var methodInfo = node.Method;

                // We have 3 method in the It class

                // TODO: think about type-safe way to check this stuff!
                // It.IsAny
                if (methodInfo.Name == "IsAny")
                    return new AnyArgumentValue();
                
                // It.IsInRange
                if (methodInfo.Name == "IsInRange")
                {
                    // Lets evaluate all arguments
                    var arguments = from arg in node.Arguments
                                    select EvaluateArgument(arg);

                    var parameters = arguments.ToArray();

                    Contract.Assert(parameters.Length == 2, 
                        "For now we have only one version of IsInRange method.");

                    return new RangeArgumentValue(parameters[0], parameters[1]);
                }

                // It.Is
                if (methodInfo.Name == "Is")
                {
                    // Is method contains a predicate that we want to extract from the
                    // expression tree.
                    
                    // node (MethodExpression) contains call to Is method and
                    // contains a predicate in the first methods argument
                    var unaryExpression = (UnaryExpression)node.Arguments[0];
                    var expression = unaryExpression.Operand;

                    // To create an ExpressionArgumentValue we should
                    // extract predicates type first
                    var funcArgumentType =
                        unaryExpression.Type // typeof(Expression<Func<T, U>>)
                        .GenericTypeArguments[0] // Func<T, U>
                        .GenericTypeArguments[0]; // T - int for Func<int, bool>
                    
                    // Constructing generic in the runtime
                    var type = typeof (ExpressionArgumentValue<>);

                    var constructedType = type.MakeGenericType(funcArgumentType);
                    return (ArgumentValue)Activator.CreateInstance(constructedType, expression);
                }

                // Unknown method!
                string message = string.Format("Unsupported method in type It. Method: {0}.", 
                    methodInfo.Name);

                throw new InvalidOperationException(message);
            }
        }

        // TODO: too many code duplication! Think about it later!
        /// <summary>
        /// Visits method call expression and evaluates all arguments
        /// </summary>
        private class ArgumentsEvaluatorVisitor : ExpressionVisitor
        {
            public bool Processed { get; private set; }
            private object _value;

            public object Value
            {
                get { return _value; }
                private set
                {
                    Processed = true;
                    _value = value;
                }
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                Value = node.Value;
                return node;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                // Member expression could be in form of simple fields or
                // properties

                if (node.Expression is ConstantExpression)
                {
                    var ci = (ConstantExpression)node.Expression;
                    Value = ci.Value;
                }
                else if (node.Member is PropertyInfo)
                {
                    var pi = (PropertyInfo)node.Member;

                    // Visiting subexpression recursivly to obtain 
                    // property objects value (for example, for expression () => foo.X
                    // pi will contain "X" and value - "foo"
                    var visitor = new ExpressionArgumentsVisitor();
                    visitor.Visit(node.Expression);
                    var value = visitor.ArgumentValue;

                    if (visitor.Processed)
                        Value = pi.GetValue(value);
                }

                return node;
            }

            protected override Expression VisitInvocation(InvocationExpression node)
            {
                Value = Expression.Lambda(node).Compile().DynamicInvoke();
                return node;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var methodInfo = node.Method;

                var visitor = new ExpressionArgumentsVisitor();
                visitor.Visit(node.Object);
                if (visitor.Processed)
                {
                    Value = methodInfo.Invoke(visitor.ArgumentValue, null);
                }
                else
                {
                    // We can't process this method call using our custom visitor.
                    // Evaluating it dynamically (this is slower but we'll have an answer)
                    Value = Expression.Lambda(node).Compile().DynamicInvoke();
                }
                return node;
            }
        }

    }
}