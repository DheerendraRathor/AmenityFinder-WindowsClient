using Windows.Storage;
using AmenityFinder.models;

namespace AmenityFinder.utils
{
    public class Requests
    {
        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public void GetAuthToken(string accessToken)
        {
            //TODO: Write logic for getting auth token and update local settings
            string authToken = "dummy";
            _localSettings.Values[Constants.UserTokenName] = authToken;
        }
        
        public 

    }
}
