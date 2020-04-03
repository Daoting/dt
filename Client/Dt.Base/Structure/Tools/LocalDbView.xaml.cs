#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base.Tools
{
    /// <summary>
    /// 系统工具列表
    /// </summary>
    public sealed partial class LocalDbView : Win
    {
        public LocalDbView()
        {
            InitializeComponent();
            Title = "本地库";
            _lv.Data = new Nl<CenterInfo>
            {
                new CenterInfo(Icons.分组, "状态库", typeof(StateDb), "State.db"),
                new CenterInfo(Icons.详细, "模型库", typeof(ModelDb), "xxxx.db"),
                new CenterInfo(Icons.排列, "状态库备份", typeof(StateDbBackup), "将数据文件State.db复制到指定位置"),
                new CenterInfo(Icons.排列, "状态库恢复", typeof(StateDbUpdate), "选择加载已备份的数据文件恢复到当前系统"),
            };
        }
    }
}
