#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Dt.Base;
using Dt.Core;
using Dt.Core.Rpc;
using Uno;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Sample
{
    public sealed partial class TestDemo1 : PageWin
    {
        public TestDemo1()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
            if (files.Count > 0)
                await _tran.UploadFiles(files);
            //if (files.Count > 0)
            //{
            //    List<IUploadFile> lf = new List<IUploadFile>();
            //    foreach (var file in files)
            //    {
            //        lf.Add(new UploadFile { File = file });
            //    }
            //    await AtFile.Upload(lf, CancellationToken.None);
            //}
        }
    }

    public class UploadFile : IUploadFile
    {
        /// <summary>
        /// 获取设置待上传的文件
        /// </summary>
        public StorageFile File { get; set; }

        /// <summary>
        /// 上传进度，可以为null
        /// </summary>
        public ProgressDelegate UploadProgress { get; } = null;
    }
}