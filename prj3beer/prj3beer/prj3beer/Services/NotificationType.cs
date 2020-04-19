using System;
using System.Collections.Generic;
using System.Text;

namespace prj3beer.Services
{
    /// <summary>
    /// A simplified way to represent all the possible notification types
    /// </summary>
    public enum NotificationType
    {
        NO_MESSAGE, //A default that should not really be seen
        TOO_HOT, //The drink was close to or has passed the desired temperature but is increasing away from the desired temperature
        IN_RANGE_HOT, //Two degrees above the desired temperature
        PERFECT, //The drink has reached it's desired temperature 
        IN_RANGE_COLD, //Two degrees below the desired temperature
        TOO_COLD //The drink was close to or has passed the desired temperature but is decresing away from the desired temperature
    }
}
