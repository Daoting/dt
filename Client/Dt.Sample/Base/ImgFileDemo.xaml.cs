﻿using System;
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
            await ImgKit.LoadImage(AtUser.DefaultPhoto, _imgFsm);
            _imgData.Source = await ImgKit.GetLocalImage("profilephoto.jpg");
            _imgFsmNoCache.Source = await Downloader.GetImage("photo/profilephoto.jpg");
        }
    }
}
