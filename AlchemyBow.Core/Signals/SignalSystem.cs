using AlchemyBow.Core.Signals.Elements;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlchemyBow.Core.Signals
{
    public sealed class SignalSystem
    {
        private readonly Dictionary<Type, object> dispatchers;

        public SignalSystem(SignalDeclarator signalDeclarator)
        {
            dispatchers = signalDeclarator.ExtractAndResetDispatchers();
        }

        public void Subscribe<T>(Action<T> action) where T : Signal
        {
            GetDispatcher<T>()?.AddListener(action);
        }

        public void Unsubscribe<T>(Action<T> action) where T : Signal
        {
            GetDispatcher<T>()?.RemoveListener(action);
        }

        public void Dispatch<T>(T signal) where T : Signal
        {
            if(signal != null)
            {
                GetDispatcher<T>()?.Dispatch(signal);
            }
            else
            {
                Debug.LogError("Attempt to dispatch 'null' signal.");
            }
        }

        private SignalDispatcher<T> GetDispatcher<T>() where T : Signal
        {
            var type = typeof(T);
            if (dispatchers.TryGetValue(type, out var dispatcher))
            {
                return (SignalDispatcher<T>)dispatcher;
            }
            else
            {
                Debug.LogError($"The signal ({type}) is not declared.");
            }
            return null;
        }
    } 
}
