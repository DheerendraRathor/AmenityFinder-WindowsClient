using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using AmenityFinder.utils;

namespace AmenityFinder.models
{
    public class Picture: AbstractModel
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public int Location { get; set; }
        public string Photo { get; set; }
        public User User { get; set; }
        public int Flags { get; set; } = 0;
        public bool IsAnonymous { get; set; } = false;
        public DateTime Created { get; set; }

        public string GetUserName => User == null ? "Anonymous" : User.GetName();

        public BitmapImage GetUserPicture => User?.Picture == null ? new BitmapImage(new Uri("ms-appx:///Assets/anon_face.png")) : new BitmapImage(new Uri(User.Picture));

        public bool IsUserTheOwner => User?.Id == UserId;

        public Visibility VisibleToUser => IsUserTheOwner ? Visibility.Visible : Visibility.Collapsed;

        public async Task Delete()
        {
            var url = string.Format(Constants.PictureDetailUrl, Id);
            var httpResponse = await Core.HttpUtils.MakeRequest(url, null, HttpUtils.HttpMethod.Delete);
            var responseData = await Core.HttpUtils.ProcessResponse(httpResponse);

        }
    }

    public class PaginatedPictures : AbstractPaginated<Picture>
    {
        
    }

    public class NewPicture: AbstractModel
    {
        public int Location { get; set; }
        public string Photo { get; set; }
        public bool IsAnonymous { get; set; }

        public async Task Upload()
        {
            var jsonData = ToJson();
            var httpResponse =
                await Core.HttpUtils.MakeRequest(Constants.PictureListUrl, jsonData, HttpUtils.HttpMethod.Post);
            var responseData = await Core.HttpUtils.ProcessResponse(httpResponse);
        }
    }

}
