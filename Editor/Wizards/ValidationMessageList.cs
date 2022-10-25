using System.Collections.Generic;

namespace AlchemyBow.Core.Editor.Wizards
{
    /// <summary>
    /// Represents a list of various validation messages.
    /// </summary>
    public sealed class ValidationMessageList
    {
        private readonly List<string> errors = new List<string>();
        private readonly List<string> warnings = new List<string>();
        private readonly List<string> infos = new List<string>();

        /// <summary>
        /// Determines if there are errors.
        /// </summary>
        /// <returns>
        /// <c>true</c> if there is at least one error; Otherwise, <c>false</c>.
        /// </returns>
        public bool HasErrors => errors.Count > 0;

        /// <summary>
        /// Determines if there are warnings.
        /// </summary>
        /// <returns>
        /// <c>true</c> if there is at least one warning; Otherwise, <c>false</c>.
        /// </returns>
        public bool HasWarnings => warnings.Count > 0;

        /// <summary>
        /// Determines if there are infos.
        /// </summary>
        /// <returns>
        /// <c>true</c> if there is at least one info; Otherwise, <c>false</c>.
        /// </returns>
        public bool HasInfos => infos.Count > 0;

        /// <summary>
        /// The collection of errors.
        /// </summary>
        /// <returns>
        /// The collection of errors.
        /// </returns>
        public IEnumerable<string> Errors => errors;

        /// <summary>
        /// The collection of warnings.
        /// </summary>
        /// <returns>
        /// The collection of warnings.
        /// </returns>
        public IEnumerable<string> Warnings => warnings;

        /// <summary>
        /// The collection of infos.
        /// </summary>
        /// <returns>
        /// The collection of infos.
        /// </returns>
        public IEnumerable<string> Infos => infos;

        /// <summary>
        /// Adds an error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public void AddError(string message)
        {
            errors.Add(message);
        }

        /// <summary>
        /// Adds a warning message.
        /// </summary>
        /// <param name="message">The warning message.</param>
        public void AddWarning(string message)
        {
            warnings.Add(message);
        }

        /// <summary>
        /// Adds an info message.
        /// </summary>
        /// <param name="message">The info message.</param>
        public void AddInfo(string message)
        {
            infos.Add(message);
        }

        /// <summary>
        /// Removes all messages.
        /// </summary>
        public void Clear()
        {
            errors.Clear();
            warnings.Clear();
            infos.Clear();
        }
    } 
}
