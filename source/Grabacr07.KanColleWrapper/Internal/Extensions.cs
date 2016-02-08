using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nekoxy;

namespace Grabacr07.KanColleWrapper.Internal
{
	internal static class Extensions
	{
		public static string GetResponseAsJson(this Session session)
		{
			return session.Response.BodyAsString.Replace("svdata=", "");
		}

		/// <summary>
		/// <see cref="Int32" /> 型の配列に安全にアクセスします。
		/// </summary>
		public static int? Get(this int[] array, int index)
		{
			return array?.Length > index ? (int?)array[index] : null;
		}
	}
}
