using System.Text;

namespace AlchemyBow.Core.Editor.Utilities
{
    /// <summary>
    /// Provides a tool to build strings by adding indented lines.
    /// </summary>
    public sealed class IndentStringBuilder
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private int indentLevel;
        private string indent;

        /// <summary>
        /// The current indent level.
        /// </summary>
        /// <returns>
        /// The current indent level.
        /// </returns>
        public int IndentLevel 
        {
            get => indentLevel;
            set 
            {
                if (value < 0)
                {
                    throw new System.ArgumentOutOfRangeException(nameof(value), value, "The indentation level cannot be a negative number.");
                }
                indentLevel = value;
                indent = new string(' ', 4 * indentLevel);
            }
        }

        /// <summary>
        /// Adds the default line terminator.
        /// </summary>
        public void AppendLine()
        {
            stringBuilder.AppendLine();
        }

        /// <summary>
        /// Adds the specified string and the default line terminator.
        /// </summary>
        /// <param name="value">The string to add.</param>
        public void AppendLine(string value)
        {
            stringBuilder.Append(indent);
            stringBuilder.AppendLine(value);
        }

        /// <summary>
        /// Gets the result string.
        /// </summary>
        /// <returns>The result string.</returns>
        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    } 
}
