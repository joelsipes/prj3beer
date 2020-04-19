using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using prj3beer.Services;

namespace prj3beer.Services
{
    /// <summary>
    /// Checks values before being displayed on the status screen.  Attaches the units, checks if the temperature
    /// is in range, and also converts to fahrenheit if necessary.
    /// </summary>
    public class CelsiusFahrenheitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double temp = (double)value; //The temperature is passed-in as an object, cast it back to a double

            if (temp < -30.0 || temp > 30.0) //the minimum and maximum acceptable temperatures
            {
                return "Temperature Out Of Range";
            }
            
            //Checks if the fahrenheit setting is applied and addends the appropriate units to the unconverted/converted temp
            return Models.Settings.TemperatureSettings ? temp + "\u00B0C" : Temperature.CelsiusToFahrenheit(temp) + "\u00B0F";
        }

        //Not yet used.  May have some application for temperature inputs/setting preferred temperature.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double temp = (double)value;

            return Models.Settings.TemperatureSettings ? temp : Temperature.FahrenheitToCelsius(temp);
        }
    }
}
