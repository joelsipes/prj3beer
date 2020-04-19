using prj3beer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace prj3beer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FriendsListModal : ContentPage
    {
        public List<Friend> Friends;
        public FriendsListModal()
        {
            InitializeComponent();

            // By default, this message should show
            Message.IsVisible = true;
            // Check to see if signed in.
            if (Settings.CurrentUserEmail.Length != 0)
            {
                // If Signed In, hide Message, load data into ListView
                Message.IsVisible = false;

                // Load all friends to this List
                Friends = App.Social.Friend.OrderBy(x => x.Name).ToList<Friend>();
                // TODO: Sort This List
                
            }

            // Set the listview's item source to the list of Friends
            FriendsList.ItemsSource = Friends;
        }
        //Closes the modal
        async private void Close_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}