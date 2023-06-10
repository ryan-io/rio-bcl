using System;

namespace Procedural {
    /// <summary>
    ///     An exception thrown when validating simplification options.
    /// </summary>
    public sealed class ValidateSimplificationOptionsException : Exception {
        /// <summary>
        ///     Creates a new simplification options validation exception.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="message">The exception message.</param>
        public ValidateSimplificationOptionsException(string propertyName, string message)
			: base(message)
			=> this.PropertyName = propertyName;

        /// <summary>
        ///     Creates a new simplification options validation exception.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The exception that caused the validation error.</param>
        public ValidateSimplificationOptionsException(string propertyName, string message, Exception innerException)
			: base(message, innerException)
			=> this.PropertyName = propertyName;

        /// <summary>
        ///     Gets the property name that caused the validation error.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        ///     Gets the message of the exception.
        /// </summary>
        public override string Message => base.Message + Environment.NewLine + "Property name: " + PropertyName;
	}
}