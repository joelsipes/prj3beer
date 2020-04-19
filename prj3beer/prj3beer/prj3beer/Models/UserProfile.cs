using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Plugin.GoogleClient.Shared;
using Xamarin.Forms;

namespace prj3beer.Models
{
    /// <summary>
    /// This class is responsible for storing the current User.
    /// The OAuth Sign in requires this class
    /// </summary>
    public class UserProfile : INotifyPropertyChanged
    {
        // Current User's Full Name
        public string Name { get; set; }
        // Current User's Email
        public string Email { get; set; }
        // Current Access Token
        public string Token { get; set; }
        // Link to the User's Google Profile Picture
        public UriImageSource Picture { get; set; }

        // Event Handler that will fire if a property is updated.
        public event PropertyChangedEventHandler PropertyChanged;

        // Default Constructor which creates the Default User Profile from the "Permanent User"
        public UserProfile()
        {   // Set the Name from the Saved Settings
            Name = Settings.CurrentUserName;
            // Set the email from the Saved Settings
            Email = Settings.CurrentUserEmail;
        }
    }
}
