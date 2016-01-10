using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace AmenityFinder.utils
{
    public static class Icons
    {
        public static readonly BitmapImage MaleIcon = new BitmapImage(new Uri("ms-appx:///Assets/male.png"));
        public static readonly BitmapImage FemaleIcon = new BitmapImage(new Uri("ms-appx:///Assets/female.png"));
        public static readonly BitmapImage CommonIcon = new BitmapImage(new Uri("ms-appx:///Assets/washroom.png"));
    }
}
