using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace Dt.MgrDemo
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
            await Kit.LoadImage(Kit.DefaultUserPhoto, _imgFsm);
            _imgFsmNoCache.Source = await Downloader.GetImage("photo/profilephoto.jpg");
        }
    }
}
