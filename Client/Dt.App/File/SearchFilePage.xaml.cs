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

        #region INaviContent
        public INaviHost Host { get; set; }

        public Menu HostMenu
        {
            get { return null; }
        }

        public string HostTitle
        {
            get { return "搜索"; }
        }
        #endregion
    }
}
