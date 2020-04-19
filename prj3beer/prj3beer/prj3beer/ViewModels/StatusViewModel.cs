using System;
using System.ComponentModel;
using Xamarin.Forms;
using prj3beer.Views;
using prj3beer.Services;
using prj3beer.Models;
using System.Timers;

namespace prj3beer.ViewModels
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        BeerContext context = new BeerContext();

        static Timer notificationTimer;

        //Instance of the notifications class.  Handles checking conditions for and the sending of notifications.
        Notifications notifications;

        //Temperature being read from external source
        double currentTemp;

        //Tracks whether or not a timer is active
        private bool activeTimer = false;

        //This boolean will control whether or not the timer
        //responsible for the current temperature mock is on or off
        public static bool timerOn = false;

        public BeerContext Context { get { return this.context; } }
        
        // Nullable double value for our temperature
        double? _temperature;

        double _minimum;

        double _maximum;

        /// Boolean for wether or not we are in Celsius or Fahrenheit (Default Celsius)
        public bool IsCelsius { get; set; }

        // string to return, either Degree C or Degree F based on Celsius Value
        public string Scale { get { return IsCelsius ? "\u00B0C" : "\u00B0F"; } }

        // Double for storing Minimum Values for Steppers based on Celsius/Fahrenheit
        public double Minimum { get { return _minimum; } set { _minimum = value; } }

        // Double for storing Maximum Values for Steppers based on Celsius/Fahrenheit
        public double Maximum { get { return _maximum; } set { _maximum = value; } }

        // Double for setting/getting values to/from our backing field (nullable double)
        public double? Temperature
        {
            // Get the value stored in the backing field
            get { return _temperature; }

            // Store the passed in value to the backing field. Do a calculation depending if we are monitoring in Fahrenheit or Celsius
            set {
                try
                {
                    _minimum = IsCelsius ? -30 : -22;
                    _maximum = IsCelsius ? 30 : 86;

                    _temperature = IsCelsius ? value : ((value * 1.8) + 32);
                    //_temperature = value;
                    
                }
                finally
                {
                    OnPropertyChanged("Temperature");
                }
            }
        }

        // Returns a string using the stored backing field, needs to be a string to show up in an Entry Field
        public string PreferredTemperatureString
        {
            // Get the temperature from the backing field, if the temperature has a value, return it's rounded value, otherwise return an empty string
            get { return _temperature.HasValue ? Math.Round(_temperature.Value).ToString() : ""; }

            // Set the value from the Entry
            set
            {
                try
                {   // store the temperature from the entry, parsed as a double. 
                    _temperature = double.Parse(value);
                    //_temperature = IsCelsius ? double.Parse(value) : double.Parse(value) * 1.8 +32;
                }
                catch
                {   // If the field is empty, set temperature to null
                    _temperature = null;
                }
                finally
                {   
                    OnPropertyChanged("PreferredTemperatureString");
                }
            }
        }

        /// <summary>
        /// Property for storing the temperature received from the bluetooth device.  Bound to a label on the Status
        /// page.
        /// </summary>
        public double CurrentTemp
        {
            set
            {
                //Has the value changed?
                if (currentTemp != value)
                {
                    currentTemp = value;

                    //Check if a notification should be sent (and send it) 

                    //MERGE QUESTION: This gave an error
                    if (StatusPage.preferredBeverage != null)
                    {
                        notifications.NotificationCheck(currentTemp, StatusPage.preferredBeverage.Temperature);
                    }
           
                    if (PropertyChanged != null)
                    {
                        //If the property has changed, fire an event.
                        OnPropertyChanged("CurrentTemp");
                    }
                }
            }
            get
            {
                return currentTemp;
            }
        }


        public bool HasActiveTimer
        {
            set
            {
                if(activeTimer != value)
                {
                    activeTimer = value;

                    if (PropertyChanged != null)
                    {
                        //If the property has changed, fire an event.
                        OnPropertyChanged("HasActiveTimer");
                    }
                }
            }
            get
            {
                return activeTimer;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public StatusViewModel()
        {
            notifications = new Notifications();

            //Checks for update temps every second.  Will eventually poll an object associated with a
            //bluetooth reading.  Currently communicates with a class bouncing between -35 and 35 degrees celsius.
            if(true)
            {
                timerOn = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    this.CurrentTemp = MockTempReadings.Temp;
                    return true;
                });
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if(changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

  

        //starts the timer taking in the timpicker time from the status page
        public void StartTimer(double selectedTime)
        {
            //If the user enters in a time greater than 0 then start the timer
            if (selectedTime > 0)
            {
                notificationTimer = new Timer(selectedTime);
                notificationTimer.Elapsed += OnTimedEvent;
                notificationTimer.AutoReset = false;
                notificationTimer.Enabled = true;
                notificationTimer.Start();
                HasActiveTimer = true;
            }
            else
            {
                //If the user presses the start button without setting a time then immediately send a notification
                notifications.SendTimeExpiredNotification(currentTemp);
            }
            //Enable cancel button, disable timepicker.
        }

        //Waits until timer hits zero and sends notification
        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            notifications.SendTimeExpiredNotification(currentTemp);
            HasActiveTimer = false;
        }

        //Cancels the timer
        public void CancelTimer()
        {
            //This will stop the times
            notificationTimer.Stop();
            //sets the active timer to false
            HasActiveTimer = false;
        }
    }
}