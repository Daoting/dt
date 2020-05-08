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
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class FileHome : Win
    {
        public FileHome()
        {
            InitializeComponent();
            _lv.Data = new Nl<CenterInfo>
            {
                new CenterInfo(Icons.保存, "文件选择", typeof(FilePickerDemo), "文件类型过滤、单选、多选"),
                new CenterInfo(Icons.保存, "拍照录像", typeof(CameraCaptureDemo), "拍照、录像生成文件"),
                new CenterInfo(Icons.日历, "FileList上传下载", typeof(FileListDemo), "跨平台文件上传下载"),
                new CenterInfo(Icons.文件, "文件格", typeof(FileCellDemo), "文件格、图像格"),
                new CenterInfo(Icons.文件夹, "Lv的文件列表", typeof(FileLvDemo), "文件格、图像格"),
                new CenterInfo(Icons.图片, "图像资源", typeof(ImgFileDemo), "不同类型图像资源的显示"),
            };
        }
    }
}
