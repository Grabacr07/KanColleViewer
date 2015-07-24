﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Controls
{
	/// <summary>
	/// 艦娘のコンディションに対応した色を表示するアイコンを表します。
	/// </summary>
	public class ConditionIcon : Control
	{
		static ConditionIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ConditionIcon), new FrameworkPropertyMetadata(typeof(ConditionIcon)));
		}
		
		#region Condition 依存関係プロパティ

		/// <summary>
		/// 艦娘のコンディションを取得または設定します。
		/// </summary>
		public ConditionType ConditionType
		{
			get { return (ConditionType)this.GetValue(ConditionTypeProperty); }
			set { this.SetValue(ConditionTypeProperty, value); }
		}
		/// <summary>
		/// <see cref="ConditionType"/> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty ConditionTypeProperty =
			DependencyProperty.Register("ConditionType", typeof(ConditionType), typeof(ConditionIcon), new UIPropertyMetadata(ConditionType.Normal));

		#endregion
	}
}
