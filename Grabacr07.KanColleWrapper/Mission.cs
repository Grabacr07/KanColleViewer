using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper
{
	public class Mission : IIdentifiable
	{
		public int Id { get; private set; }

		public string Title { get; private set; }

		public string Detail { get; private set; }

		public Mission(int id, string title, string detail)
		{
			this.Id = id;
			this.Title = title;
			this.Detail = detail;
		}


		#region static members

		private static readonly Mission unknown = new Mission(-1, "(未知の遠征任務)", "KanColleViewer が認識していない遠征任務です。");

		/// <summary>
		/// KanColleViewer が認識していない未知の遠征任務を表します。
		/// </summary>
		public static Mission Unknown
		{
			get { return unknown; }
		}

		#endregion
	}
}
