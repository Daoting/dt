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
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Sample
{
    public sealed partial class CustomSearchForm : UserControl, IRptSearchForm
    {
        readonly RptInfo _info;
        Menu _menu;

        public CustomSearchForm(RptInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            _fv.Data = _info.BuildParamsRow();
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<RptInfo> Query;

        /// <summary>
        /// 查询面板菜单
        /// </summary>
        public Menu Menu
        {
            get
            {
                if (_menu == null)
                {
                    _menu = new Menu();
                    Mi mi = new Mi { ID = "查询", Icon = Icons.搜索 };
                    mi.Click += OnQuery;
                    _menu.Items.Add(mi);
                    mi = new Mi { ID = "重置", Icon = Icons.撤消 };
                    mi.Click += OnResetParams;
                    _menu.Items.Add(mi);
                    _menu.Items.Add(new Mi { ID = "自定义", Icon = Icons.两人 });
                }
                return _menu;
            }
        }

        void OnQuery(object sender, Mi e)
        {
            _info.UpdateParams(_fv.Row);
            Query?.Invoke(this, _info);
        }

        void OnResetParams(object sender, Mi e)
        {
            _fv.Row.RejectChanges();
        }
    }
}
