using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using VerificationFakes;
using System.Linq;

namespace VerificationFakesTests.FluentTestCaseBuilder
{
    //TODO ST: consider extract this as a special public API!
    internal class VerificationTestCaseData : TestCaseData
    {
        public VerificationTestCaseData(object actions, object expected, Times times)
            : base(actions, expected, times)
        {
        }

        public static VerificationTestCaseData Create<T>(List<Expression<Action<T>>> actions,
            Expression<Action<T>> expected, Times times)
        {
            if (actions.Count == 1)
            {
                return new VerificationTestCaseData(actions[0], expected, times);
            }

            var testCaseData = new VerificationTestCaseData(actions, expected, times);
            
            string name = string.Join(", ", actions.Select(a => a.ToString()));
            name += ", " + expected + ", " + times;
            testCaseData.SetName(name);

            return testCaseData;
        }
        
        public static VerificationTestCaseData Create<T, TResult>(List<Expression<Func<T, TResult>>> actions,
            Expression<Func<T, TResult>> expected, Times times)
        {
            if (actions.Count == 1)
            {
                return new VerificationTestCaseData(actions[0], expected, times);
            }

            var testCaseData = new VerificationTestCaseData(actions, expected, times);
            
            string name = string.Join(", ", actions.Select(a => a.ToString()));
            name += ", " + expected + ", " + times;
            testCaseData.SetName(name);

            return testCaseData;
        }
        
    }

    internal interface IActionProvider<T>
    {
        IActionProvider<T> Do(Expression<Action<T>> expression);
        VerificationTestCaseData Expects(Expression<Action<T>> expression);
        VerificationTestCaseData Expects(Expression<Action<T>> expression, Times times);
    }

    internal class ActionProviderData<T> : IActionProvider<T>
    {
        private readonly List<Expression<Action<T>>> _list;

        public ActionProviderData(List<Expression<Action<T>>> list)
        {
            _list = list;
        }

        public IActionProvider<T> Do(Expression<Action<T>> expression)
        {
            _list.Add(expression);
            return this;
        }

        public VerificationTestCaseData Expects(Expression<Action<T>> expression)
        {
            return VerificationTestCaseData.Create(_list, expression, Times.Once());
        }

        public VerificationTestCaseData Expects(Expression<Action<T>> expression, Times times)
        {
            return VerificationTestCaseData.Create(_list, expression, times);
        }
    }

    internal interface IFuncProvider<T, TResult>
    {
        IFuncProvider<T, TResult> Do(Expression<Func<T, TResult>> expression);
        VerificationTestCaseData Expects(Expression<Func<T, TResult>> expression);
        VerificationTestCaseData Expects(Expression<Func<T, TResult>> expression, Times times);
    }

    internal class FuncProvider<T, TResult> : IFuncProvider<T, TResult>
    {
        private readonly List<Expression<Func<T, TResult>>> _list;

        public FuncProvider(List<Expression<Func<T, TResult>>> list)
        {
            _list = list;
        }

        public IFuncProvider<T, TResult> Do(Expression<Func<T, TResult>> expression)
        {
            _list.Add(expression);
            return this;
        }

        public VerificationTestCaseData Expects(Expression<Func<T, TResult>> expression)
        {
            return VerificationTestCaseData.Create(_list, expression, Times.Once());
        }

        public VerificationTestCaseData Expects(Expression<Func<T, TResult>> expression, Times times)
        {
            return VerificationTestCaseData.Create(_list, expression, times);
        }
    }

    internal class Builder
    {
        public static IActionProvider<T> Do<T>(Expression<Action<T>> expression)
        {
            return new ActionProviderData<T>(new List<Expression<Action<T>>>() { expression });
        }
    }

    internal class Builder<T>
    {
        public IActionProvider<T> Do(Expression<Action<T>> expression)
        {
            return new ActionProviderData<T>(new List<Expression<Action<T>>>() { expression });
        }
        
        public IFuncProvider<T, TResult> Do<TResult>(Expression<Func<T, TResult>> expression)
        {
            return new FuncProvider<T, TResult>(new List<Expression<Func<T, TResult>>>() { expression });
        }
    }
}