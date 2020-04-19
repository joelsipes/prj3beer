using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace prj3beer.Models
{
    /// <summary>
    /// This Class holds all of the persistant settings in the beer app
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        //Constant values to store the Key and default values for the Settings Dictionary
        #region Setting Constants

        private const string TemperatureKey = "temperature_key";
        private static readonly bool TemperatureDefault = true;

        private const string BaseURL = "base_url";
        private static readonly string BaseURLDefault = @"https://my-json-server.typicode.com/prj3beer/prj3beer-api";

        // Stores The Current User Name
        private const string UserKey = "user_key";
        // Default value for Name should be null
        private static readonly string DefaultUser = null;

        // Stores The User Email
        private const string EmailKey = "email_key";
        // Default value for Email should be null
        private static readonly string DefaultEmail = null;

        private const string LoginMethodKey = "login_key";
        private static readonly string LoginMethodDefault = null;

        // Stores a boolean for showing the welcome modal
        private const string WelcomePromptKey = "welcome_key";

        // Default value for the Welcome Prompt (show)
        private static readonly bool WelcomePrompt = true;

        private const string NotificationKey = "notification_key";
        private static readonly bool NotificationDefault = true;

        private const string InRangeKey = "inrange_key";
        private static readonly bool InRangeDefault = true;

        private const string TooHotColdKey = "toohotcold_key";
        private static readonly bool TooHotColdDefault = true;

        private const string BeverageKey = "beverage_key";
        private static readonly int BeverageDefault = -1;
        #endregion

        /// <summary>
        /// This attribute gets or sets the temperature unit settings, 
        /// </summary>
        public static bool TemperatureSettings
        {
            get
            {
                //Returns true for Celcius and false for Fahtrenheit, if Temperature was never set its default is true 
                return AppSettings.GetValueOrDefault(TemperatureKey, TemperatureDefault);
            }
            set
            {
                //Sets the TemperatureSettings KeyValue pair and sets its value to the passed in value
                AppSettings.AddOrUpdateValue(TemperatureKey, value);
            }
        }

        /// <summary>
        /// This Property is responsible for setting/updating the values for the Currently Logged in User Name
        /// </summary>
        public static string CurrentUserName
        {
            get
            {   // Gets the value stored in the UserKey, or the default user if it is not set.
                return AppSettings.GetValueOrDefault(UserKey, DefaultUser);
            }
            set
            {   // Update the UserKey with the passed in value
                AppSettings.AddOrUpdateValue(UserKey, value);
            }
        }

        /// <summary>
        /// This Property is responsible for setting/updating the values for the Currently Logged in User Email
        /// </summary>
        public static string CurrentUserEmail
        {
            get
            {   // Gets the value stored in the EmailKey, or the default user if it is not set.
                return AppSettings.GetValueOrDefault(EmailKey, DefaultEmail);
            }
            set
            {   // Update the EmailKey with the passed in value
                AppSettings.AddOrUpdateValue(EmailKey, value);
            }
        }

        /// <summary>
        /// This property is responsible for storing and retrieving the beer database API url.
        /// </summary>
        public static string URLSetting
        {
            get
            {
                //Gets the url provided if possible, otherwise use the default url
                return AppSettings.GetValueOrDefault(BaseURL, BaseURLDefault);
            }
            set
            {
                //Sets the APIManager url to use for API interactions
                AppSettings.AddOrUpdateValue(BaseURL, value);
            }
        }

        public static string LoginMethodSetting
        {
            get
            {   // Gets the value stored in the WelcomePromptKey, or the default user if it is not set.
                return AppSettings.GetValueOrDefault(LoginMethodKey, LoginMethodDefault);
            }
            set
            {   // Update the WelcomePromptKey with the passed in value
                AppSettings.AddOrUpdateValue(LoginMethodKey, value);
            }
        }

        /// <summary>
        /// This Property is responsible for setting/updating the promp key for a user returning to the Application.
        /// </summary>
        public static bool WelcomePromptSetting
        {
            get
            {   // Gets the value stored in the WelcomePromptKey, or the default user if it is not set.
                return AppSettings.GetValueOrDefault(WelcomePromptKey, WelcomePrompt);
            }
            set
            {   // Update the WelcomePromptKey with the passed in value
                AppSettings.AddOrUpdateValue(WelcomePromptKey, value);
            }
        }

        /// <summary>
        /// This attribute gets or sets the master notification settings
        /// </summary>
        public static bool NotificationSettings
        {
            get
            {
                //Returns true or false (essentially IsToggled property) value for master notification switch
                //Otherwise returns default, which is true (IsToggled = true)
                return AppSettings.GetValueOrDefault(NotificationKey, NotificationDefault);
            }
            set
            {
                //Set the NotificationSettings key-value pair sets its value to the passed in value
                AppSettings.AddOrUpdateValue(NotificationKey, value);
            }
        }

        /// <summary>
        /// This attribute gets or sets the in-range notification subsettings
        /// </summary>
        public static bool InRangeSettings
        {
            get
            {
                //Returns true or false (essentially IsToggled property) value for in-range notification switch
                //Otherwise returns default, which is true (IsToggled = true)
                return AppSettings.GetValueOrDefault(InRangeKey, InRangeDefault);
            }
            set
            {
                //Set the InRangeSettings key-value pair sets its value to the passed in value
                AppSettings.AddOrUpdateValue(InRangeKey, value);
            }
        }

        /// <summary>
        /// This attribute gets or sets the too hot/cold notification subsettings
        /// </summary>
        public static bool TooHotColdSettings
        {
            get
            {
                //Returns true or false (essentially IsToggled property) value for too hot/cold notification switch
                //Otherwise returns default, which is true (IsToggled = true)
                return AppSettings.GetValueOrDefault(TooHotColdKey, TooHotColdDefault);
            }
            set
            {
                //Set the TooHotColdSettings key-value pair sets its value to the passed in value
                AppSettings.AddOrUpdateValue(TooHotColdKey, value);
            }
        }
        public static int BeverageSettings
        {
            get { return AppSettings.GetValueOrDefault(BeverageKey, BeverageDefault); }

            set { AppSettings.AddOrUpdateValue(BeverageKey, value); }
        }

        public static Views.SettingsMenu SettingsMenu
        {
            get => default;
            set
            {
            }
        }
    }
}