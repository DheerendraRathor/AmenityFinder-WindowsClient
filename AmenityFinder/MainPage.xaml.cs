using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
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
using static AmenityFinder.utils.Requests;

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
        }

        private void CompleteFbLogin()
        {
            FButton.Click -= FbLoginButton_onClick;
            FButton.Click += NavigateToMap;
            FButton.Content = "Enter";
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            var fromFb = false;

            try
            {
                var args = (IActivatedEventArgs) e.Parameter;

                if (args?.Kind == ActivationKind.Protocol)
                {
                    fromFb = true;
                }
            }
            catch (InvalidCastException)
            {
                // Not facebook login redirect
            }
            
            if (_localSettings.Values.ContainsKey(Constants.UserTokenName))
            {
                CompleteFbLogin();
            }
            else if (fromFb)
            {
                var accessToken = _localSettings.Values[Constants.FbAccessTokenName] as string;
                if (accessToken != null)
                {
                    ProgressRingGrid.Visibility = Visibility.Visible;
                    await Requests.GetAuthToken(accessToken);
                    ProgressRingGrid.Visibility = Visibility.Collapsed;
                    CompleteFbLogin();
                }
                else
                {
                    FbFailedBlock.Visibility = Visibility.Visible;
                }
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

        private void NavigateToMap(Object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof (MapView));
        }

    }
}
