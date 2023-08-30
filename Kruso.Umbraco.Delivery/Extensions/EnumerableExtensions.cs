using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Extensions
{
	internal static class EnumerableExtensions
	{
		internal static int Count(this IEnumerable lst)
		{
			int count = 0;
			foreach (var item in lst)
			{
				++count;
			}
			return count;
		}
	}
}
