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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public partial class MenuDemo : Win
    {
        public MenuDemo()
        {
            InitializeComponent();
            if (!Kit.IsPhoneUI)
            {
                _lv.Data = _m.AllItems.ToNl();
                _lv.ItemClick += OnSelectMi;
            }
        }

        void OnItemClick(Mi e)
        {
            Kit.Msg(string.Format("点击菜单项：{0}", e.ID));
        }

        void OnSelectMi(ItemClickArgs e)
        {
            _fv.Data = e.Data;
        }

        void OnBtnCall()
        {
            Kit.Msg("内置按钮点击事件");
        }
    }
}