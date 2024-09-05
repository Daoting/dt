﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-11-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 搜索文件
    /// </summary>
    public sealed partial class SearchFilePage : Tab
    {
        readonly IFileMgr _fileMgr;

        public SearchFilePage(IFileMgr p_fileMgr)
        {
            InitializeComponent();
            _fileMgr = p_fileMgr;
        }

        async void OnSearch(string e)
        {
            _lv.Data = await _fileMgr.SearchFiles(e);
        }
    }
}
