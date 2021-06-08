#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class MenuDemo : Win
    {
        public MenuDemo()
        {
            InitializeComponent();
            _lv.Data = _m.AllItems.ToNl();
            _lv.ItemClick += OnSelectMi;
        }

        void OnItemClick(object sender, Mi e)
        {
            Kit.Msg(string.Format("点击菜单项：{0}", e.ID));
        }

        void OnSelectMi(object sender, ItemClickArgs e)
        {
            _fv.Data = e.Data;
        }
    }
}