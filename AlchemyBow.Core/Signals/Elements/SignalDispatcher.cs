using System;

namespace AlchemyBow.Core.Signals.Elements
{
    public sealed class SignalDispatcher<T> where T : Signal
    {
        private Action<T> callback;

        public void AddListener(Action<T> action)
        {
            callback += action;
        }

        public void RemoveListener(Action<T> action)
        {
            callback -= action;
        }

        public void Dispatch(T signal)
        {
            callback?.Invoke(signal);
        }
    } 
}
