using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using AmenityFinder.utils;

namespace AmenityFinder.models
{
    public class Location: AbstractModel
    {

        public int Id { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Name { get; set; }
        public bool IsFree { get; set; } = true;
        public float Rating { get; set; } = 0.0F;
        //public User User { get; set; }
        public bool Male { get; set; } = true;
        public bool Female { get; set; } = true;
        public int FlagCount { get; set; } = 0;

        public string GetCoordinatesAsString()
        {
            return string.Format("{0}\u00B0, {1}\u00B0", Latitude, Longitude);
        }

        public string GetRating()
        {
            return string.Format("{0:0.0}", Rating);
        }

        public string GetPriceText()
        {
            return IsFree ? "FREE" : "PAID";
        }

        public BitmapImage GetLocationIcon()
        {
            if (Male && Female) return Icons.CommonIcon;
            if (Male) return Icons.MaleIcon;
            if (Female) return Icons.FemaleIcon;
            return null;
        }

        public async Task<Post> GetCurrentUserPost()
        {
            var currentPostUrl = string.Format(Constants.LocationGetCurrentPost, Id);
            var httpResponse = await Core.HttpUtils.MakeRequest(currentPostUrl, null, HttpUtils.HttpMethod.Get);
            var responseData = await Core.HttpUtils.ProcessResponse(httpResponse);
            var post = AbstractModel.Deserialize<PostResult>(responseData);
            return post.Result;
        }

        public async Task<Location> FlagLocation()
        {
            var flagPostUrl = string.Format(Constants.LocationFlagPostUrl, Id);
            var httpResponse = await Core.HttpUtils.MakeRequest(flagPostUrl, null, HttpUtils.HttpMethod.Post);
            var responseData = await Core.HttpUtils.ProcessResponse(httpResponse);
            var location = AbstractModel.Deserialize<Location>(responseData);
            return location;
        }

    }

    public class NewLocation: AbstractModel
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Name { get; set; }
        public bool IsFree { get; set; } = true;
        public bool Male { get; set; } = true;
        public bool Female { get; set; } = true;
        public bool IsAnonymous { get; set; } = true;

    }

    public class SearchByBBox : AbstractModel
    {
        public float LatMin { get; set; }
        public float LongMin { get; set; }
        public float LatMax { get; set; }
        public float LongMax { get; set; }
    }

    public class SearchByBBoxResult : AbstractModel
    {
        public List<Location> Results { get; set; } 
    }

}
