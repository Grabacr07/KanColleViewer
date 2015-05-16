using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper
{
	public class CatchedErrorLogWriter
	{
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		public CatchedErrorLogWriter()
		{

		}
		public void ReportException(object sender, Exception exception)
		{
			#region const
			const string messageFormat = @"
===========================================================
ERROR, date = {0}, sender = {1},
{2}
";
			string path = Path.Combine(MainFolder, "CatchedError.log");

			#endregion

			try
			{
				var message = string.Format(messageFormat, DateTimeOffset.Now, sender, exception);

				Debug.WriteLine(message);
				File.AppendAllText(path, message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
