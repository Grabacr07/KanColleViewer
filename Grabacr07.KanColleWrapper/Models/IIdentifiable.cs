using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 要素を一意に識別できるようにします。
	/// </summary>
	public interface IIdentifiable
	{
		int Id { get; }
	}
}
