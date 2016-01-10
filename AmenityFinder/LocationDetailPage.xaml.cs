using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using AmenityFinder.models;
using AmenityFinder.utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AmenityFinder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationDetailPage : Page
    {
        private Location _location;
        private bool _changedNewPicture = false;
        private ObservableCollection<Picture> _locationPictures = new ObservableCollection<Picture>();
        private ObservableCollection<Post> _locationPosts = new ObservableCollection<Post>();
        private byte[] _pixels;

        public LocationDetailPage()
        {
            this.InitializeComponent();

            if (!Core.IsUserLoggedIn())
            {
                AddNewReviewButton.Visibility = Visibility.Collapsed;
                FlagLocationButton.Visibility = Visibility.Collapsed;
                AddPhotoInLocationButton.Visibility = Visibility.Collapsed;
            }

            CollapsePictureTab();
            ReopenReviewTab();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var location = (Location) e.Parameter;

            if (location == null)
            {
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
                return;
            }

            LocationTypeIcon.Source = location.GetLocationIcon();
            LocationName.Text = location.Name;
            LocationCoordinates.Text = location.GetCoordinatesAsString();
            Rating.Text = location.GetRating();
            IsLocationFree.Text = location.GetPriceText();

            _location = location;

            GetLocationPosts();
            GetLocationPictures();

            base.OnNavigatedTo(e);
        }

        private async void GetLocationPosts()
        {
            
            var paginatedPosts = await Requests.GetPaginatedPosts(_location);
            _locationPosts.Clear();
            foreach (var post in paginatedPosts.Results)
            {
                _locationPosts.Add(post);
            }
        }

        private async void GetLocationPictures()
        {
            
            var paginatedPictures = await Requests.GetPaginatedPictures(_location);
            _locationPictures.Clear();
            foreach (var picture in paginatedPictures.Results)
            {
                _locationPictures.Add(picture);
            }
        }

        private void CollapseReviewTab()
        {
            ReviewListView.Visibility = Visibility.Collapsed;
            AddNewReviewButton.Visibility = Visibility.Collapsed;
        }

        private void ReopenReviewTab()
        {
            ReviewListView.Visibility = Visibility.Visible;
            if (Core.IsUserLoggedIn()) AddNewReviewButton.Visibility = Visibility.Visible;
        }

        private void CollapsePictureTab()
        {
            PictureListView.Visibility = Visibility.Collapsed;
            AddPhotoInLocationButton.Visibility = Visibility.Collapsed;
        }

        private void ReopenPictureTab()
        {
            PictureListView.Visibility = Visibility.Visible;
            if (Core.IsUserLoggedIn()) AddPhotoInLocationButton.Visibility = Visibility.Visible;
        }

        private void ReviewTab_Clicked(object sender, RoutedEventArgs routedEventArgs)
        {
            CollapsePictureTab();
            ReopenReviewTab();
        }

        private void GalleryTab_Clicked(object sender, RoutedEventArgs routedEventArgs)
        {
            CollapseReviewTab();
            ReopenPictureTab();
        }

        private void CollapseNavigation()
        {
            GetDirectionButton.Visibility = Visibility.Collapsed;
        }

        private void ReopenNavigation()
        {
            GetDirectionButton.Visibility = Visibility.Visible;
        }

        private void CollapseFlagButton()
        {
            FlagLocationButton.Visibility = Visibility.Collapsed;
        }

        private void ReopenFlagButton()
        {
            FlagLocationButton.Visibility = Visibility.Visible;
        }

        private async void UpvotePost(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var post = button.DataContext as Post;
            if (post != null) await post.Upvote();
            button.DataContext = post;
        }

        private async void DownvotePost(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var post = button.DataContext as Post;
            if (post != null) await post.Downvote();
            button.DataContext = post;
        }

        private async void GetDirectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            var bingMaps = new Uri(string.Format("bingmaps:?rtp=~pos.{0}_{1}", _location.Latitude, _location.Longitude));
            await Launcher.LaunchUriAsync(bingMaps);
        }

        private async void NewPicture_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".svg");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var stream = await file.OpenReadAsync();

                var decoder = await BitmapDecoder.CreateAsync(stream);
                using (var encoderStream = new InMemoryRandomAccessStream())
                {
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(encoderStream, decoder);
                    var newHeight = 256u;
                    var aspectRatio = ((float) decoder.PixelWidth)/decoder.PixelHeight;
                    var newWidth = (uint) (newHeight*aspectRatio);

                    encoder.BitmapTransform.ScaledHeight = newHeight;
                    encoder.BitmapTransform.ScaledWidth = newWidth;

                    await encoder.FlushAsync();

                    _pixels = new byte[newWidth*newHeight*4];

                    await encoderStream.ReadAsync(_pixels.AsBuffer(), (uint) _pixels.Length, InputStreamOptions.None);

                    var bitmapImage = new BitmapImage();

                    await bitmapImage.SetSourceAsync(stream);
                    NewPicture.Source = bitmapImage;
                    _changedNewPicture = true;
                }
            }
        }

        private async void AddNewReviewButton_OnClick(object sender, RoutedEventArgs e)
        {
            CollapsePictureTab();
            CollapseReviewTab();
            CollapseFlagButton();
            CollapseNavigation();

            NewPostGrid.Visibility = Visibility.Visible;
            NewPostProgressRing.Visibility = Visibility.Visible;

            var userPost = await _location.GetCurrentUserPost();

            if (userPost != null)
            {
                RatingSlider.Value = userPost.Rating;
                CommentBox.Text = userPost.Comment;
                NewPostAnonSwitch.IsOn = userPost.IsAnonymous;
                PrevPostId.Text = userPost.Id.ToString(CultureInfo.CurrentCulture);
            }

            NewPostProgressRing.Visibility = Visibility.Collapsed;

            var b = new Binding
            {
                Mode = BindingMode.OneWay,
                Source = !string.IsNullOrWhiteSpace(CommentBox.Text)
            };

            SubmitNewPost.SetBinding(VisibilityProperty, b);
            CancelNewPostAddition.Visibility = Visibility.Visible;

        }

        private void CleanNewPostAddition()
        {
            NewPostProgressRing.Visibility = Visibility.Collapsed;
            NewPostGrid.Visibility = Visibility.Collapsed;
            SubmitNewPost.Visibility = Visibility.Collapsed;
            CancelNewPostAddition.Visibility = Visibility.Collapsed;

            ReopenNavigation();
            ReopenFlagButton();
            ReopenReviewTab();

            
        }
        private async void SubmitNewPost_OnClick(object sender, RoutedEventArgs e)
        {
            var post = new NewPost
            {
                Comment = CommentBox.Text,
                Rating = (float) RatingSlider.Value,
                IsAnonymous = NewPostAnonSwitch.IsOn,
                Location = _location.Id,
                Id = Convert.ToInt32(PrevPostId.Text)
            };

            NewPostProgressRing.Visibility = Visibility.Visible;


            await post.AddNewPost();

            CleanNewPostAddition();

            GetLocationPosts();
        }

        private void AddPhotoInLocationButton_OnClick(object sender, RoutedEventArgs e)
        {
            InitializeNewPictureForm();
        }

        private void CleanNewPictureForm()
        {
            _changedNewPicture = false;
            _pixels = null;
            NewPictureGrid.Visibility = Visibility.Collapsed;
            SubmitNewPicture.Visibility = Visibility.Collapsed;
            CancelNewPictureAddition.Visibility = Visibility.Collapsed;

            ReopenNavigation();
            ReopenFlagButton();
            ReopenPictureTab();
        }

        private void InitializeNewPictureForm()
        {
            CollapsePictureTab();
            CollapseReviewTab();
            CollapseFlagButton();
            CollapseNavigation();

            NewPictureGrid.Visibility = Visibility.Visible;

            var b = new Binding
            {
                Mode = BindingMode.OneWay,
                Source = _changedNewPicture
            };

            SubmitNewPicture.SetBinding(VisibilityProperty, b);
            CancelNewPictureAddition.Visibility = Visibility.Visible;
        }

        private async void SubmitNewPicture_OnClick(object sender, RoutedEventArgs e)
        {
            if (_changedNewPicture)
            {
                NewPictureProgressRing.Visibility = Visibility.Visible;
                var base64Image = Convert.ToBase64String(_pixels);
                var newPicture = new NewPicture
                {
                    Location = _location.Id,
                    Photo = base64Image,
                    IsAnonymous = PictureAnonSwitch.IsOn
                };
                await newPicture.Upload();
                
                CleanNewPictureForm();

                GetLocationPictures();

                CollapseReviewTab();
                ReopenPictureTab();
                
            }
        }

        private void CancelNewPostAddition_OnClick(object sender, RoutedEventArgs e)
        {
            CleanNewPostAddition();
        }

        private async void DeleteUploadedPicture(object sender, RoutedEventArgs e)
        {
            var confirmDelete = await Core.CreateAndGetDialogResponse("Are you sure about deleting this?");
            if (!confirmDelete) return;

            var button = (Button) sender;
            var picture = (Picture) button.DataContext;

            if (picture != null)
            {
                await picture.Delete();
            }

            GetLocationPictures();

        }

        private async void FlagLocationButton_OnClick(object sender, RoutedEventArgs e)
        {
            await _location.FlagLocation();
        }

        private void CancelNewPictureAddition_OnClick(object sender, RoutedEventArgs e)
        {
            CleanNewPostAddition();
            CollapseReviewTab();
            ReopenPictureTab();
        }
    }
}
