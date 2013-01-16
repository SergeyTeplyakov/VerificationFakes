using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace VerificationFakes.Core
{
    /// <summary>
    /// Wraps <see cref="StubBase"/> and implements <see cref="ICustomStubObserver"/> to produce
    /// observed method calls and fire an event when actuall method calls.
    /// </summary>
    internal class FakesCustomStubObserver : ICustomStubObserver, IStubObserver
    {
        private readonly object _observedLock = new object();
        private readonly List<StubObservedCall> _observedCalls = new List<StubObservedCall>();
        public FakesCustomStubObserver(StubBase stub)
        {
            Contract.Requires(stub != null, "stub should not be null.");
            
            stub.InstanceObserver = this;
        }

        public ObservedCall[] GetObservedCalls()
        {
            lock (_observedLock)
            {
                return _observedCalls
                    .Select(ci => new ObservedCall(ci.StubbedMethod, ci.GetArguments()))
                    .ToArray();
            }
        }

        public event EventHandler<MethodCalledEventArgs> MethodCalled;

        void IStubObserver.Enter(Type stubbedType, Delegate stubCall)
        {
            if (stubCall == null)
                return;

            SynchronizedAdd(stubbedType, stubCall, new object[]{});
        }

        void IStubObserver.Enter(Type stubbedType, Delegate stubCall, object arg1)
        {
            if (stubCall == null)
                return;

            SynchronizedAdd(stubbedType, stubCall, new[] { arg1});
        }

        void IStubObserver.Enter(Type stubbedType, Delegate stubCall, object arg1, object arg2)
        {
            if (stubCall == null)
                return;

            SynchronizedAdd(stubbedType, stubCall, new[] { arg1, arg2});
        }

        void IStubObserver.Enter(Type stubbedType, Delegate stubCall, object arg1, object arg2, object arg3)
        {
            if (stubCall == null)
                return;

            SynchronizedAdd(stubbedType, stubCall, new[] {arg1, arg2, arg3});
        }

        void IStubObserver.Enter(Type stubbedType, Delegate stubCall, params object[] args)
        {
            if (stubCall == null)
                return;
            
            SynchronizedAdd(stubbedType, stubCall, args);
        }

        private void OnMethodCalled(MethodCalledEventArgs e)
        {
            var handler = MethodCalled;
            if (handler != null) 
                handler(this, e);
        }

        private void SynchronizedAdd(Type stubbedType, Delegate stubCall, object[] args)
        {
            var observedCall = new StubObservedCall(stubbedType, stubCall, args);

            lock(_observedLock)
            {
                _observedCalls.Add(observedCall);
            }

            OnMethodCalled(new MethodCalledEventArgs(observedCall.StubbedMethod, observedCall.GetArguments()));
        }
    }
}