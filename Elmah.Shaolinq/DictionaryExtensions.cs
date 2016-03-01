using System;
using System.Collections;

namespace Elmah.Shaolinq
{
	internal static class DictionaryExtensions
	{
		internal static T Find<T>(this IDictionary dict, object key, T @default)
		{
			if (dict == null)
			{
				throw new ArgumentNullException(nameof(dict));
			}

			return (T) (dict[key] ?? @default);
		}
	}
}