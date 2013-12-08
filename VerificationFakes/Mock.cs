﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using VerificationFakes.Core;

namespace VerificationFakes
{
    /// <summary>
    /// Provides a mock implementation of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Generic type parameter for mocking.</typeparam>
    public class Mock<T> where T : class
    {
        // TODO ST: we have only one spot in the codebase that depends on Fakes.
        // Consider creating a special shim for this.

        private readonly List<ExpectedCall> _setupCalls = new List<ExpectedCall>();

        private readonly ExpressionsParser _parser = new ExpressionsParser();
        private readonly ICustomStubObserver _actualCallsObserver;
        private readonly Verificator _verificator;

        public Mock(StubBase<T> stub)
            : this(stub, MockBehavior.Default)
        {}

        public Mock(StubBase<T> stub, MockBehavior behavior)
        {
            Contract.Requires(stub != null, "stub should not be null");

            _actualCallsObserver = new FakesCustomStubObserver(stub);
            _actualCallsObserver.MethodCalled += VerifyObservedCall;
            _verificator = new Verificator(behavior, new ErrorFormatter());
        }

        private void VerifyObservedCall(object sender, MethodCalledEventArgs e)
        {
            _verificator.VerifyForUnexpectedCall(_setupCalls, e.ObservedCall);
        }

        public void Setup(Expression<Action<T>> expression)
        {
            Contract.Requires(expression != null, "expression should not be null.");
            Contract.Ensures(_setupCalls.Count == Contract.OldValue(_setupCalls.Count) + 1,
                "Setup method should one expected call to _setupCalls.");

            Setup(expression, Times.Once());
        }
        
        public void Setup(Expression<Action<T>> expression, Times times)
        {
            Contract.Requires(expression != null, "expression should not be null.");
            Contract.Requires(times != null, "times should not be null.");
            Contract.Ensures(_setupCalls.Count == Contract.OldValue(_setupCalls.Count) + 1,
                "Setup method should one expected call to _setupCalls.");

            var expected = _parser.Parse(expression);
            expected.Times = times;
            _setupCalls.Add(expected);
        }

        public void Setup<TResult>(Expression<Func<T, TResult>> expression)
        {
            Contract.Requires(expression != null, "expression should not be null.");
            Contract.Ensures(_setupCalls.Count == Contract.OldValue(_setupCalls.Count) + 1,
                "Setup method should one expected call to _setupCalls.");

            Setup(expression, Times.Once());
        }
        
        public void Setup<TResult>(Expression<Func<T, TResult>> expression, Times times)
        {
            Contract.Requires(expression != null, "expression should not be null.");
            Contract.Requires(times != null, "times should not be null.");

            Contract.Ensures(_setupCalls.Count == Contract.OldValue(_setupCalls.Count) + 1,
                "Setup method should one expected call to _setupCalls.");

            var expected = _parser.Parse(expression);
            expected.Times = times;

            _setupCalls.Add(expected);
        }

        // TODO: argument name is not good! (took from the Moq)
        public void Verify(Expression<Action<T>> expression)
        {
            Contract.Requires(expression != null, "expression should not be null");

            Verify(expression, Times.Once());
        }
        
        public void Verify(Expression<Action<T>> expression, Times times)
        {
            Contract.Requires(expression != null, "expression should not be null");

            var expected = _parser.Parse(expression);
            expected.Times = times;

            var actual = _actualCallsObserver.GetObservedCalls();

            _verificator.Verify(expected, actual);
        }
        
        public void Verify<TResult>(Expression<Func<T, TResult>> expression)
        {
            Contract.Requires(expression != null, "expression should not be null");

            Verify(expression, Times.Once());
        }
        
        public void Verify<TResult>(Expression<Func<T, TResult>> expression, Times times)
        {
            Contract.Requires(expression != null, "expression should not be null");

            var expected = _parser.Parse(expression);
            expected.Times = times;

            var actual = _actualCallsObserver.GetObservedCalls();

            _verificator.Verify(expected, actual);
        }

        public void VerifyAll()
        {
            var actual = _actualCallsObserver.GetObservedCalls();
            _verificator.VerifyAll(_setupCalls, actual);
        }
    }

}


// TODO List
/*
 * Features:
 * 1. Strict mode
 * 2. It.IsRegex
 * 3. TestCaseData Generator?
 * 4. Move using of the Fakes into one plase. Create a ability to use other frameworks as well.
 * 
 * Cleanup
 * 1. Test abstract classes not only interfaces
 * 2. Documentation (with samples in the xml-doc)
 * 3. Publish via nuget
 * 4. Fix TODO's. Some of them are crucial
 * 5. Folder structure in the main project
 * 6. Cleanup test cases
 * 7. Check contracts
 * 
 * 
 */