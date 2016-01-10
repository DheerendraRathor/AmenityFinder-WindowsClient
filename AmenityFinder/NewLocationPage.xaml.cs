using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AmenityFinder.models;
using AmenityFinder.utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AmenityFinder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewLocationPage : Page
    {
        public NewLocationPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var newLocation = (NewLocation) e.Parameter;

            if (newLocation == null) return;

            Latitude.Text = newLocation.Latitude.ToString(CultureInfo.CurrentCulture);
            Longitude.Text = newLocation.Longitude.ToString(CultureInfo.CurrentCulture);

            if (!string.IsNullOrWhiteSpace(newLocation.Name))
            {
                LocationName.Text = newLocation.Name;
            }
            else
            {
                AddCommand.Visibility = Visibility.Collapsed;
            }

            base.OnNavigatedTo(e);
        }

        private void CancelNewLocationAddition(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void AddNewLocation_Click(object sender, RoutedEventArgs e)
        {
            var newLocation = new NewLocation
            {
                Name = LocationName.Text,
                Longitude = Convert.ToSingle(Longitude.Text),
                Latitude = Convert.ToSingle(Latitude.Text),
                IsFree = IsFree.IsOn,
                Male = IsForMale.IsOn,
                Female = IsForFemale.IsOn,
                IsAnonymous = IsAnonymous.IsOn
            };

            ProgressRingGrid.Visibility = Visibility.Visible;
            CommandBar.Visibility = Visibility.Collapsed;
            await Requests.AddLocation(newLocation);
            ProgressRingGrid.Visibility = Visibility.Collapsed;

            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }

        }

        private void LocationName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            AddCommand.Visibility = string.IsNullOrWhiteSpace(LocationName.Text) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
