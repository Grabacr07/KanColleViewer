using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
	public class KanColleWindowSettings : WindowSettings
	{
		/// <summary>
		/// メイン ウィンドウから情報表示部分が分割されているかどうかを示す設定値を取得します。
		/// </summary>
		public SerializableProperty<bool> IsSplit => this.Cache(key => new SerializableProperty<bool>(key, Providers.Local, false));

		/// <summary>
		/// メイン ウィンドウの情報表示部分のドック位置を示す設定値を取得します。
		/// </summary>
		public SerializableProperty<Dock> Dock => this.Cache(key => new SerializableProperty<Dock>(key, Providers.Local, System.Windows.Controls.Dock.Bottom));
		
		/// <summary>
		/// メイン ウィンドウを自動的にリサイズするかどうかを示す設定値を取得します。
		/// </summary>
		public SerializableProperty<bool> AutomaticallyResize => this.Cache(key => new SerializableProperty<bool>(key, Providers.Roaming, true));

	}

	public class ShipCatalogWindowSettings : WindowSettings { }

	public class SlotItemCatalogWindowSettings : WindowSettings { }
}
