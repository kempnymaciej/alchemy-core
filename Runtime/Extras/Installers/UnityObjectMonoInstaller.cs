using AlchemyBow.Core.IoC;
using UnityEngine;

namespace AlchemyBow.Core.Extras.Installers
{
    /// <summary>
    /// A variant of <c>MonoInstaller</c> that enables binding a single Unity object (e.g., a <c>MonoBehaviour</c> or a <c>ScriptableObject</c>) assigned through the inspector.
    /// </summary>
    public class UnityObjectMonoInstaller : MonoInstaller
    {
        /// <summary>
        /// Prevents the target from being installed when toggled.
        /// </summary>
        [Tooltip("Prevents the target from being installed when toggled.")]
        [SerializeField] private bool mute;
        
        /// <summary>
        /// The target Unity object to bind. For example, a <c>MonoBehaviour</c> or a <c>ScriptableObject</c>.
        /// </summary>
        [Tooltip("The target Unity object to bind. For example, a MonoBehaviour or a ScriptableObject.")]
        [SerializeField] private Object target;
        
        /// <summary>
        /// Determines whether the target should be bound as inaccessible.
        /// </summary>
        [Tooltip("Determines whether the target should be bound as inaccessible.")]
        [SerializeField] private bool asInaccessible;
        
        /// <summary>
        /// Determines whether to try to add the target to the dynamic list of core loading callbacks handlers.
        /// </summary>
        [Tooltip("Determines whether to try to add the target to the dynamic list of core loading callbacks handlers.")]
        [SerializeField] private bool asCoreLoadingCallbacksHandler = true;
        
        public override void InstallBindings(IBindOnlyContainer container)
        {
            if (mute)
            {
                return;
            }
            
            if (asInaccessible)
            {
                container.BindInaccessible(target);
            }
            else
            {
                container.Bind(target.GetType(), target);
            }

            if (asCoreLoadingCallbacksHandler && target is ICoreLoadingCallbacksHandler callbacksHandler)
            {
                container.AddToDynamicListBinding(callbacksHandler);
            }
        }
    }
}