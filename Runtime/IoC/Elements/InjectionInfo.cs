using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AlchemyBow.Core.IoC.Elements
{
    /// <summary>
    /// This class is responsible for analyzing types in terms of injecting dependency, and reflection baking.
    /// </summary>
    public sealed class InjectionInfo
    {
        private static readonly Dictionary<Type, InjectionInfo> bakedInfos = new Dictionary<Type, InjectionInfo>();

        /// <summary>
        /// <c>InjectionInfo</c> for the parent type (or <c>null</c> if there is no parent type).
        /// </summary>
        public readonly InjectionInfo parentInfo;
        /// <summary>
        /// Reflected declared <c>[Inject]</c> fields of type (<c>null</c> if the type is not decorated with <c>[InjectionTarget]</c>).
        /// </summary>
        public readonly FieldInfo[] declaredInjectFields;

        private InjectionInfo(InjectionInfo parentInfo, FieldInfo[] declaredInjectFields)
        {
            this.parentInfo = parentInfo;
            this.declaredInjectFields = declaredInjectFields;
        }

        /// <summary>
        /// Analyzes the type for dependency injection information.
        /// </summary>
        /// <param name="type">The type to analyse.</param>
        /// <returns><c>InjectionInfo</c> for the type.</returns>
        /// <remarks>The results are cached.</remarks>
        public static InjectionInfo GetInjectionInfo(Type type)
        {
            InjectionInfo info;
            if (type == null)
            {
                info = null;
            }
            else if(!bakedInfos.TryGetValue(type, out info))
            {
                var parentInfo = GetInjectionInfo(type.BaseType);
                var declaredInjectFields = IsInjectionTarget(type) ? GetDeclaredInjectFields(type) : null;
                info = new InjectionInfo(parentInfo, declaredInjectFields);
                bakedInfos.Add(type, info);
            }

            return info;
        }

        private static FieldInfo[] GetDeclaredInjectFields(Type type)
        {
            return type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(fi => fi.GetCustomAttribute<InjectAttribute>() != null).ToArray();
        }

        private static bool IsInjectionTarget(Type type)
        {
            return type.GetCustomAttribute<InjectionTargetAttribute>(false) != null;
        }
    } 
}
