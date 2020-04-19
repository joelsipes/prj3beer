using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace prj3beer.Services
{
    /// <summary>
    /// This is used to enable/disable the time controls so we dont have to write 5 lines of code to disable/enable all controls
    /// Uses data binding to bind the boolean to the IsEnabled property of the controls
    /// </summary>
    class TimerButtonConverter : IValueConverter
    {

        //Returns the opposite value of the boolean
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
