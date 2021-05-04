using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace ChatApp.Mobile.Converters
{
    /// <summary>
    /// Преобразователь логического значения в аттрибут видимости и обратно.
    /// Реализует интерфейс преобразователя значений.
    /// </summary>
    public class BooleanToVisibility : IValueConverter
    {
        /// <summary>
        /// Преобразователь логического значение в значение атрибута видимости.
        /// </summary>
        /// <param name="value">Преобразуемое значение</param>
        /// <param name="targetType">Целевой ти результата</param>
        /// <param name="parameter">Параметр преобразования. Имеет значение 
        /// "Inverse" (обратный) для обратного значения результата.</param>
        /// <param name="culture">Культура приложения</param>
        /// <returns>Преобразованное значение</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = parameter as string;
            if(param == "Inverse")
            {
                return !(bool)value;
            }

            return (bool)value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
