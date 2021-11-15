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
            _nav.Data = new Nl<Nav>
            {
                new Nav("文件选择", typeof(FilePickerDemo), Icons.保存) { Desc = "文件类型过滤、单选、多选" }, 
                new Nav("拍照录像", typeof(CameraCaptureDemo), Icons.保存) { Desc = "拍照、录像生成文件" },
                new Nav("FileList上传下载", typeof(FileListDemo), Icons.日历) { Desc ="跨平台文件上传下载" },
                new Nav("文件格", typeof(FileCellDemo), Icons.文件) { Desc = "文件格、图像格" },
                new Nav("Lv的文件列表", typeof(FileLvDemo), Icons.文件夹) { Desc = "文件格、图像格" },
                new Nav("图像资源", typeof(ImgFileDemo), Icons.图片) { Desc = "不同类型图像资源的显示" },
            };
        }
    }
}
