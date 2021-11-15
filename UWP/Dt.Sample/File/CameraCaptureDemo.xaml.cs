using Dt.Base;
using Dt.Core;
using System;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Dt.Sample
{
    public sealed partial class CameraCaptureDemo : Win
    {
        public CameraCaptureDemo()
        {
            InitializeComponent();
        }

        async void OnTakePhoto(object sender, RoutedEventArgs e)
        {
            CapturePhotoOptions op = new CapturePhotoOptions();
            if ((bool)_cbFront.IsChecked)
                op.UseFrontCamera = true;
            var fd = await Kit.TakePhoto(op);
            if (fd != null)
                _img.Source = new BitmapImage(new Uri(fd.FilePath));
            else
                _img.Source = null;
        }

        async void OnTakeVideo(object sender, RoutedEventArgs e)
        {
            var op = new CaptureVideoOptions();
            if ((bool)_cbFront.IsChecked)
                op.UseFrontCamera = true;
            if ((bool)_cbSpan.IsChecked)
                op.DesiredLength = TimeSpan.FromSeconds(6);

            var fd = await Kit.TakeVideo(op);
            if (fd != null)
            {
                _mp.Source = MediaSource.CreateFromUri(new Uri(fd.FilePath));
                if (!string.IsNullOrEmpty(fd.ThumbPath))
                    _img.Source = new BitmapImage(new Uri(fd.ThumbPath));
                else
                    _img.Source = null;
            }
            else
            {
                _mp.Source = null;
                _img.Source = null;
            }
        }
    }
}
