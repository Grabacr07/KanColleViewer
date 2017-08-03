using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
    // 원정 성공 가능 여부
    public enum ExpeditionPossible
    {
        // 성공 가능
        Possible = 0,

        // 편성 문제
        NotAccepted,

        // 보급 문제
        NotSupplied
    }
}
