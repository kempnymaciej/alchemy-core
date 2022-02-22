using UnityEngine;

namespace AlchemyBow.Core.IoC.Installers
{
    public class SimpleMonoBehaviourInstaller<TKey, TValue> : MonoInstaller 
        where TValue : MonoBehaviour, TKey
    {
        [SerializeField]
        private TValue instance = null;

        public override void InstallBindings(IBindOnlyContainer container)
        {
            container.Bind<TKey, TValue>(instance);
        }
    }
}
