using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Grabacr07.KanColleWrapper.Models
{
	public class WebTranslator
	{
		public WebTranslator()
		{
			count = 0;
		}
		static int count;
		public string RawTranslate(string orign, TranslateKind kind = TranslateKind.Naver)
		{

			if (count > 25) return orign;
			 
			switch (kind)
			{
				case TranslateKind.Naver:
					return NaverTranslator(orign);
				case TranslateKind.Google:
					return GoogleTranslator(orign);
			}
			return orign;
		}
		private string GoogleTranslator(string input)
		{
			if (input.Count() > 0)
			{
				try
				{
					bool SpanRemain = true;
					string url = String.Format("https://translate.google.com/?hl=ko&ie=UTF8&text={0}&langpair=ja%7Cko#ja/ko/{1}", HttpUtility.UrlEncode(input), HttpUtility.UrlEncode(input));

					WebClient webClient = new WebClient();
					string result = webClient.DownloadString(url);
					int temp1 = result.IndexOf("id=result_box") - 6;//span시작

					result = result.Substring(result.IndexOf("id=result_box") - 6, result.Count() - temp1);//span시작부터 문서끝까지
					result = result.Substring(0, result.IndexOf("</div"));//첫번째 </div가 발견되는 부분 이후를 모두 제거

					result = result.Substring(result.IndexOf(">") + 1, result.Count() - result.IndexOf(">") - 1);
					result = result.Substring(result.IndexOf(">") + 1, result.Count() - result.IndexOf(">") - 1);
					result = result.Replace("</span>", "");
					result = result.Replace("</div>", "");

					while (SpanRemain)
					{
						if (result.Contains("<") || result.Contains(">"))
						{
							int inx = result.IndexOf("<");
							int endinx = result.IndexOf(">") + 1;
							string removetext;

							if (inx != endinx)
							{
								removetext = result.Substring(inx, endinx - inx);
								result = result.Replace(removetext, "");
							}

							if (!result.Contains("<span")) SpanRemain = false;
						}
						else SpanRemain = false;
					}
					count++;
					return result;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					return "번역에러";
				}
			}
			else return null;

		}
		private string NaverTranslator(string input)
		{
			if (input.Count() > 0)
			{
				string url = String.Format("http://jpdic.naver.com/search.nhn?range=all&q={0}&sm=jpd_hty", HttpUtility.UrlEncode(input));

				WebClient webClient = new WebClient();

				webClient.Encoding = Encoding.UTF8;

				string result = webClient.DownloadString(url);

				try
				{
					int temp1 = result.IndexOf("jap_ico") - 13;//span시작

					result = result.Substring(temp1, result.Count() - temp1);//span시작부터 문서끝까지
					result = result.Substring(0, result.IndexOf("</div"));//첫번째 </div가 발견되는 부분 이후를 모두 제거
					if (!result.Contains("<span class=\"jp\" lang=\"ja\">")) result = result.Substring(result.IndexOf("</strong") + 22, result.Count() - result.IndexOf("</strong>") - 22);
					else result = result.Substring(result.IndexOf("</strong") + 43, result.Count() - result.IndexOf("</strong>") - 43);
					result = result.Substring(0, result.IndexOf("</span"));//첫번째 </div가 발견되는 부분 이후를 모두 제거

					result = result.Replace("\n", " ");

					count++;
					return result;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					return "번역에러";
				}
			}
			else return null;
		}
		public enum TranslateKind
		{
			Naver,
			Google,
			InfoSeek,
		}
	}
}
