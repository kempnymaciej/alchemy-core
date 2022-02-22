using AlchemyBow.Core.Signals.Elements;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlchemyBow.Core.Signals
{
    public sealed class SignalDeclarator
    {
        private Dictionary<Type, object> dispatchers;

        public SignalDeclarator()
        {
            this.dispatchers = new Dictionary<Type, object>();
        }

        public void Declare<T>() where T : Signal
        {
            var type = typeof(T);
            if (!dispatchers.ContainsKey(type))
            {
                dispatchers.Add(type, new SignalDispatcher<T>());
            }
            else
            {
                Debug.LogWarning($"The signal ({type}) is already registered.");
            }
        }

        public Dictionary<Type, object> ExtractAndResetDispatchers()
        {
            var dispatchers = this.dispatchers;
            this.dispatchers = new Dictionary<Type, object>();
            return dispatchers;
        }
    } 
}
