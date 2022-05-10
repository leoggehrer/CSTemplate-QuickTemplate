//@BaseCode
//MdStart
using System.Text;

namespace QuickTemplate.Logic.Modules.Exceptions
{
    internal static partial class ErrorMessage
	{
		static ErrorMessage()
		{
			ClassConstructing();
			Messages = new Dictionary<ErrorType, string>
			{
#if ACCOUNT_ON
				{ ErrorType.InitAppAccess, "The initialization of the app access is not permitted because an app access has already been initialized." },
				{ ErrorType.InvalidAccount, "Invalid identity or password." }
#endif
			};

			InitMessages();
			ClassConstructed();
		}
		static partial void ClassConstructing();
		static partial void ClassConstructed();
		private static void InitMessages()
		{
			foreach (var item in Enum.GetValues(typeof(ErrorType)))
			{
				if (Messages.ContainsKey((ErrorType)item) == false)
				{
					var sb = new StringBuilder();
					var error = Enum.GetName(typeof(ErrorType), item) ?? string.Empty;

					foreach (var chr in error)
					{
						if (char.IsUpper(chr))
						{
							if (sb.Length > 0)
							{
								sb.Append(' ');
							}
							sb.Append(chr);
						}
						else
							sb.Append(chr);
					}
					Messages.Add((ErrorType)item, sb.ToString());
				}
			}
		}
		private static Dictionary<ErrorType, string> Messages { get; set; }

		internal static string GetAt(ErrorType errorType)
		{
			string result = string.Empty;

			if (Messages.ContainsKey(errorType))
			{
				result = Messages[errorType];
			}
			return result;
		}
	}
}
//MdEnd