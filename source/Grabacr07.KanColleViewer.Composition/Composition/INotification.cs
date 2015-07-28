using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// ユーザーへの通知を表すメンバーを公開します。
	/// </summary>
	public interface INotification
	{
		/// <summary>
		/// 通知の種類を示す文字列を取得します。
		/// </summary>
		string Type { get; }

		/// <summary>
		/// 通知のヘッダーを取得します。
		/// </summary>
		string Header { get; }

		/// <summary>
		/// 通知の本文を取得します。
		/// </summary>
		string Body { get; }

		/// <summary>
		/// 通知に対しユーザーが実行できるアクションを表すメソッドを取得します。
		/// </summary>
		Action Activated { get; }

		/// <summary>
		/// 通知に失敗したとき、その失敗の原因を表す例外オブジェクトを受け取るメソッドを取得します。
		/// </summary>
		Action<Exception> Failed { get; }
	}

	public static class Notification
	{
		/// <summary>
		/// 通知の種類を示す静的メンバーを公開します。
		/// </summary>
		public static class Types
		{
			/// <summary>
			/// テスト通知を識別するための文字列を取得します。
			/// </summary>
			public static string Test { get; } = nameof(Test);

			/// <summary>
			/// 工廠で艦娘の建造が完了したことを示す通知を識別するための文字列を取得します。
			/// </summary>
			public static string BuildingCompleted { get; } = nameof(BuildingCompleted);

			/// <summary>
			/// 艦娘の入渠が完了したことを示す通知を識別するための文字列を取得します。
			/// </summary>
			public static string RepairingCompleted { get; } = nameof(RepairingCompleted);

			/// <summary>
			/// 艦隊が遠征から帰投したことを示す通知を識別するための文字列を取得します。
			/// </summary>
			public static string ExpeditionReturned { get; } = nameof(ExpeditionReturned);

			/// <summary>
			/// 艦隊の疲労が回復したことを示す通知を識別するための文字列を取得します。
			/// </summary>
			public static string FleetRejuvenated { get; } = nameof(FleetRejuvenated);
		}

		public static INotification Create(string type, string header, string body, Action activated = null, Action<Exception> failed = null)
		{
			return new AnonymousNotification
			{
				Type = type,
				Header = header,
				Body = body,
				Activated = activated,
				Failed = failed,
			};
		}

		private class AnonymousNotification : INotification
		{
			public string Type { get; internal set; }

			public string Header { get; internal set; }

			public string Body { get; internal set; }

			public Action Activated { get; internal set; }

			public Action<Exception> Failed { get; internal set; }
		}
	}
}
