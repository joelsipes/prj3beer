using System;
using System.ComponentModel;
using prj3beer.Models;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.FacebookClient;
using Newtonsoft.Json.Linq;
using prj3beer.Views;
using System.Linq;
using prj3beer.Services;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace prj3beer.ViewModels
{
    /// <summary>
    /// This is all of the functionality for OAuth.
    /// That takes place on the Credential Select View Model.
    /// Currently Only the Google Sign In is set up in the application.
    /// </summary>
    public class CredentialSelectViewModel : INotifyPropertyChanged
    {
        // Permissions string to store the keys for Facebook permissions
        protected string[] permissions = new string[] { "email", "public_profile", "user_posts" };

        // Creates a new UserProfile, set up with a getter/setter for OAuth
        public UserProfile User { get; set; } = new UserProfile();

        // Getter/Setter Attribute for the User's Name
        public string Name { get => User.Name; set => User.Name = value; }

        // Getter/Setter Attribute for the User's Email
        public string Email { get => User.Email; set => User.Email = value; }

        // Getter/Setter Attribute for the User's Picture
        public UriImageSource Picture { get => User.Picture; set => User.Picture = value; }

        // Getter/Setter Attribute for a logged in status
        public bool IsLoggedIn { get; set; }

        // Google's Token - Persistent for 1 hour
        public string Token { get; set; }

        // Command that triggers when the user logs in with Google
        public Command GoogleLoginCommand { get; set; }

        // command that will trigger when the User logs out
        public Command LogoutCommand { get; set; }

        // Command that triggers when the user logs in with Facebook
        public Command FacebookLoginCommand { get; set; }

        // LoadFacebookDataCommand - trigger the command to load all the Facebook data using LoadFacebookData method
        public Command LoadFacebookDataCommand { get; set; }

        // Private Interfaced GoogleClientManager
        // This interface enforces that elements on this ViewModel use certain methods.
        private readonly IGoogleClientManager _googleClientManager;

        // Event handler for properties changing
        public event PropertyChangedEventHandler PropertyChanged;


        // Constructor for the Credential View Model
        public CredentialSelectViewModel()
        {
            // Initialize the GoogleLogInCommand command
            GoogleLoginCommand = new Command(GoogleLoginAsync);

            // Initialize the Facebook Login Command
            FacebookLoginCommand = new Command(FacebookLogin);

            // Initialize the LoadFacebook Data Command
            //LoadFacebookDataCommand = new Command(LoadFacebookData);

            // Implement in the future for logging a user out!
            LogoutCommand = new Command(Logout);

            // Instantiate the googleClientManager
            _googleClientManager = CrossGoogleClient.Current;

            // Boolean for if the user is currently Logged in or not
            IsLoggedIn = false;
        }

        // This method is called using the LogoutCommand
        public void Logout()
        {
            // Switch Statement (gets the last signed in method from settings)
            switch (Settings.LoginMethodSetting)
            {
                case "Facebook":
                    // If We are currently logged in with facebook,
                    if (CrossFacebookClient.Current.IsLoggedIn)
                    {   // Call the logout method 
                        CrossFacebookClient.Current.Logout();
                        // Set local IsLoggedIn bool to false;
                        IsLoggedIn = false;

                        // Clear the current local user email,
                        User.Email = "";
                        // Clear the currently logged in user from the settings.
                        Settings.CurrentUserEmail = "";
                        Settings.CurrentUserName = "";

                        //Delete the Database of Friends
                        DeleteFriends();

                        // Pop This Modal
                        App.Current.MainPage.Navigation.PopModalAsync();
                    }
                    break;

                case "Google":
                    // Add the Event Handler to the OnLogout property of the Google Client Manager
                    _googleClientManager.OnLogout += OnLogoutCompleted;
                    // Call the logout function
                    _googleClientManager.Logout();
                    break;
            }

        }

        /// <summary>
        /// This function is called when a user logs out of Google
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="loginEventArgs"></param>
        private void OnLogoutCompleted(object sender, EventArgs loginEventArgs)
        {
            // Set is logged in to false, and clear all User Credentials
            IsLoggedIn = false;
            User.Email = "";
            Settings.CurrentUserEmail = "";
            Settings.CurrentUserName = "";

            // Pop the Modal off the Navigation Stack
            App.Current.MainPage.Navigation.PopModalAsync();

            // Remove the Event Handler from the Logout property
            _googleClientManager.OnLogout -= OnLogoutCompleted;
        }

        // This method is called using the LoginCommand
        public async void GoogleLoginAsync()
        {
            // Set the Last Login Method to Google
            Settings.LoginMethodSetting = "Google";

            // Add the Event Handler to the GoogleClient Manager's on login property
            _googleClientManager.OnLogin += OnLoginCompleted;

            try
            {   // Try to use the Google Client Manager to Login Asyncronously
                await _googleClientManager.LoginAsync();
            }
            catch (GoogleClientSignInNetworkErrorException error)
            {   // This error will trigger when there is a network error
                await App.Current.MainPage.DisplayAlert("Error", error.Message, "OK");
            }
            catch (GoogleClientSignInCanceledErrorException error)
            {   // This error will trigger when a user cancells a sign in/up process
                await App.Current.MainPage.DisplayAlert("Error", error.Message, "OK");
            }
            catch (GoogleClientSignInInvalidAccountErrorException error)
            {   // This error will trigger on invalid credentials being entered
                await App.Current.MainPage.DisplayAlert("Error", error.Message, "OK");
            }
            catch (GoogleClientSignInInternalErrorException error)
            {   //This error shouldn't trigger from our app, but from a google server error
                await App.Current.MainPage.DisplayAlert("Error", error.Message, "OK");
            }
            catch (GoogleClientNotInitializedErrorException error)
            {   // This error will trigger if there is an error with out OAuth Credentials
                await App.Current.MainPage.DisplayAlert("Error", error.Message, "OK");
            }
            catch (GoogleClientBaseException error)
            {   // This should catch all other errors
                await App.Current.MainPage.DisplayAlert("Error", error.Message, "OK");
            }
        }

        /// <summary>
        /// This method will complete all the Login Functionality For Google
        /// </summary>
        /// <returns></returns>
        public async void FacebookLogin()
        {
            // Set the Last Login Method to Facebook
            Settings.LoginMethodSetting = "Facebook";

            // Grab the response from the Facebook Client using the LoginAsync Method
            FacebookResponse<bool> response = await CrossFacebookClient.Current.LoginAsync(permissions);

            // Switch Statement based on the Status Code returned
            switch (response.Status)
            {
                // If the Login Was Completed
                case FacebookActionStatus.Completed:
                    // Set is Logged in to true,
                    IsLoggedIn = true;
                    // Load all of the User's Data
                    await LoadFacebookData();

                    // Task for poping the modal in the correct order
                    // May not need to create the task anymore
                    var syncTask2 = new Task(() => {
                        App.Current.MainPage.Navigation.PopModalAsync();
                    });

                    // May not need to to do this now
                    syncTask2.RunSynchronously();

                    break;

                // If a User Cancels out of the login process
                case FacebookActionStatus.Canceled:
                    // Perform No Tasks, as you will just return from the Facebook Login
                    break;

                // If the Facebook Account is unauthorized,
                case FacebookActionStatus.Unauthorized:
                    // Display an Unauthorized Message
                    await App.Current.MainPage.DisplayAlert("Unauthorized", response.Message, "Ok");
                    break;

                // Display an error if there was an error with the login process
                case FacebookActionStatus.Error:
                    // Display an Error Message
                    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Ok");
                    break;
            }
        }

        /// <summary>
        /// This method is called when a user has completed the log in process
        /// </summary>
        /// <param name="loginEventArgs">The event that is triggered</param>
        private void OnLoginCompleted(object sender, GoogleClientResultEventArgs<GoogleUser> loginEventArgs)
        {
            // If the compelted event data is not null (which it shouldn't be if the user has logged in)
            if (loginEventArgs.Data != null)
            {   // Create a new googleUser using the returned data from the event
                GoogleUser googleUser = loginEventArgs.Data;

                // Store the returned name to the local user
                User.Name = googleUser.Name;
                // As well as store it in the settings
                Settings.CurrentUserName = googleUser.Name;

                // Store the returned email to the local user
                User.Email = googleUser.Email;
                // As well as store it in settings
                Settings.CurrentUserEmail = googleUser.Email;

                // Change the logged in boolean to true
                IsLoggedIn = true;

                // You will be welcomed after sign in
                Settings.WelcomePromptSetting = true;

                // Set the token to the Active Token from the Cross Google Client
                Token = CrossGoogleClient.Current.ActiveToken;

                // Also save it to the user
                User.Token = CrossGoogleClient.Current.ActiveToken;

                // Pop The Modal
                App.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {   // If there is an issue retriving data from the returned event
                App.Current.MainPage.DisplayAlert("Error", loginEventArgs.Message, "OK");
            }

            // Removes (unsubscribes) the event handler from the GoogleClientHandler
            _googleClientManager.OnLogin -= OnLoginCompleted;
        }

        /// <summary>
        /// This method will Populate all of the user's data from the facebook profile
        /// </summary>
        /// <returns></returns>
        public async Task LoadFacebookData()
        {
            // Store the Facebook Data in a string, populated from the Current Facebook Profile
            FacebookResponse<String> jsonData = await CrossFacebookClient.Current.RequestUserDataAsync
            (                   // Request the ID, Name, Email, Picture, Cover, Friends
                  new string[] { "id", "name", "email", "picture", "cover", "friends" }, new string[] { }
            );

            // Parse the Data out of the JSON string
            JObject data = JObject.Parse(jsonData.Data);

            // Create a new User 
            User = new UserProfile()
            {   // Set it's name from the data[name] property,
                Name = data["name"].ToString(),
                // Set it's picture from the picture source, creating a new URI based on the picture, data, and url fields
                Picture = new UriImageSource { Uri = new Uri($"{data["picture"]["data"]["url"]}") },
                // Set it's email from the data[email] property
                Email = data["email"].ToString(),
            };

            // After the user is created from the returned Facebook data, set persistent user's data
            Settings.CurrentUserName = User.Name;
            Settings.CurrentUserEmail = User.Email;

            // After signing in with facebook, grab the friends
            GetFriends();

            /* These two try catch blocks are used to update each individual pages menu, depending on if a user is signed in or not */
            try
            {   // Grab the last page in the stack, 
                BeverageSelectPage current = (BeverageSelectPage)App.Current.MainPage.Navigation.NavigationStack.ElementAt(App.Current.MainPage.Navigation.NavigationStack.Count - 1);
                // call the current last page on the stack's reappearing
                current.ReAppearing();

            }
            catch (Exception){}
            try
            {
                // Grab the last page in the stack, 
                StatusPage current = (StatusPage)App.Current.MainPage.Navigation.NavigationStack.ElementAt(App.Current.MainPage.Navigation.NavigationStack.Count - 1);
                // call the current last page on the stack's reappearing
                current.ReAppearing();
            }
            catch (Exception){}
        }

        /// <summary>
        /// This method will make a me/friends request to facebook requesting back all registered friends the user has
        /// </summary>
        private async void GetFriends()
        {
            // Request the facebook data table, called "me/friends"
            var jsonData = await CrossFacebookClient.Current.QueryDataAsync("me/friends", new string[] { "user_friends" }, new Dictionary<string, string>()
            {
              //{"fields", "id, first_name, last_name, middle_name, name, email, picture"} // all info available
                {"fields", "id, name, picture"}
             });

            // Parse the response from facebook into a Json Object
            JObject data = JObject.Parse(jsonData.Data);

            // Get the "data" object out of the parsed JSON
            var dataList = data["data"];

            // Call the Store Friends Method
            StoreFriends(dataList);
        }


        /// <summary>
        /// This method is responsible for generating friends based on the returned data from the facebook login
        /// </summary>
        /// <param name="friendsData">the Json Token containing the friends list</param>
        private void StoreFriends(JToken friendsData)
        {
            // Make sure the database exists
            App.Social.Database.EnsureCreated();

            // Initialize a friend object
            Friend newFriend = new Friend();

            // For every "friend" in the JSON Token Data
            foreach (var friend in friendsData)
            {
                // Set the values of the new friend
                newFriend = new Friend()
                {   // Grab the ID from the id field
                    ID = friend["id"].ToString(),
                    // Grab the name from the name field
                    Name = friend["name"].ToString(),
                    // Store the URI, based on the friends picture/data/url as an Absolute URI
                    ImageURI = new Uri($"{friend["picture"]["data"]["url"]}").AbsoluteUri
                };
                // Add that friend to the database
                App.Social.Friend.Add(newFriend);
                //App.Social.Entry<Friend>(newFriend).State = EntityState.Detached;
            }
            // Save the changes made to the database
            App.Social.SaveChanges();
        }

        /// <summary>
        /// This method will remove all friends from the database
        /// </summary>
        private void DeleteFriends()
        {
            // Grab all the friends from the database
            List<Friend> allFriends = App.Social.Friend.ToList<Friend>();

            // Remove all the friends from the database
            App.Social.Friend.RemoveRange(allFriends);

            // Save Changes to the Database
            App.Social.SaveChanges();

            // Ensure the database is deleted
            //App.Social.Database.EnsureDeleted();
        }
    }



}
