using System;
using System.Collections.Generic;
using System.Text;

namespace prj3beer.Services
{
    /// <summary>
    /// An iinterface implemented in platform specific classes
    /// Instantiated by dependenciesService on app startup
    /// </summary>
    public interface INotificationHandler
    {
        /// <summary>
        /// Intiates platform specific startup code
        /// </summary>
        void Initialize();

        /// <summary>
        /// The method to tell the device specific to platform to send a notification
        /// </summary>
        /// <param name="title">The title of the notification</param>
        /// <param name="body">The body of the notification</param>
        void SendLocalNotification(string title, string body);
    }
}