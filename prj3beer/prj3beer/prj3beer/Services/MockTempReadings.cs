using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace prj3beer.Services
{
    /// <summary>
    /// Placeholder for temperature inputs.  Bounces between -35 and 35 degrees celsius
    /// </summary>
    public static class MockTempReadings
    {
        private static bool isCounting = false;
        private static double temp;
        private static bool goesDown; //whether the temperature counts down or up.

        public static double Temp
        {
            get
            {
                return temp;
            }
            set
            {
                temp = value;
            }
        }

        public static bool GoesDown
        {
            get
            {
                return goesDown;
            }
            set
            {
                goesDown = value;
            }
        }

        /// <summary>
        /// Initiates the counting.
        /// </summary>
        /// <param name="newTemp">The temperature to start counting from.  20 by default.</param>
        /// <param name="direction">Which direciton to incrememnt.  Down by default.</param>
        public static void StartCounting(double newTemp=20.0, bool direction=true, bool count=true)
        {
            Temp = newTemp;
            GoesDown = direction;

            //Do not start a new timer if a previous one exists, will run simultaneously with previous timers.
            if (!isCounting)
            {
                isCounting = true;

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (temp < -35.0)
                    {
                        goesDown = false;
                    }
                    if (temp > 35.0)
                    {
                        goesDown = true;
                    }
                    if (count)
                    {
                        if (goesDown)
                        {
                            temp -= 1.0;
                        }
                        else
                        {
                            temp += 1.0;
                        }
                    }
                    return true;
                });
            }
        }
    }
}
