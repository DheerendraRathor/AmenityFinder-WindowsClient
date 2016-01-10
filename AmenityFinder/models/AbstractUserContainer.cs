using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace AmenityFinder.models
{
    public class AbstractUserContainer
    {
        public User User { get; set; }

        public string GetUserName => User == null ? "Anonymous" : User.GetName();

        public BitmapImage GetUserPicture => User?.Picture == null ? new BitmapImage(new Uri("ms-appx:///Assets/anon_face.png")) : new BitmapImage(new Uri(User.Picture));
    }
}
