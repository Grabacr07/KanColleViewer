using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Grabacr07.Desktop.Metro.Win32;

namespace Grabacr07.Desktop.Metro.Chrome
{
	/// <summary>
	/// <see cref="GlowWindow"/> の各種座標を計算します。
	/// </summary>
	internal abstract class GlowWindowProcessor
	{
		#region static members

		/// <summary>
		/// ウィンドウの発光サイズを取得します。
		/// </summary>
		public static double GlowSize { get; set; }

		/// <summary>
		/// ウィンドウの発光効果のエッジサイズを取得します。
		/// </summary>
		public static double EdgeSize { get; set; }

		static GlowWindowProcessor()
		{
			GlowSize = 9.0;
			EdgeSize = 20.0;
		}

		#endregion

		public abstract Orientation Orientation { get; }

		/// <summary>
		/// 派生クラスでオーバーライドされると、<see cref="GlowWindow"/>
		/// に設定する <see cref="Control.HorizontalContentAlignment"/> プロパティの値を取得します。
		/// </summary>
		public abstract HorizontalAlignment HorizontalAlignment { get; }

		/// <summary>
		/// 派生クラスでオーバーライドされると、<see cref="GlowWindow"/>
		/// に設定する <see cref="Control.VerticalContentAlignment"/> プロパティの値を取得します。
		/// </summary>
		public abstract VerticalAlignment VerticalAlignment { get; }

		/// <summary>
		/// 派生クラスでオーバーライドされると、<see cref="GlowWindow"/>
		/// の左端座標を取得します。
		/// </summary>
		/// <param name="ownerLeft">アタッチ先ウィンドウの左端座標。</param>
		/// <param name="ownerWidth">アタッチ先ウィンドウの現在の横幅。</param>
		public abstract double GetLeft(double ownerLeft, double ownerWidth);

		/// <summary>
		/// 派生クラスでオーバーライドされると、<see cref="GlowWindow"/>
		/// の上端座標を取得します。
		/// </summary>
		/// <param name="ownerTop">アタッチ先ウィンドウの上端座標。</param>
		/// <param name="ownerHeight">アタッチ先ウィンドウの現在の縦幅。</param>
		public abstract double GetTop(double ownerTop, double ownerHeight);

		/// <summary>
		/// 派生クラスでオーバーライドされると、<see cref="GlowWindow"/>
		/// の横幅を取得します。
		/// </summary>
		/// <param name="ownerLeft">アタッチ先ウィンドウの左端座標。</param>
		/// <param name="ownerWidth">アタッチ先ウィンドウの現在の横幅。</param>
		public abstract double GetWidth(double ownerLeft, double ownerWidth);

		/// <summary>
		/// 派生クラスでオーバーライドされると、<see cref="GlowWindow"/>
		/// の縦幅を取得します。
		/// </summary>
		/// <param name="ownerTop">アタッチ先ウィンドウの上端座標。</param>
		/// <param name="ownerHeight">アタッチ先ウィンドウの現在の縦幅。</param>
		public abstract double GetHeight(double ownerTop, double ownerHeight);

		/// <summary>
		/// 派生クラスでオーバーライドされると、<paramref name="point"/> 座標でのヒットテスト結果を返します。
		/// </summary>
		/// <param name="point">ウィンドウ メッセージで受け取ったマウス座標。</param>
		/// <param name="actualWidht">発光ウィンドウの現在の幅。</param>
		/// <param name="actualHeight">発光ウィンドウの現在の高さ。</param>
		/// <returns>ヒットテスト結果。</returns>
		public abstract HitTestValues GetHitTestValue(Point point, double actualWidht, double actualHeight);

		/// <summary>
		/// 派生クラスでオーバーライドされると、<paramref name="point"/> 座標でのカーソルを返します。
		/// </summary>
		/// <param name="point">ウィンドウ メッセージで受け取ったマウス座標。</param>
		/// <param name="actualWidht">発光ウィンドウの現在の幅。</param>
		/// <param name="actualHeight">発光ウィンドウの現在の高さ。</param>
		/// <returns>マウスカーソル。</returns>
		public abstract Cursor GetCursor(Point point, double actualWidht, double actualHeight);
	}
}
