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
    public sealed partial class CrossHome : Win
    {
        public CrossHome()
        {
            InitializeComponent();
            _lv.Data = new List<CenterInfo>
            {
                new CenterInfo(Icons.保存, "文件选择", typeof(FilePickerDemo), "文件类型过滤、单选、多选"),
                new CenterInfo(Icons.日历, "上传下载", typeof(FileTransDemo), "跨平台文件上传下载"),
            };
        }
    }
}
