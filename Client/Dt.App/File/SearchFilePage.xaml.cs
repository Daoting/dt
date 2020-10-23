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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.App.File
{
    /// <summary>
    /// 搜索文件
    /// </summary>
    public sealed partial class SearchFilePage : UserControl, INaviContent
    {
        readonly IFileMgr _fileMgr;

        public SearchFilePage(IFileMgr p_fileMgr)
        {
            InitializeComponent();
            _fileMgr = p_fileMgr;
        }

        async void OnSearch(object sender, string e)
        {
            _lv.Data = await _fileMgr.SearchFiles(e);
        }

        #region INaviContent
        void INaviContent.AddToHost(INaviHost p_host)
        {
            p_host.Title = "搜索";
            p_host.Menu = null;
        }
        #endregion
    }
}
