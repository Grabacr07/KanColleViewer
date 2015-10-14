using System.Collections.Generic;
using System.Security.Permissions;
using Grabacr07.KanColleWrapper.Models.Translations;

namespace Grabacr07.KanColleWrapper.Models.Updater
{
	public class UpdateAvailableEventArgs
	{
		public string Version { get; }

		public UpdateAvailableEventArgs(string version)
		{
			this.Version = version;
		}
	}
}