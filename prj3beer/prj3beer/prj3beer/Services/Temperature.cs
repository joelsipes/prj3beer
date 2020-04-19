using System;
using System.Collections.Generic;
using System.Text;

namespace prj3beer.Services
{
    /// <summary>
    /// This class stores a list of temperatures used to calculate the an average temperature reading.
    /// It also contains helper methods for doing temperature conversions.
    /// </summary>
    public class Temperature
    {
        /// <summary>
        /// This method will convert a Celsius value into a Fahrenheit Value
        /// </summary>
        /// <param name="tempInCels">Passed in Celsius Temp</param>
        /// <returns>Returns the value in Fahrenheit</returns>
        public static double CelsiusToFahrenheit(double tempInCels)
        {
            // Returns the rounded integer of the celsuis to fahrenheit temperature
            return (double)(Math.Round(tempInCels * 1.8 + 32));
        }

        /// <summary>
        /// This method will convert a Fahrenheit value into a Celsius Value
        /// </summary>
        /// <param name="tempInFahren">Passed in Fahrenheit Temp</param>
        /// <returns>Returns the value in Celsius</returns>
        public static double FahrenheitToCelsius(double tempInFahren)
        {
            // Returns the rounded integer of the Fahrenheit to Celsius temperature
            return (double)(Math.Round((tempInFahren - 32) * (5.0 / 9.0)));
        }
    }
}