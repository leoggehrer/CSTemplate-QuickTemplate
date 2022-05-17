//@BaseCode
//MdStart
namespace CommonBase.Extensions
{
	public static partial class ExceptionExtensions
	{
        public static string GetError(this Exception source)
        {
            var tab = string.Empty;
            var errMsg = source.Message;
            Exception? innerException = source.InnerException;

            while (innerException != null)
            {
                tab += "\t";
                errMsg = $"{errMsg}{Environment.NewLine}{tab}{innerException.Message}";
                innerException = innerException.InnerException;
            }
            return errMsg;
        }
    }
}
//MdEnd