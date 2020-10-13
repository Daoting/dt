#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
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

namespace Dt.Base.Report
{
    public sealed partial class RptSearchForm : UserControl
    {
        readonly RptInfo _info;

        public RptSearchForm(RptInfo p_info, Tab p_tab)
        {
            InitializeComponent();
            _info = p_info;
            p_tab.Menu = CreateMenu();
            LoadCells(_info.Root, _fv);
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<RptInfo> Query;

        /// <summary>
        /// 执行查询
        /// </summary>
        internal void DoQuery()
        {
            Dict dict = new Dict();
            foreach (var cell in _fv.Row.Cells)
            {
                if (cell.Val == null || string.IsNullOrEmpty(cell.Val.ToString()))
                {
                    AtKit.Warn(string.Format("【{0}】未设置参数值！", cell.ID));
                    return;
                }
                dict.Add(cell.ID, cell.Val);
            }
            _info.Params = dict;

            // 触发Query事件
            Query?.Invoke(this, _info);
        }

        internal static void LoadCells(RptRoot p_root, Fv p_fv)
        {
            foreach (var row in p_root.Params.Data)
            {
                p_fv.Items.Add(Fv.CreateCell(row));
            }
        }

        Menu CreateMenu()
        {
            Menu menu = new Menu();
            Mi mi = new Mi { ID = "查询", Icon = Icons.搜索 };
            mi.Click += (s, args) => DoQuery();
            menu.Items.Add(mi);
            mi = new Mi { ID = "重置", Icon = Icons.撤销 };
            mi.Click += OnResetParams;
            menu.Items.Add(mi);
            return menu;
        }

        void OnResetParams(object sender, Mi e)
        {
            _fv.Row.RejectChanges();
        }
    }
}
