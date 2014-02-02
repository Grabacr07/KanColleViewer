using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Grabacr07.KanColleWrapper.Models
{
	public static class Rank
	{
		public static string GetName(int rank)
		{
			switch (rank)
			{
				case 1:
					return "원수";
				case 2:
					return "대장";
				case 3:
					return "중장";
				case 4:
					return "소장";
				case 5:
					return "대령";
				case 6:
					return "중령";
				case 7:
					return "풋내기 중령";
				case 8:
					return "소령";
				case 9:
					return "중견 소령";
				case 10:
				default:
					return "풋내기 소령";
			}
		}
	}
}
