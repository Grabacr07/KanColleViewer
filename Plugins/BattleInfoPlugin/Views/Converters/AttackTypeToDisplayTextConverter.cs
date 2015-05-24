using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using BattleInfoPlugin.Models;

namespace BattleInfoPlugin.Views.Converters
{
    public class AttackTypeToDisplayTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is AttackType)) return "";
            var type = (AttackType)value;
            return type == AttackType.주주컷인 ? "컷인 (x1.5)"
                : type == AttackType.주철컷인 ? "컷인 (x1.3)"
                : type == AttackType.주전컷인 ? "컷인 (x1.2)"
                : type == AttackType.주부컷인 ? "컷인 (x1.1)"
                : type == AttackType.뇌격컷인 ? "컷인 (x1.5 x2)"
                : type == AttackType.주주주컷인 ? "컷인 (x2.0)"
                : type == AttackType.주주부컷인 ? "컷인 (x1.75)"
                : type == AttackType.주뢰컷인 ? "컷인 (x1.3 x2)"
                : type == AttackType.연격 ? "연격 (x1.2 x2)"
                : "통상";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
