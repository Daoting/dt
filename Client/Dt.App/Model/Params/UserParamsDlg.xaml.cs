#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Model
{
    public sealed partial class UserParamsDlg : Dlg
    {
        public UserParamsDlg()
        {
            InitializeComponent();
        }

        public async void Show(string p_paramid)
        {
            Title = p_paramid;
            _lv.Data = await AtCm.Query("参数-用户设置", new { paramid = p_paramid });
            if (!Kit.IsPhoneUI)
            {
                Width = 500;
                Height = 400;
            }
            await ShowAsync();
        }
    }
}
