#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.App.Workflow
{
    public partial class WfDataList : Win
    {
        public WfDataList()
        {
            InitializeComponent();
            Load();
        }

        async void Load()
        {
            _lv.Data = await AtCm.Query("流程-参与的流程", new { userid = AtUser.ID });
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            if (!e.IsChanged)
                return;

            var row = e.Row;
            if (row.Tag != null)
            {
                LoadCenter(row.Tag);
                return;
            }

            var tpName = row.Str("ListType");
            Throw.IfNullOrEmpty(tpName, "流程定义中未设置列表窗口类型！");
            var type = Type.GetType(tpName);
            Throw.IfNull(type, $"列表窗口类型[{tpName}]不存在！");

        }
    }
}