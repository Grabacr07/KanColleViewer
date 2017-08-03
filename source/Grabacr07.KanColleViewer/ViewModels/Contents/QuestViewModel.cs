﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using System.Net;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class QuestViewModel : ViewModel
	{
		#region Id 변경통지 프로퍼티

		private int _Id;
		public int Id
		{
			get { return this._Id; }
			set
			{
				if (this._Id != value)
				{
					this._Id = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Type 変更通知プロパティ

		private QuestType _Type;

		public QuestType Type
		{
			get { return this._Type; }
			set
			{
				if (this._Type != value)
				{
					this._Type = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Category 変更通知プロパティ

		private QuestCategory _Category;

		public QuestCategory Category
		{
			get { return this._Category; }
			set
			{
				if (this._Category != value)
				{
					this._Category = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region State 変更通知プロパティ

		private QuestState _State;

		public QuestState State
		{
			get { return this._State; }
			set
			{
				if (this._State != value)
				{
					this._State = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region StateText 変更通知プロパティ

		public string StateText
		{
			get
			{
				if (this.State == QuestState.Accomplished) return " 완료 ";
				else if (this.Progress == QuestProgress.Progress80) return " 80% ";
				else if (this.Progress == QuestProgress.Progress50) return " 50% ";
				return "";
			}
		}

		#endregion

		#region Progress 変更通知プロパティ

		private QuestProgress _Progress;

		public QuestProgress Progress
		{
			get { return this._Progress; }
			set
			{
				if (this._Progress != value)
				{
					this._Progress = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Title 変更通知プロパティ

		private string _Title;

		public string Title
		{
			get { return this._Title; }
			set
			{
				if (this._Title != value)
				{
					this._Title = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Detail 変更通知プロパティ

		private string _Detail;

		public string Detail
		{
			get { return this._Detail; }
			set
			{
				if (this._Detail != value)
				{
					this._Detail = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region TitleJP 変更通知プロパティ

		private string _TitleJP;

		public string TitleJP
		{
			get { return this._TitleJP; }
			set
			{
				if (this._TitleJP != value)
				{
					this._TitleJP = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region DetailJP 変更通知プロパティ

		private string _DetailJP;

		public string DetailJP
		{
			get { return this._DetailJP; }
			set
			{
				if (this._DetailJP != value)
				{
					this._DetailJP = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsUntaken 変更通知プロパティ

		private bool _IsUntaken;

		public bool IsUntaken
		{
			get { return this._IsUntaken; }
			set
			{
				if (this._IsUntaken != value)
				{
					this._IsUntaken = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region JPView 変更通知プロパティ

		private bool _JPView;

		public bool JPView
		{
			get { return this._JPView; }
			set
			{
				if (this._JPView != value)
				{
					this._JPView = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public string QuestIdText =>
			QuestNameTable.IdNameTable.ContainsKey(this.Id)
				? QuestNameTable.IdNameTable[this.Id]
				: string.Format("[{0}]", this.Id);

		#region Page 변경통지 프로퍼티

		private int _Page { get; set; }
		public int Page
		{
			get { return this._Page; }
			set
			{
				if (this._Page != value)
				{
					this._Page = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region LastOnPage 변경통지 프로퍼티

		private bool _LastOnPage { get; set; }
		public bool LastOnPage
		{
			get { return this._LastOnPage; }
			set
			{
				if (this._LastOnPage != value)
				{
					this._LastOnPage = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region QuestProgressText 변경통지 프로퍼티

		private string _QuestProgressText { get; set; }
		public string QuestProgressText
		{
			get { return this._QuestProgressText; }
			set
			{
				if (this._QuestProgressText != value)
				{
					this._QuestProgressText = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("QuestTrackingVisible");
				}
			}
		}

		#endregion

		#region QuestProgressValue 변경통지 프로퍼티

		private double _QuestProgressValue { get; set; }
		public double QuestProgressValue
		{
			get { return this._QuestProgressValue; }
			set
			{
				if (this._QuestProgressValue != value)
				{
					this._QuestProgressValue = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("QuestTrackingVisible");
				}
			}
		}

		#endregion

		public bool QuestTrackingVisible => (this.QuestProgressText?.Length > 0) && (this.State != QuestState.None || this.QuestProgressValue > 0);


		public QuestViewModel(Quest quest)
		{
			this.JPView = false;

			if (quest == null)
			{
				this.IsUntaken = true;
			}
			else
			{
				this.Id = quest.Id;
				this.IsUntaken = false;

				this.Type = quest.Type;
				this.Category = quest.Category;

				this.State = quest.State;
				this.Progress = quest.Progress;

				this.Title = WebUtility.HtmlDecode(quest.Title).Replace("<br>", Environment.NewLine);
				this.Detail = WebUtility.HtmlDecode(quest.Detail).Replace("<br>", Environment.NewLine);

				this.TitleJP = WebUtility.HtmlDecode(quest.TitleJP).Replace("<br>", Environment.NewLine);
				this.DetailJP = WebUtility.HtmlDecode(quest.DetailJP).Replace("<br>", Environment.NewLine);

				this.Page = quest.Page + 1;
			}
			
		}
	}
}
