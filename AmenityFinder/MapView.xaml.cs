using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    public sealed partial class MapView : Page
    {
        private readonly ResourceLoader keysLoader = ResourceLoader.GetForCurrentView(Constants.KeyFile);
        private Geolocator _geolocator = null;
        private MapIcon _userLocation;
        private MapIcon _newLocationIcon = new MapIcon();
        private bool _firstAdded = false;
        private IAsyncOperation<MapLocationFinderResult> _futureResult;
        private AppViewBackButtonVisibility previousAppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

        public MapView()
        {
            this.InitializeComponent();
            MapControl.MapServiceToken = keysLoader.GetString(Constants.MapServiceTokenKeyName);
            MapService.ServiceToken = MapControl.MapServiceToken;
            
            _userLocation = new MapIcon();
            _userLocation.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/UserLocation.png"));
            _userLocation.Title = "Your Location";
            _userLocation.NormalizedAnchorPoint = new Point(0.5, 0.5);

        }

        private async void UpdateAddressBar(string msg, Visibility visiblity = Visibility.Visible)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AddressBar.Text = msg;
                AddressBarGrid.Visibility = visiblity;
            });
        }

        private void InitializeFuture()
        {
            if (_futureResult == null)
            {
                return;
            }

            _futureResult.Completed = (info, status) =>
            {
                switch (status)
                {
                    case AsyncStatus.Completed:
                        var results = info.GetResults();
                        if (results.Status == MapLocationFinderStatus.Success)
                        {
                            var locations = results.Locations;
                            if (locations.Count > 0)
                            {
                                UpdateAddressBar(locations[0].Address.FormattedAddress);
                            }
                            else
                            {
                                UpdateAddressBar("Unable to determine location");
                            }
                        }
                        else
                        {
                            UpdateAddressBar("Unable to determine location");
                        }
                        break;
                    case AsyncStatus.Error:
                        UpdateAddressBar("Unable to determine location");
                        break;
                    case AsyncStatus.Canceled:
                        UpdateAddressBar("", Visibility.Collapsed);
                        break;
                    case AsyncStatus.Started:
                        UpdateAddressBar("Finding address...");
                        break;
                }
            };
        }

        private async void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateLocationData(e.Position);
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateLocation();
        }

        private void SearchByBBoxTaskCompletion(Task<SearchByBBoxResult> searchByBBoxResult)
        {
            SearchByBBoxResult result = searchByBBoxResult.Result;
            foreach (var location in result.Results)
            {
                bool a = true;
            }
        }

        public async void UpdateLocation()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    Geolocator geolocator = new Geolocator();
                    geolocator.PositionChanged += OnPositionChanged;
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    UpdateLocationData(pos);
                    MapControl.MapElements.Add(_userLocation);
                    MapControl.Center = pos.Coordinate.Point;
                    break;
                case GeolocationAccessStatus.Denied:
                    break;
                case GeolocationAccessStatus.Unspecified:
                    break;
            }
        }

        private void UpdateLocationData(Geoposition pos)
        {
            _userLocation.Location = pos.Coordinate.Point;
        }

        private SearchByBBox GetBoundingBox()
        {
            Geopoint bottomLeft, topRight;
            var w2 = MapControl.ActualWidth;
            var h2 = MapControl.ActualHeight;

            try
            {
                MapControl.GetLocationFromOffset(new Point(0, 0), out bottomLeft);
            }
            catch (ArgumentException)
            {
                MapControl.GetLocationFromOffset(new Point(0, h2), out bottomLeft);
                bottomLeft = new Geopoint(new BasicGeoposition() {Latitude = -90, Longitude = bottomLeft.Position.Longitude});
            }

            try
            {
                MapControl.GetLocationFromOffset(new Point(w2, h2), out topRight);
            }
            catch (ArgumentException)
            {
                MapControl.GetLocationFromOffset(new Point(w2, 0), out topRight);
                topRight = new Geopoint(new BasicGeoposition() {Latitude = 90, Longitude = topRight.Position.Longitude});
            }

            var bbox = new SearchByBBox
            {
                LatMin = (float) bottomLeft.Position.Latitude,
                LongMin = (float) bottomLeft.Position.Longitude,
                LatMax = (float) topRight.Position.Latitude,
                LongMax = (float) topRight.Position.Longitude
            };

            return bbox;

        }

        private void NewLocation_Click(object sender, RoutedEventArgs e)
        {
            AddNewButton.Icon = new SymbolIcon(Symbol.Forward);
            AddNewButton.Label = "Continue";
            AddNewButton.Click -= NewLocation_Click;
            AddNewButton.Click += ContinueAddingNewLocation;

            MapIconButton.Visibility = Visibility.Collapsed;
              
            MapControl.MapTapped += PinLocationOnMap;

            previousAppViewBackButtonVisibility =
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += CancelNewLocation;

        }

        private void PinLocationOnMap(Object sender, MapInputEventArgs e)
        {
            Geopoint touchedPoint;
            
            MapControl.GetLocationFromOffset(new Point(e.Position.X, e.Position.Y), out touchedPoint);
            _newLocationIcon.Location = touchedPoint;

            _futureResult =  MapLocationFinder.FindLocationsAtAsync(touchedPoint);
            InitializeFuture();

            if (!_firstAdded)
            {
                _firstAdded = true;
                _newLocationIcon.Image =
                    RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/pin_25.png"));
                _newLocationIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                MapControl.MapElements.Add(_newLocationIcon);
            }
        }

        private void CancelNewLocation(object sender, BackRequestedEventArgs e)
        {
            _futureResult?.Cancel();

            AddNewButton.Icon = new SymbolIcon(Symbol.Add);
            AddNewButton.Label = "New Location";
            AddNewButton.Click -= ContinueAddingNewLocation;
            AddNewButton.Click += NewLocation_Click;

            MapIconButton.Visibility = Visibility.Visible;

            MapControl.MapTapped -= PinLocationOnMap;
            MapControl.MapElements.Remove(_newLocationIcon);
            _firstAdded = false;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                previousAppViewBackButtonVisibility;
            SystemNavigationManager.GetForCurrentView().BackRequested -= CancelNewLocation;
            e.Handled = true;

        }


        private void CenterMapOnUserPosition(object sender, TappedRoutedEventArgs e)
        {
            if (_userLocation.Location != null)
            {
                MapControl.Center = _userLocation.Location;
            }
        }

        private void ContinueAddingNewLocation(object sender, RoutedEventArgs e)
        {
            if (!_firstAdded) return;

            var newLocation = new NewLocation
            {
                Name = AddressBar.Text,
                Latitude = (float) _newLocationIcon.Location.Position.Latitude,
                Longitude = (float) _newLocationIcon.Location.Position.Longitude
            };

            this.Frame.Navigate(typeof (NewLocationPage), newLocation);
        }

        private void MapCameraChanged(MapControl mapControl, MapActualCameraChangedEventArgs args)
        {
            var boundingBox = GetBoundingBox();
            var searchByBboxResult = Requests.GetLocationByBBox(boundingBox);
            searchByBboxResult.ContinueWith(SearchByBBoxTaskCompletion);
        }
    }
}
