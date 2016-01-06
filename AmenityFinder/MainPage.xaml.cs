using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AmenityFinder.models;
using AmenityFinder.utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AmenityFinder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            this.InitializeComponent();
            var accessToken = _localSettings.Values[Constants.FbAccessTokenName] as string;
            if (accessToken != null)
            {
                Requests.GetAuthToken(accessToken);
            }
            else
            {
                FBFailedBlock.Visibility = Visibility.Visible;
            }
        }

        private async void FbLoginButton_onClick(object sender, RoutedEventArgs e)
        {

            await Windows.System.Launcher.LaunchUriAsync(
                new Uri(
                    "fbconnect://authorize?client_id=625784407575569&redirect_uri=msft-9cca4bcf684046e19596c45e910d77db://fbauth&scope=email"));
            var nl = new NewLocation
            {
                Latitude = 23.390F,
                Longitude = 32.32F,
                Name = "Test"
            };
            //await new Requests().AddLocation(nl);
        }
    }
}
