//@BaseCode
//MdStart

namespace QuickTemplate.Logic.Modules.Exceptions
{
    /// <summary>
    /// Represents errors that occur during application execution.
    /// </summary>
    public partial class AuthorizationException : LogicException
    {
        /// <summary>
        /// Initializes a new instance of the AuthorizationException class with a specified error message.
        /// </summary>
        /// <param name="errorType">Identification of the error message.</param>
        public AuthorizationException(ErrorType errorType)
            : base(errorType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AuthorizationException class with a specified error message.
        /// </summary>
        /// <param name="errorType">Identification of the error message.</param>
        /// <param name="message">The message that describes the error.</param>
        public AuthorizationException(ErrorType errorType, string message)
            : base(errorType, message)
        {
        }
    }
}
//MdEnd