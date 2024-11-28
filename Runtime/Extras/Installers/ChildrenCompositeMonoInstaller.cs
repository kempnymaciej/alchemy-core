using AlchemyBow.Core.IoC;
using UnityEngine;

namespace AlchemyBow.Core.Extras.Installers
{
    /// <summary>
    /// A variant of <c>MonoInstaller</c> that locates and installs other MonoInstallers in the same GameObject and its children.
    /// </summary>
    /// <remarks>
    /// If a <c>ChildrenCompositeMonoInstaller</c> is found in a child GameObject, the child GameObject and its children are skipped, but the <c>ChildrenCompositeMonoInstaller</c> is triggered.
    /// Together with the <c>mute</c> property, this enables the creation of more sophisticated installer hierarchies.
    /// </remarks>
    [DisallowMultipleComponent]
    public class ChildrenCompositeMonoInstaller : MonoInstaller
    {
        /// <summary>
        /// If toggled, the installer skips binding installation but prevents its subordinate <c>MonoInstaller</c> instances from being triggered by a parent <c>ChildrenCompositeMonoInstaller</c>, if present.
        /// </summary>
        [Tooltip("If toggled, the installer skips binding installation but prevents its subordinate MonoInstaller instances from being triggered by a parent ChildrenCompositeMonoInstaller, if present.")]
        [SerializeField] private bool mute;
        /// <summary>
        /// If toggled, the installer includes only active GameObjects including itself.
        /// </summary>
        [Tooltip("If toggled, the installer includes only active GameObjects including itself.")]
        [SerializeField] private bool onlyActiveGameObjects;
        
        public override void InstallBindings(IBindOnlyContainer container)
        {
            if (mute)
            {
                return;
            }
            
            if (onlyActiveGameObjects && !gameObject.activeInHierarchy)
            {
                return;
            }

            foreach (var monoInstaller in GetComponents<MonoInstaller>())
            {
                if (monoInstaller == this)
                {
                    continue;
                }

                monoInstaller.InstallBindings(container);
            }
            
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                HandleChild(transform.GetChild(i), container);
            }
        }
        
        private void HandleChild(Transform child, IBindOnlyContainer container)
        {
            if (onlyActiveGameObjects && !child.gameObject.activeSelf)
            {
                return;
            }

            if (child.TryGetComponent<ChildrenCompositeMonoInstaller>(out var childrenCompositeMonoInstaller))
            {
                childrenCompositeMonoInstaller.InstallBindings(container);
                return;
            }

            foreach (var monoInstaller in child.GetComponents<MonoInstaller>())
            {
                monoInstaller.InstallBindings(container);
            }
            
            int childCount = child.childCount;
            for (int i = 0; i < childCount; i++)
            {
                HandleChild(child.GetChild(i), container);
            }
        }
    }
}