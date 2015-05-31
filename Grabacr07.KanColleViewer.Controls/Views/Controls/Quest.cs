using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	public class Quest : Control
	{
		static Quest()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Quest), new FrameworkPropertyMetadata(typeof(Quest)));
		}

		#region IsDetailView 依存関係プロパティ

		public bool IsDetailView
		{
			get { return (bool)this.GetValue(IsDetailViewProperty); }
			set { this.SetValue(IsDetailViewProperty, value); }
		}
		public static readonly DependencyProperty IsDetailViewProperty =
			DependencyProperty.Register("IsDetailView", typeof(bool), typeof(Quest), new UIPropertyMetadata(false));

		#endregion

		#region Category 依存関係プロパティ

		public QuestCategory Category
		{
			get { return (QuestCategory)this.GetValue(CategoryProperty); }
			set { this.SetValue(CategoryProperty, value); }
		}
		public static readonly DependencyProperty CategoryProperty =
			DependencyProperty.Register("Category", typeof(QuestCategory), typeof(Quest), new UIPropertyMetadata(QuestCategory.Sortie));

		#endregion

		#region State 依存関係プロパティ

		public QuestState State
		{
			get { return (QuestState)this.GetValue(StateProperty); }
			set { this.SetValue(StateProperty, value); }
		}
		public static readonly DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(QuestState), typeof(Quest), new UIPropertyMetadata(QuestState.None));

		#endregion
		
		#region Progress 依存関係プロパティ

		public QuestProgress Progress
		{
			get { return (QuestProgress)this.GetValue(ProgressProperty); }
			set { this.SetValue(ProgressProperty, value); }
		}
		public static readonly DependencyProperty ProgressProperty =
			DependencyProperty.Register("Progress", typeof(QuestProgress), typeof(Quest), new UIPropertyMetadata(QuestProgress.None));

		#endregion

		#region Title 依存関係プロパティ

		public string Title
		{
			get { return (string)this.GetValue(TitleProperty); }
			set { this.SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(Quest), new UIPropertyMetadata(""));

		#endregion

		#region Detail 依存関係プロパティ

		public string Detail
		{
			get { return (string)this.GetValue(DetailProperty); }
			set { this.SetValue(DetailProperty, value); }
		}
		public static readonly DependencyProperty DetailProperty =
			DependencyProperty.Register("Detail", typeof(string), typeof(Quest), new UIPropertyMetadata(""));

		#endregion

		#region IsUntaken 依存関係プロパティ

		public bool IsUntaken
		{
			get { return (bool)this.GetValue(IsUntakenProperty); }
			set { this.SetValue(IsUntakenProperty, value); }
		}
		public static readonly DependencyProperty IsUntakenProperty =
			DependencyProperty.Register("IsUntaken", typeof(bool), typeof(Quest), new UIPropertyMetadata(false));

		#endregion

		#region TextWrapping 依存関係プロパティ

		public TextWrapping TextWrapping
		{
			get { return (TextWrapping)this.GetValue(TextWrappingProperty); }
			set { this.SetValue(TextWrappingProperty, value); }
		}
		public static readonly DependencyProperty TextWrappingProperty =
			DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(Quest), new UIPropertyMetadata(TextWrapping.Wrap));

		#endregion
	}
}
