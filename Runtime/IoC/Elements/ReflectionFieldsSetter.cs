using System.Reflection;

namespace AlchemyBow.Core.IoC.Elements
{
    /// <summary>
    /// Provides a mechanism for setting specific fields to specific values.
    /// </summary>
    public sealed class ReflectionFieldsSetter
    {
        private readonly (FieldInfo, object)[] fields;

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="fields">An array of field-value pairs to be set.</param>
        public ReflectionFieldsSetter((FieldInfo, object)[] fields)
        {
            this.fields = fields;
        }

        /// <summary>
        /// Sets the values of the fields in the specified object.
        /// </summary>
        /// <param name="target">The object where the fields need to be set.</param>
        public void SetFields(object target)
        {
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    field.Item1.SetValue(target, field.Item2);
                } 
            }
        }
    }
}
