using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

            base.OnNavigatedTo(e);
        }

        private void CancelNewLocationAddition(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void AddNewLocation_Click(object sender, RoutedEventArgs e)
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

            AddCommand.IsTapEnabled = false;
            CancelCommand.IsTapEnabled = false;

            Requests.AddLocation(newLocation);

        }
    }
}
