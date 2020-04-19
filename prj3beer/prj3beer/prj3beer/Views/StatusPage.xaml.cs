using Microsoft.Data.Sqlite;
using prj3beer.Models;
using prj3beer.Services;
using prj3beer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Plugin.FacebookClient;

namespace prj3beer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusPage : ContentPage
    {
        // Static View Model for the page
        static StatusViewModel svm;
        // Static Beverage to keep track of current Beverage
        static Beverage currentBeverage;
        // Static Preference Object to keep track of
        public static Preference preferredBeverage;
        // Static Brand Object to keep track of current Beverage/Preference
        static Brand currentBrand;
        // Saved ID from the last selected Beverage
        int savedID;
        
        public StatusPage()
        {
            InitializeComponent();
            PopulateTimeEntryFields();
            //The id on the settings page of the app
            // Defaults as -1, seleccting a beverage changes it
            savedID = Settings.BeverageSettings;

            //If a beverage was not selected
            if (savedID == -1)
            {
                //Set default values of a beverage
                beverageName.Text = "No Beverage";
                brandName.Text = "No Brand";
                beverageImage.Source = ImageSource.FromFile("placeholder_can");
                TemperatureStepper.IsEnabled = false;
                TemperatureInput.IsEnabled = false;
            }
            else
            {
                // Instantiate new StatusViewModel
                svm = new StatusViewModel();

                // Setup the current Beverage (find it from the Context) -- This will be passed in from a viewmodel/bundle/etc in the future.
                //currentBeverage = new Beverage { BeverageID = 1, Name = "Great Western Radler", Brand = svm.Context.Brands.Find(2), Type = Models.Type.Radler, Temperature = 2 };
                currentBeverage = svm.Context.Beverage.Find(savedID);
                currentBrand = svm.Context.Brand.Find(currentBeverage.BrandID);
                //svm.Context.Beverage.Find(2);

                // Setup the preference object using the passed in beverage
                SetupPreference();

                // When you first start up the Status Screen, Disable The Inputs (on first launch of application)
                EnablePageElements(false);

                // If the current Beverage is set, (will run if a beverage has been selected)
                if (preferredBeverage != null)
                {   // enable all the elements on the page
                    EnablePageElements(true);
                }

                PopulateStatusScreen();
              
            }
            MockTempReadings.StartCounting();
        }



        //This method will populate the entry fields with values
        private void PopulateTimeEntryFields()
        {
            //This will hold the values for the time entry fields
            List<int> minuteList = new List<int>();
            List<int> hourList = new List<int>();
            for (int i = 0; i <= 60; i++)
            {
                if (i <= 24)
                {
                    hourList.Add(i);
                }
                minuteList.Add(i);
            }


            //MenuPage page = new MenuPage(); //What is this doing here?
            hourPicker.ItemsSource = hourList;
            minutePicker.ItemsSource = minuteList;
            secondPicker.ItemsSource = minuteList;

            //Set the default value to the first entry in the picker
            hourPicker.SelectedIndex = 0;
            minutePicker.SelectedIndex = 0;
            secondPicker.SelectedIndex = 0;
        }

        /// <summary>
        /// This method is run if there is a beverage selected
        /// Sets the images and text elements to represent the selected beverage
        /// </summary>
        private void PopulateStatusScreen()
        {
            // Sets displayed information of the beverage
            beverageName.Text = currentBeverage.Name.ToString();
            brandName.Text = currentBrand.Name.ToString();
            beverageImage.Source = (preferredBeverage.SaveImage(currentBeverage.ImageURL)).Source;
            
            // Size of all our images we currently use, and looks good on screen
            beverageImage.WidthRequest = 200;
            beverageImage.HeightRequest = 200;

            // Ensure elements are enabled if there is a beverage selected
            beverageName.IsEnabled = false;
            brandName.IsEnabled = false;
            beverageImage.IsEnabled = false;
            FavouriteButton.Source = preferredBeverage.Favourite ? "Favourite" : "NotFavourite";
        }

        public void UpdateViewModel(object sender, EventArgs args)
        {
            svm.IsCelsius = Settings.TemperatureSettings;
        }

        /// <summary>
        /// This method sets up a Preferred beverage object with the passed in beverage
        /// </summary>
        /// <param name="passedInBeverage">Beverage passed in from other page</param>
        //private void SetupPreference(Beverage passedInBeverage)
        private void SetupPreference()
        {   // Set the page's preferred beverage equal to -> Finding the Beverage in the Database.
            // If the object is found in the database, it will return itself immediately,
            // and attach itself to the context (Database).

            // TODO: Handle Pre-existing Preference Object.
            preferredBeverage = svm.Context.Preference.Find(savedID);
            //preferredBeverage = null; // This is what the previous line SHOULD be doing.


            // If that Preferred beverage did not exist, it will be set to null,
            // So if it is null...
            if(preferredBeverage == null)
            {   // Create a new Preferred Beverage, with copied values from the Passed In Beverage.
                preferredBeverage = new Preference() { BeverageID = currentBeverage.BeverageID, Temperature = currentBeverage.Temperature, Favourite = false };
                // Add the beverage to the Context (Database)
                svm.Context.Preference.Add(preferredBeverage);
                svm.Context.SaveChanges();
            }

        }
       
        /// <summary>
        /// This method will write changes to the Database for any changes that have happened.
        /// </summary>
        /// <param name="context">Database</param>
        public async void UpdatePreference(BeerContext context)
        {
            try
            {   // Set the Temperature of the Preferred beverage to the StatusViewModel's Temperature,
                // Do a calculation if the temperature is currently set to fahrenheit
                preferredBeverage.Temperature = svm.IsCelsius ? svm.Temperature.Value : ((svm.Temperature.Value - 32) / 1.8);
            }
            catch (Exception)
            {
                throw;
            }

            try
            {   // Write Changes to Database when it is not busy.
                svm.Context.Preference.Update(preferredBeverage);
                await context.SaveChangesAsync();
            }
            catch (SqliteException) { throw; }
        }

        /// <summary>
        /// This method will enable or disable all inputs on the screen
        /// </summary>
        /// <param name="enabled">True or False</param>
        private void EnablePageElements(bool enabled)
        {
            // Enable/Disable Steppers
            this.TemperatureStepper.IsEnabled = enabled;

            // Enable / Disable Entry
            this.TemperatureInput.IsEnabled = enabled;
        }

        /// <summary>
        /// This method will clear the input field when the Entry Box gains focus
        /// </summary>
        /// <param name="sender">Entry Field</param>
        /// <param name="args"></param>
        private void SelectEntryText(object sender, EventArgs args)
        {   // Store the sender casted as an entry to an Entry Object (to avoid casting repeatedly)
            Entry text = (Entry)sender;

            // This string will get the text from the StatusViewModel's Preferred Temperature String
            string cursorPosition = ((StatusViewModel)BindingContext).PreferredTemperatureString;
            // Set the value of the entry to an empty string
            text.Text = "";
            // Then set the text to the text retreived from the SVM
            text.Text = cursorPosition;

            // Set the cursor position to 0
            text.CursorPosition = 0;
            // Select all of the Text in the Entry
            text.SelectionLength = text.Text.Length;

            UpdatePreference(svm.Context);
            //this.PrefTemp.Text = preferredBeverage.Temperature.ToString();
        }

        /// <summary>
        /// When the Stepper is changed, update the preference
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemperatureStepperChanged(object sender, ValueChangedEventArgs e)
        {   // Update the preference object using the Context in the StatusViewModel
            UpdatePreference(svm.Context);
        }

        /// <summary>
        /// This method is called every time the page is opened.
        /// </summary>
        protected override void OnAppearing()
        {   // Instantiate a new StatusViewModel
            svm = new StatusViewModel();

            //currentTemperature.SetBinding(Label.TextProperty, "CurrentTemp", default, new CelsiusFahrenheitConverter());

            if (Settings.BeverageSettings != -1)// So default opening no longer uses a drink that does not exist
            {
                // Set it's Monitored Celsius value to the value from the Settings 
                svm.IsCelsius = Settings.TemperatureSettings;

                // Set the Temperature Stepper to the Max/Minimum possible
                TemperatureStepper.Maximum = 86;
                TemperatureStepper.Minimum = -30;

                // Set the temperature of the StatusViewModel to the current preferred beverage temperature
                svm.Temperature = preferredBeverage.Temperature;

                // is we are currently set to Celsius,
                if (svm.IsCelsius)
                {   // Set the Steppers to Min/Max for Celsius,
                    TemperatureStepper.Minimum = -30;
                    TemperatureStepper.Maximum = 30;
                }
                else
                {   // Otherwise set the Min/Max to Fahrenheit
                    TemperatureStepper.Minimum = -22;
                    TemperatureStepper.Maximum = 86;
                }
                //  Update the binding context to equal the new StatusViewModel
                BindingContext = svm;
            }

            else
            {   // Otherwise set the Min/Max to Fahrenheit
                TemperatureStepper.Minimum = -22;
                TemperatureStepper.Maximum = 86;
            }
            //  Update the binding context to equal the new StatusViewModel
            BindingContext = svm;

            LogInOutButton();




            if(Settings.NotificationSettings)
            {
                ShowIdealTempMode();
            }
            else
            {
                ShowTimerMode();
            }        

}

        /// <summary>
        /// This method will replace the Log In / Log Out button 
        /// </summary>
        private void LogInOutButton()
        {
            // Remove the Log in/out button
            ToolbarItems.RemoveAt(2);

            // If there is no current user signed in
            if (Settings.CurrentUserEmail.Length == 0)
            {
                // Create a new ToolBar Button
                ToolbarItem SignInButton = new ToolbarItem
                {   // Assign it the properties below
                    AutomationId = "SignIn",
                    Text = "Sign In",
                    // Set the menu button to the sub-menu
                    Order = ToolbarItemOrder.Secondary
                };
                // Add the button to the menu
                ToolbarItems.Add(SignInButton);
            }
            else // A user is signed in
            {   // Create a new toolbar button caled Sign Out
                ToolbarItem SignOutButton = new ToolbarItem
                {   // Assign it the properties below
                    AutomationId = "SignOut",
                    Text = "Sign Out",
                    // Set the menu button to the sub-menu
                    Order = ToolbarItemOrder.Secondary
                };
                // Add the button to the menu
                ToolbarItems.Add(SignOutButton);
            }
            // Add the click event handler to the button
            ToolbarItems.ElementAt(2).Clicked += SignInOut_Clicked;
        }

        /// <summary>
        /// This method is called when the Settings Menu Button is Clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Clicked(object sender, EventArgs e)
        {
            // Push a new settings modal
            Navigation.PushModalAsync(new NavigationPage(new SettingsMenu()));
        }

        /// <summary>
        /// This method is called when the Sign In or Out button is clicked.
        /// </summary>
        private async void SignInOut_Clicked(object sender, EventArgs e)
        {
            // Push a new login page modal
            await Navigation.PushModalAsync(new CredentialSelectPage());
        }

        /// <summary>
        /// This method is called when the Friends button is clicked.
        /// </summary>
        private async void Friends_Clicked(object sender, EventArgs e)
        {
            // Push a new friends modal
            await Navigation.PushModalAsync(new NavigationPage(new FriendsListModal()));
        }

        /// <summary>
        /// This method is a public method for calling a pages "OnAppearing()" method.
        /// Used for Signing In and Signing out, refreshing the menu. 
        /// </summary>
        public void ReAppearing()
        {
            OnAppearing();
        }


        /// <summary>
        /// Event Handler for when a Beverage is being shared
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShareButtonClicked(object sender, EventArgs e)
        {
            // This might have to be a Task instead, or this runs the task. (as below)
            // I think I should be able to make the Button just run a "Command" instead of a click handler and task. Possibly redundant this way.
            // Will need to research more into Xamarin.Essentials.Share
            // https://docs.microsoft.com/en-us/xamarin/essentials/share?context=xamarin%2Fxamarin-forms&tabs=android&WT.mc_id=docs-forums-jamont
            //Task.Run(()=>ShareBeverage());

            //CrossShare.Current.Share(new ShareMessage()
            //{
            //    Title = "Title To Share",
            //    Text = "Text To Share",
            //    Url = currentBeverage.ImageURL
            //});
            Task.Run(()=>ShareLink());
            //Task.Run(()=>ShareBeverage());

        }

        private async Task ShareLink()
        {
            // Get the Beverage Name
            string bevName = currentBeverage.Name;
            // Get the Target Temp``
            double bevTemp = this.TemperatureStepper.Value;
            // Build a string such as "I just enjoyed a %s at a perfect %d degrees!
            string shareOutput = "I just enjoyed a " + bevName + " at a perfect " + bevTemp.ToString() + Degree.Text;

            FacebookShareLinkContent linkContent = new FacebookShareLinkContent(shareOutput, new Uri("http://play.google.com/store/apps/details?id=com.levismedia.beer"));
            var ret = await CrossFacebookClient.Current.ShareAsync(linkContent);
        }

        #region Story 51
        /// <summary>
        /// This task will create a Share Request. It will use the current Beverage and other elements on the screen to send data.
        /// </summary>
        private async Task ShareBeverage()
        {
            // Get the Beverage Name
            string bevName = currentBeverage.Name;
            // Get the Target Temp
            double bevTemp = this.TemperatureStepper.Value;
            // Build a string such as "I just enjoyed a %s at a perfect %d degrees!
            string shareOutput = "I just enjoyed a " + bevName + " at a perfect " + bevTemp.ToString() + Degree.Text;

            /* Couldn't get to work */
            //Uri uri = new Uri("@https://www.liquormarts.ca/sites/mlcc_public_website/files/styles/product_fullsize/public/product/17021_product_front.png?itok=Iph2EdJx");
            //FacebookSharePhoto photo = new FacebookSharePhoto("Caption Here", uri);
            //FacebookSharePhoto[] photos = new FacebookSharePhoto[] { photo };
            //FacebookSharePhotoContent photoContent = new FacebookSharePhotoContent(photos);
            //FacebookResponse<Dictionary<string, object>> response = await CrossFacebookClient.Current.ShareAsync(photoContent);


            /* Couldn't get to work */
            //byte[] byteData;
            //byteData = System.IO.File.ReadAllBytes(currentBeverage.ImageURL);

            //FacebookSharePhoto photo = new FacebookSharePhoto(shareOutput, byteData);
            //FacebookSharePhoto[] photos = new FacebookSharePhoto[] { photo };
            //FacebookSharePhotoContent photoContent = new FacebookSharePhotoContent(photos);
            //FacebookResponse<Dictionary<string, object>> response = await CrossFacebookClient.Current.ShareAsync(photoContent);

            // Does not work to Facebook App
            var share = new ShareTextRequest()
            {
                Title = shareOutput,
                Text = shareOutput,     // Shows up in messenger                // Shows up in FB lite if URI not enabled
                Subject = shareOutput//,  // Shows up in messenger                // Shows up in FB lite if URI not enabled
                //Uri = currentBeverage.ImageURL      // Shows up in messenger if valid URL   // Shows up in FB lite
            };

            /*DOES NOT WORK */ // Possibly because there are no files on SD card?
                               //var share2 = new ShareFileRequest
                               //{
                               //    File = new ShareFile(currentBeverage.ImageURL),
                               //    Title = shareOutput
                               //};

            //await Share.RequestAsync(share);

            // Works with FB Lite + Messenger, not full FB.
            await Share.RequestAsync(shareOutput, shareOutput);

            //await Share.RequestAsync(new ShareTextRequest
            //{
            //   // Uri = currentBeverage.ImageURL,
            //    Text = shareOutput,
            //    Title = bevName,
            //    Subject = shareOutput
            //});
            ////}
            //catch // IF URI is invalid
            //{
            //    await Share.RequestAsync(new ShareTextRequest
            //    {
            //        Text = shareOutput,
            //        Title = bevName,
            //        Subject = shareOutput
            //        //PresentationSourceBounds = // Only Used on iOS
            //    });
            //}
        }
        #endregion



        /// <summary>
        /// Event Handler for clicking the Favorite Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FavouriteButtonClicked(object sender, EventArgs e)
        {
            // If the beverage is favourited.
            if(preferredBeverage.Favourite)
            {
                // Remove as a favourite.
                preferredBeverage.Favourite = false;
                // Change the FavouriteButton image source.
                FavouriteButton.Source = "NotFavourite";
            }
            // If the beverage is favourited.
            else
            {
                // If the user has less than 5 favourited beverages.
                if(svm.Context.Preference.Where(p => p.Favourite == true).Count() < 5)
                {
                    // Add as a favourite.
                    preferredBeverage.Favourite = true;
                    // Change the FavouriteButton image source.
                    FavouriteButton.Source = "Favourite";
                }
                // If the user has 5 favourited beverages. Limit on number of favourites can be changed at any time.
                else
                {
                    // Display a toast to the user, works for Android and iOS.
                    DependencyService.Get<IToastHandler>().LongToast("You cannot have any more favourites");
                }
            }

            // Update the preferredBeverage as its favourite boolean may have changed.
            svm.Context.Preference.Update(preferredBeverage);
            // Saving changes is necessary as the EntityFramework tracking doesn't seem dependable.
            svm.Context.SaveChanges();
        }

        #region story38_user_is_sent_current_temperature_notification_based_on_timer
        //The following two methods handle switching between the two methods of beverage tracking: preferred temperature and timer-based
        //Switch to displaying the timer settings.
        private void ShowTimerMode()
        {
            PreferredTempControls.IsVisible = false;
            TimeModeControls.IsVisible = true;
        }

        //Switch to displaying the preferred temp controls.
        private void ShowIdealTempMode()
        {
            TimeModeControls.IsVisible = false;
            PreferredTempControls.IsVisible = true;
        }

        //Event-handler for when the start timer button is clicked
        private void StartTimerClicked(object sender, EventArgs e)
        {

            //Used to keep track of total time
            double totalTime = 0;
            //Converts the picker numbers from the screen into milliseconds
            totalTime += Convert.ToDouble(hourPicker.SelectedItem) * 3600000;
            totalTime += Convert.ToDouble(minutePicker.SelectedItem) * 60000;
            totalTime += Convert.ToDouble(secondPicker.SelectedItem) * 1000;

            //Starts the timer using the total time passed in from the time pickers
            svm.StartTimer(totalTime);



        }

        //This method will cancel the timer and reset the pickers back to 0
        private void OnCancelTimerButtonClicked(object sender, EventArgs e)
        {
            //Set the index of each picker to 0 (0 seconds, 0 minutes, 0 hours)
            //hourPicker.SelectedIndex = 0;
            //minutePicker.SelectedIndex = 0;
            //secondPicker.SelectedIndex = 0;

            //Cancel the timer
            svm.CancelTimer();
        }
    }
    #endregion
}

