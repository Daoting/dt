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
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.App.File
{
    /// <summary>
    /// 搜索文件
    /// </summary>
    public sealed partial class SelectSearchPage : Mv
    {
        readonly SelectFileDlg _owner;

        public SelectSearchPage(SelectFileDlg p_owner)
        {
            InitializeComponent();

            _owner = p_owner;
            if (_owner.IsMultiSelection)
                _lv.SelectionMode = Base.SelectionMode.Multiple;
        }

        async void OnSearch(object sender, string e)
        {
            if (string.IsNullOrEmpty(_owner.TypeFilter))
                _lv.Data = await AtCm.Query("文件-搜索所有文件", new { name = $"%{e}%", userid = Kit.UserID });
            else
                _lv.Data = await AtCm.Query("文件-搜索扩展名文件", new { name = $"%{e}%", userid = Kit.UserID, extname = _owner.TypeFilter });
        }

        void OnSelect(object sender, Mi e)
        {
            if (_lv.SelectedCount == 0)
                return;

            List<string> ls = new List<string>();
            foreach (var row in _lv.SelectedRows)
            {
                string info = row.Str("Info");
                if (info.Length > 2)
                    ls.Add(info.Substring(1, info.Length - 2));
            }
            _owner.SelectedFiles = ls;
            _owner.Close(true);
        }
    }
}
