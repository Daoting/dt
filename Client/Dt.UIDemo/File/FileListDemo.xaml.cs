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


namespace Dt.UIDemo
{
    public sealed partial class FileListDemo : Win
    {
        public FileListDemo()
        {
            InitializeComponent();
            LoadData();
        }

        async void LoadData()
        {
            var xml =  await CookieX.Get("FileTransDemo");
            if (!string.IsNullOrEmpty(xml))
            {
                _fl.Data = xml;
            }
            else
            {
                // Fsm中的测试文件
                _fl.Data = "[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"],[\"photo/Logon.wav\",\"Logon\",\"00:04\",384496,\"daoting\",\"2020-03-13 10:37\"],[\"photo/mov.mp4\",\"mov\",\"00:00:10 (320 x 176)\",788493,\"daoting\",\"2020-03-13 10:37\"],[\"photo/profilephoto.jpg\",\"profilephoto\",\"300 x 300 (.jpg)\",17891,\"daoting\",\"2020-03-13 10:37\"],[\"photo/文本文档.txt\",\"文本文档\",\"txt文件\",8,\"daoting\",\"2020-03-13 10:37\"],[\"photo/项目文档.docx\",\"项目文档\",\"docx文件\",13071,\"daoting\",\"2020-03-13 10:37\"],[\"photo/mov.mp4\",\"mov\",\"00:00:10 (320 x 176)\",788493,\"daoting\",\"2020-03-13 10:37\"]]";
            }
        }

        async void OnUploadFinished(object sender, bool suc)
        {
            if (suc)
                await CookieX.Save("FileTransDemo", _fl.Data);
        }

        async void OnClear(object sender, RoutedEventArgs e)
        {
            await CookieX.DelByID("FileTransDemo");
            _fl.Data = null;
        }

        void OnDelTemp(object sender, RoutedEventArgs e)
        {
            Kit.ClearCacheFiles();
        }

        void OnAddPadding(object sender, RoutedEventArgs e)
        {
            _fl.Spacing = _fl.Spacing + 4;
        }

        void OnDelPadding(object sender, RoutedEventArgs e)
        {
            _fl.Spacing = (_fl.Spacing - 4) >= 0 ? _fl.Spacing - 4 : 0;
        }
    }
}
