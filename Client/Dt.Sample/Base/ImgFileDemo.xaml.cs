using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Dt.Sample
{
    public sealed partial class ImgFileDemo : Win
    {
        public ImgFileDemo()
        {
            InitializeComponent();
            LoadImg();
        }

        async void LoadImg()
        {
            if (await Downloader.GetAndCacheFile("sys/photo/profilephoto.jpg"))
                _imgFsm.Source = await AtLocal.GetImage("profilephoto.jpg");
            _imgData.Source = await AtLocal.GetImage("profilephoto.jpg");
            _imgFsmNoCache.Source = await Downloader.GetImage("sys/photo/profilephoto.jpg");
        }
    }
}
