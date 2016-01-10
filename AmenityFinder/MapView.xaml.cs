using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
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
        private AppViewBackButtonVisibility _previousAppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        private DateTime _bboxFiringTime = DateTime.Now;
        private Dictionary<int, Location> _locationMarkers = new Dictionary<int, Location>(); 
        private Dictionary<MapIcon, int> _locationMarkerIcons = new Dictionary<MapIcon, int>();

        private Location _currentTappedLocation;

        public MapView()
        {
            this.InitializeComponent();
            MapControl.MapServiceToken = keysLoader.GetString(Constants.MapServiceTokenKeyName);
            MapService.ServiceToken = MapControl.MapServiceToken;

            _userLocation = new MapIcon
            {
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/UserLocation.png")),
                Title = "Your Location",
                NormalizedAnchorPoint = new Point(0.5, 0.5)
            };

            if (!Core.IsUserLoggedIn())
            {
                AddNewButton.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Collapsed;
            }

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
            MapCameraChanged(MapControl, null);
        }

        private void SearchByBBoxTaskCompletion(SearchByBBoxResult searchByBBoxResult)
        {
            
            foreach (var location in searchByBBoxResult.Results)
            {
                if (!_locationMarkers.ContainsKey(location.Id))
                {
                    var mapIcon = new MapIcon
                    {
                        Location = new Geopoint(new BasicGeoposition
                        {
                            Latitude = location.Latitude,
                            Longitude = location.Longitude
                        }),
                        NormalizedAnchorPoint = new Point(0.5, 1.0),
                        Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/pin_green_25.png")),
                    };
                    MapControl.MapElements.Add(mapIcon);
                    _locationMarkerIcons[mapIcon] = location.Id;
                }
                _locationMarkers[location.Id] = location;

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
            Geopoint topLeft, bottomRight;
            var width = MapControl.ActualWidth;
            var height = MapControl.ActualHeight;

            try
            {
                MapControl.GetLocationFromOffset(new Point(0, 0), out topLeft);
            }
            catch (ArgumentException)
            {
                MapControl.GetLocationFromOffset(new Point(0, height), out topLeft);
                topLeft = new Geopoint(new BasicGeoposition() {Latitude = -90, Longitude = topLeft.Position.Longitude});
            }

            try
            {
                MapControl.GetLocationFromOffset(new Point(width, height), out bottomRight);
            }
            catch (ArgumentException)
            {
                MapControl.GetLocationFromOffset(new Point(width, 0), out bottomRight);
                bottomRight = new Geopoint(new BasicGeoposition() {Latitude = 90, Longitude = bottomRight.Position.Longitude});
            }

            var bbox = new SearchByBBox
            {
                LatMax = (float) topLeft.Position.Latitude,
                LongMin = (float) topLeft.Position.Longitude,
                LatMin = (float) bottomRight.Position.Latitude,
                LongMax = (float) bottomRight.Position.Longitude
            };

            return bbox;

        }

        private void NewLocation_Click(object sender, RoutedEventArgs e)
        {
            AddNewButton.Icon = new SymbolIcon(Symbol.Forward);
            AddNewButton.Label = "Continue";
            AddNewButton.Click -= NewLocation_Click;
            AddNewButton.Click += ContinueAddingNewLocation;
            AddNewButton.Visibility = Visibility.Collapsed;

            LocationMarkerDetails.Visibility = Visibility.Collapsed;
              
            MapControl.MapTapped += PinLocationOnMap;

            _previousAppViewBackButtonVisibility =
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
                AddNewButton.Visibility = Visibility.Visible;
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

            MapControl.MapTapped -= PinLocationOnMap;
            MapControl.MapElements.Remove(_newLocationIcon);
            _firstAdded = false;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                _previousAppViewBackButtonVisibility;
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

            Frame.Navigate(typeof (NewLocationPage), newLocation);
        }

        private async void MapCameraChanged(MapControl mapControl, MapActualCameraChangedEventArgs args)
        {
            var currentTime = DateTime.Now;
            var timeElapsed = currentTime - _bboxFiringTime;
            if (timeElapsed.TotalSeconds > 1)
            {
                _bboxFiringTime = currentTime;
            }
            else
            {
                return;
            }
            var boundingBox = GetBoundingBox();
            var searchByBboxResult = await Requests.GetLocationByBBox(boundingBox);
            SearchByBBoxTaskCompletion(searchByBboxResult);
        }

        private void MapControl_OnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var clickedIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
            Location location;
            try
            {
                location = _locationMarkers[_locationMarkerIcons[clickedIcon]];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            if (_currentTappedLocation != null)
            {
                if (location.Id == _currentTappedLocation.Id)
                {
                    LocationMarkerDetails_OnTapped(MapControl, null);
                }
            }

            IsLocationFree.Text = location.GetPriceText();
            LocationTypeIcon.Source = location.GetLocationIcon();
            LocationName.Text = location.Name;
            LocationCoordinates.Text = location.GetCoordinatesAsString();
            Rating.Text = location.GetRating();
            LocationMarkerDetails.Visibility = Visibility.Visible;

            _currentTappedLocation = location;
        }

        private void LocationMarkerDetails_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof (LocationDetailPage), _currentTappedLocation);
        }

        private void LogoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            Core.LogoutUser(this, true);
        }

        private void MapControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            MapCameraChanged(MapControl, null);
        }
    }
}
