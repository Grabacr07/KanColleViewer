using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Views.Contents
{
	public class QuestViewSource : CollectionViewSource
	{
		#region Type 依存関係プロパティ

		public QuestType Type
		{
			get { return (QuestType)this.GetValue(TypeProperty); }
			set { this.SetValue(TypeProperty, value); }
		}
		public static readonly DependencyProperty TypeProperty =
			DependencyProperty.Register("Type", typeof(QuestType), typeof(QuestViewSource), new UIPropertyMetadata(QuestType.OneTime));

		#endregion

		public QuestViewSource()
		{
			this.Filter += (sender, args) =>
			{
				var quest = args.Item as QuestViewModel;
				args.Accepted = quest != null && quest.Type == this.Type;
			};
		}
	}
}
