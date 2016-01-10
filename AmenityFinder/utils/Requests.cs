using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using AmenityFinder.models;
using Newtonsoft.Json;

namespace AmenityFinder.utils
{
    public static class Requests
    {
        private static ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private static readonly HttpUtils HttpUtils = new HttpUtils();

        public static async Task GetAuthToken(string accessToken)
        {
            var accessTokenModel = new AccessTokenModel {AccessToken = accessToken};
            var httpResponseMessage =
                await HttpUtils.MakeRequest(Constants.AccountLoginUrl, accessTokenModel.ToJson(), HttpUtils.HttpMethod.Post);
            var responseData = await HttpUtils.ProcessResponse(httpResponseMessage);
            var authModel = JsonConvert.DeserializeObject<AuthenticationResponseModel>(responseData);
            _localSettings.Values[Constants.UserTokenName] = authModel.Token;
            _localSettings.Values[Constants.UidFieldName] = authModel.Uid;

        }

        public static async Task<Location> AddLocation(NewLocation newLocation)
        {
            var jsonData = newLocation.ToJson();
            var httpResponseMessage = await HttpUtils.MakeRequest(Constants.LocationListUrl, jsonData, HttpUtils.HttpMethod.Post);
            var responseData = await HttpUtils.ProcessResponse(httpResponseMessage);
            var location = JsonConvert.DeserializeObject<Location>(responseData);
            return location;
        }

        public static async Task<SearchByBBoxResult> GetLocationByBBox(SearchByBBox bbox)
        {
            var jsonData = bbox.ToJson();
            var httpResponseMessage =
                await HttpUtils.MakeRequest(Constants.LocationSearchByBBoxUrl, jsonData, HttpUtils.HttpMethod.Post);
            var responseData = await HttpUtils.ProcessResponse(httpResponseMessage);
            var locationList = JsonConvert.DeserializeObject<SearchByBBoxResult>(responseData, new JsonSerializerSettings {ContractResolver = new SnakeCasePropertyNamesContractResolver()});
            return locationList;
        }

        public static async Task<PaginatedPosts> GetPaginatedPosts(Location location)
        {
            var urlString = string.Format(Constants.LocationGetPostsUrl, location.Id);

            var httpResponseMessage = await HttpUtils.MakeRequest(urlString, null, HttpUtils.HttpMethod.Get);
            var responseData = await HttpUtils.ProcessResponse(httpResponseMessage);

            var paginatedPosts = AbstractModel.Deserialize<PaginatedPosts>(responseData);

            return paginatedPosts;
        }

        public static async Task<PaginatedPictures> GetPaginatedPictures(Location location)
        {
            var urlString = string.Format(Constants.LocationGetPicturesUrl, location.Id);

            var httpResponseMessage = await HttpUtils.MakeRequest(urlString, null, HttpUtils.HttpMethod.Get);
            var responseData = await HttpUtils.ProcessResponse(httpResponseMessage);

            var paginatedPictures = AbstractModel.Deserialize<PaginatedPictures>(responseData);

            return paginatedPictures;
        }

    }
}
