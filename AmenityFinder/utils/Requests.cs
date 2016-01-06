using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;
using AmenityFinder.models;
using Newtonsoft.Json;

namespace AmenityFinder.utils
{
    public static class Requests
    {
        private static ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        static HttpUtils httpUtils = new HttpUtils();

        public static async void GetAuthToken(string accessToken)
        {
            var accessTokenModel = new AccessTokenModel {AccessToken = accessToken};
            var httpResponseMessage =
                await httpUtils.MakeRequest(Constants.AccountLoginUrl, accessTokenModel.ToJson(), HttpUtils.HttpMethod.Post);
            var responseData = await httpUtils.ProcessResponse(httpResponseMessage);
            var authModel = JsonConvert.DeserializeObject<AuthenticationResponseModel>(responseData);
            _localSettings.Values[Constants.UserTokenName] = authModel.Token;
        }

        public static async Task<Location> AddLocation(NewLocation newLocation)
        {
            var jsonData = newLocation.ToJson();
            HttpResponseMessage httpResponseMessage = await httpUtils.MakeRequest(Constants.LocationListUrl, jsonData, HttpUtils.HttpMethod.Post);
            string responseData = await httpUtils.ProcessResponse(httpResponseMessage);
            Location location = JsonConvert.DeserializeObject<Location>(responseData);
            return location;
        }

        //public async Task<List<Location>> GetLocationByBBox()

    }
}
