using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace wp05_bikeshop.Logics
{
    internal class MyConverter : IValueConverter // interface 메서드 안쓴 상태에서는 오류표시남
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            // 대상에다 표현 할 때 값을 변환, 표현(OneWay)
        {
            return value.ToString() + " km/h";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            // 대상값이 바뀌어서 원본(소스)의 값을 변환, 표현(TwoWay)
        {
            return int.Parse((string)value) * 3;
        }
    }
}
