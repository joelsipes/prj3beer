using Xamarin.Forms;
using prj3beer.Services;
using prj3beer.Views;
using prj3beer.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using prj3beer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace prj3beer
{
    public partial class App : Application
    {
        //Static BeerContext to pass between pages for database operations
        public static BeerContext Context;
        public static APIManager APIManager;
        public static SocialContext Social;
        public App()
        {
            InitializeComponent();

            //Instantiate context & API Manager
            Context = new BeerContext();
            Social = new SocialContext();
            APIManager = new APIManager();

            // Reset the local user object in settings
            ResetUser(); // Remove for persistent login 

            SetupDatabases();

            MainPage = new NavigationPage(new BeverageSelectPage());
        }

        public void SetupDatabases()
        {
            // Set the default URL of API to default
            Settings.URLSetting = default;

            StatusViewModel.timerOn = false;

            App.APIManager = new APIManager();

            App.Context = new BeerContext();

            App.Social = new SocialContext();

            // Remove after debug
            App.Social.Database.EnsureDeleted();

            App.Social.Database.EnsureCreated();

            FetchData();
        }

        public static async void FetchData()
        {
            // REMOVE FOR PERSISTENT Data
            Context.Database.EnsureDeleted();

            // Ensure the Database is Created
            Context.Database.EnsureCreated();

            // Set URL of api Manager to point to the Brands API
            // Load the Brands that Validate into the Local Storage
            List<Brand> brands = await APIManager.GetBrandsAsync();

            try
            {
                Context.Brand.AttachRange(brands);
                Context.Brand.AddRange(brands);
            }
            catch(DbUpdateException exception)
            {
                Debug.WriteLine(exception.Message);

                Context.Brand.UpdateRange(brands);
            }
            finally
            {
                Context.ChangeTracker.DetectChanges();
                Context.SaveChanges();
            }

            // Load the Beverages that Validate into the Local Storage
            List<Beverage> beverages = await APIManager.GetBeveragesAsync();

            try
            {
                Context.Beverage.AttachRange(beverages);
                Context.Beverage.AddRange(beverages);
            }
            catch (DbUpdateException exception)
            {
                Debug.WriteLine(exception.Message);

                Context.Beverage.UpdateRange(beverages);

            }
            finally
            {

                Context.ChangeTracker.DetectChanges();
                Context.SaveChanges();
            }


            // Release database resources
            //Context?.Dispose();
        }
        /// <summary>
        /// This method is used to remove the currently saved user that is stored in settings
        /// May be used by the rest when the actual Log Out process HAS to happen.
        /// </summary>
        public void ResetUser()
        {
            Settings.WelcomePromptSetting = false;
            Settings.CurrentUserEmail = "";
            Settings.CurrentUserName = "";
        }

        private void SetUpPreset()
        {
            // Set the default URL of API to default
            Settings.URLSetting = default;

            StatusViewModel.timerOn = false;

            //This was moved to improve reliability of the tests - potentially moving some time
            //MockTempReadings.StartCounting();

            // Instantiate a new Context (Database)
            BeerContext context = new BeerContext();

            //Instantiate a new API Manager
            APIManager apiManager = new APIManager();

            // Connect to the API and store Beverages/Brands in the Database
#if DEBUG
            //LoadFixtures(context);
            //FetchData(context, apiManager);
#elif RELEASE
            //Release mode breaks, but can swap these for API usage
            FetchData();
          
#endif
            //MainPage = new MainPage(context);
            //MainPage = new MainPage();
        }

        

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private async void LoadFixtures()
        {
            /*
            List<Brand> brandList = new List<Brand>();

            brandList.Add(new Brand() { BrandID = 4, Name = "Great Western Brewery" });
            brandList.Add(new Brand() { BrandID = 5, Name = "Churchhill Brewing Company" });
            brandList.Add(new Brand() { BrandID = 6, Name = "Prarie Sun Brewery" });
            brandList.Add(new Brand() { BrandID = 7, Name = new string('a', 61) });
            brandList.Add(new Brand() { BrandID = 3, Name = "" });
            // Story 24 Brand, for testing
            brandList.Add(new Brand() { BrandID = 25, Name = "Coors" });

            //ValidateBrands(brandList, context);
            */

            // Create a series of 3 new beverages with different values.
            Beverage bev1 = new Beverage { BeverageID = 1, Name = "Great Western Radler", BrandID = 1, Type = Type.Radler, Temperature = 3 };
            Beverage bev2 = new Beverage { BeverageID = 2, Name = "Great Western Pilsner", BrandID = 1, Type = (Type)4, Temperature = 13 };
            Beverage bev3 = new Beverage { BeverageID = 3, Name = "Original 16 Copper Ale", BrandID = 1, Type = (Type)5, Temperature = 2 };
            Beverage bev4 = new Beverage { BeverageID = 4, Name = "Original 16 Pale Ale", BrandID = 201, Type = (Type)5, Temperature = 2 };
            Beverage bev5 = new Beverage { BeverageID = 5, Name = "Churchill Blonde Lager", BrandID = 2, Type = (Type)4, Temperature = 3 };
            Beverage bev6 = new Beverage { BeverageID = 6, Name = "Rebellion Zilla IPA", BrandID = 3, Type = (Type)3, Temperature = 4 };
            Beverage bev7 = new Beverage { BeverageID = 7, Name = "Rebellion Amber Ale", BrandID = 3, Type = (Type)1, Temperature = 53 };
            Beverage bev8 = new Beverage { BeverageID = 0, Name = "Rebellion Lentil Beer", BrandID = 3, Type = (Type)10, Temperature = 3 };
            Beverage bev9 = new Beverage { BeverageID = 99, Name = "Rebellion Pear Beer", BrandID = 3, Type = (Type)3, Temperature = 3 };
            Beverage bev10 = new Beverage { BeverageID = 8, Name = "ThisNameIsWayTooLongAndIKnowBecauseIJustStartedTypingRandomStuffButIMadeSureToTypeItInPascalCaseXOXOLOLOLOL", BrandID = 2, Type = (Type)8, Temperature = -40 };
            Beverage bev11 = new Beverage { BeverageID = 9, Name = " ", BrandID = 0, Type = (Type)6, Temperature = 3 };

            Preference pref1 = new Preference { BeverageID = 1, Temperature = 10 };

            try
            {   // Try to Delete The Database
                await App.Context.Database.EnsureDeletedAsync();
                // Try to Create the Database
                await App.Context.Database.EnsureCreatedAsync();
                // Add Each beverage to the Database - ready to be written to the database.(watched)
                App.Context.Beverage.Add(bev1);
                App.Context.Beverage.Add(bev2);
                App.Context.Beverage.Add(bev3);
                // Story 24 Beverages, for testing
                App.Context.Beverage.Add(bev4);
                App.Context.Beverage.Add(bev5);
                App.Context.Beverage.Add(bev6);
                App.Context.Preference.Add(pref1);

                // Save Changes (updates/new) to the database
                await App.Context.SaveChangesAsync();
            }
            catch (SqliteException)
            {
                throw;
            }
        }

    }
}
