#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-11-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Model
{
    /// <summary>
    /// 分组菜单项页面
    /// </summary>
    [View("我的设置")]
    public sealed partial class MyParamsSetting : UserControl
    {
        public MyParamsSetting()
        {
            InitializeComponent();
            LoadVals();
        }

        async void LoadVals()
        {
            Dict dt = await AtCm.GetParams(Kit.UserID);
            Table tbl = (Table)dt["result"];
            if (tbl == null || tbl.Count == 0)
                return;

            Row row = new Row();
            foreach (var r in tbl)
            {
                row.AddCell(r.Str(0), r.Str(1));
            }
            _fv.Data = row;
            row.Changed += OnValChanged;
        }

        async void OnValChanged(object sender, Cell e)
        {
            if (await AtCm.SaveParams(Kit.UserID, e.ID, e.GetVal<string>()))
                e.AcceptChanges();
        }
    }
}
