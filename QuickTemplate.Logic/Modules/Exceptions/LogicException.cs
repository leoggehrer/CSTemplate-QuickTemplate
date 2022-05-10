//@BaseCode
//MdStart

namespace QuickTemplate.Logic.Modules.Exceptions
{
    /// <summary>
    /// Represents errors encountered while running the application.
    /// </summary>
    public partial class LogicException : Exception
    {
        public int ErrorId { get; } = -1;

        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorType">Identification of the error message.</param>
        public LogicException(ErrorType errorType)
            : base(ErrorMessage.GetAt(errorType))
        {
            ErrorId = (int)errorType;
        }

        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorType">Identification of the error message.</param>
        /// <param name="message">The message that describes the error.</param>
        public LogicException(ErrorType errorType, string message)
            : base(message)
        {
            ErrorId = (int)errorType;
        }

        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="errorType">Identification der Fehlermeldung.</param>
        /// <param name="ex">Exception die aufgetreten ist.</param>
        public LogicException(ErrorType errorType, Exception ex)
            : base(ex.Message, ex.InnerException)
        {
            ErrorId = (int)errorType;
        }

        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LogicException(string? message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogicException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">An instance of inner exception.</param>
        public LogicException(string? message, Exception? innerException) 
            : base(message, innerException)
        {
        }
    }
}

//MdEnd