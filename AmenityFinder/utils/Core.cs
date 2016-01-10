using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace AmenityFinder.utils
{
    public static class Core
    {
        private static ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public static bool IsUserLoggedInProp => IsUserLoggedIn();

        public static bool IsUserLoggedIn()
        {
            return _localSettings.Values.ContainsKey(Constants.UserTokenName);
        }

        public static int GetUid()
        {
            if (_localSettings.Values.ContainsKey(Constants.UidFieldName))
            {
                return (int)_localSettings.Values[Constants.UidFieldName];
            }
            return -1;
        }

        public static async void LogoutUser(Page page = null, bool dialog = false)
        {
            if (dialog)
            {
                var confirmLogout = await CreateAndGetDialogResponse("Are you sure you want to logout?", "Logout Confirmation");
                if (!confirmLogout) return;
            }

            _localSettings.Values.Remove(Constants.UserTokenName);
            page?.Frame.Navigate(typeof (MainPage));
        }

        public static HttpUtils HttpUtils = new HttpUtils();

        public static async Task<bool> CreateAndGetDialogResponse(string message, string title = null)
        {
            var dialog = title == null ? new MessageDialog(message) : new MessageDialog(message, title);

            dialog.Commands.Add(new UICommand("Yes") {Id = 0});
            dialog.Commands.Add(new UICommand("No") {Id = 1});

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            return result.Label == "Yes";

        }

    }
}
