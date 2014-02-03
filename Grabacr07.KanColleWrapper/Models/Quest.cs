using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using System.IO;

namespace Grabacr07.KanColleWrapper.Models
{
	public class Quest : RawDataWrapper<kcsapi_quest>, IIdentifiable
	{
		public int Id
		{
			get { return this.RawData.api_no; }
		}

		/// <summary>
		/// 任務のカテゴリ (編成、出撃、演習 など) を取得します。
		/// </summary>
		public QuestCategory Category
		{
			get { return (QuestCategory)this.RawData.api_category; }
		}

		/// <summary>
		/// 任務の種類 (1 回のみ、デイリー、ウィークリー) を取得します。
		/// </summary>
		public QuestType Type
		{
			get { return (QuestType)this.RawData.api_type; }
		}

		/// <summary>
		/// 任務の状態を取得します。
		/// </summary>
		public QuestState State
		{
			get { return (QuestState)this.RawData.api_state; }
		}

		/// <summary>
		/// 任務の進捗状況を取得します。
		/// </summary>
		public QuestProgress Progress
		{
			get { return (QuestProgress)this.RawData.api_progress_flag; }
		}

		/// <summary>
		/// 任務名を取得します。
		/// </summary>
		public string Title
		{
            //원본 코드. api값을 그대로 출력한다
			//get { return this.RawData.api_title; }
            //commit발췌.https://github.com/Zharay/KanColleViewer/commit/ba21e509635aa59343b1070abd97702d1b060eb4
            //수정코드.https://github.com/Zharay/KanColleViewer/blob/ba21e509635aa59343b1070abd97702d1b060eb4/Grabacr07.KanColleWrapper/Models/Quest.cs
            get
            {
                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                string Main_folder = Path.GetDirectoryName(location);
                if (System.IO.File.Exists(Main_folder + "\\quest.txt") == true)
                {
                    System.IO.StreamReader filereader = new System.IO.StreamReader(Main_folder + "\\quest.txt", System.Text.Encoding.UTF8, true);
                    string read_line = null;
                    string jap_name = null;
                    string eng_name = null;
                    while (true)
                    {
                        read_line = filereader.ReadLine();
                        if (String.IsNullOrEmpty(read_line)) { filereader.Close(); break; }
                        else
                        {
                            char[] delimiter = { ';' };
                            jap_name = read_line.Split(delimiter)[1];
                            eng_name = read_line.Split(delimiter)[2];
                            if (String.Equals(RawData.api_title, jap_name)) { filereader.Close(); return eng_name; }
                        }
                    }
                    return this.RawData.api_title;
                }
                else
                {
                    return this.RawData.api_detail;
                }
            }


		}

		/// <summary>
		/// 任務の詳細を取得します。
		/// </summary>
		public string Detail
		{
			//원본코드
            //get { return this.RawData.api_detail; }
            //commit발췌.https://github.com/Zharay/KanColleViewer/commit/ba21e509635aa59343b1070abd97702d1b060eb4
            //수정코드.https://github.com/Zharay/KanColleViewer/blob/ba21e509635aa59343b1070abd97702d1b060eb4/Grabacr07.KanColleWrapper/Models/Quest.cs
            get
            {//get시작
                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                string Main_folder = Path.GetDirectoryName(location);
                if (System.IO.File.Exists(Main_folder + "\\quest.txt") == true)
                {
                System.IO.StreamReader filereader = new System.IO.StreamReader(Main_folder+"\\quest.txt", System.Text.Encoding.UTF8, true);
                
                
                    string read_line = null;
                    string jap_name = null;
                    string eng_name = null;
                    while (true)
                    {
                        read_line = filereader.ReadLine();
                        if (String.IsNullOrEmpty(read_line)) { filereader.Close(); break; }
                        else
                        {
                            char[] delimiter = { ';' };
                            jap_name = read_line.Split(delimiter)[3];
                            eng_name = read_line.Split(delimiter)[4];
                            if (String.Equals(RawData.api_detail, jap_name)) { filereader.Close(); return eng_name; }
                        }
                    }
                    return this.RawData.api_detail;
                }
                else
                {
                    return this.RawData.api_detail;
                }
                //get 종료
            }

		}


		public Quest(kcsapi_quest rawData) : base(rawData) { }


		public override string ToString()
		{
			return string.Format("ID = {0}, Category = {1}, Title = \"{2}\", Type = {3}, State = {4}", this.Id, this.Category, this.Title, this.Type, this.State);
		}
	}
}
