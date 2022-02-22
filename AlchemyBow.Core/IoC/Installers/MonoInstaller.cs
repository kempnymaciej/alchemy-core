using UnityEngine;

namespace AlchemyBow.Core.IoC.Installers
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        public abstract void InstallBindings(IBindOnlyContainer container);
    }
}
