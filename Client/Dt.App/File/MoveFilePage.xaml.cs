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
    /// 选择移动到的目标文件夹
    /// </summary>
    public sealed partial class MoveFilePage : UserControl, INaviContent
    {
        readonly IFileMgr _fileMgr;
        readonly MoveFileDlg _owner;

        public MoveFilePage(IFileMgr p_fileMgr, MoveFileDlg p_owner)
        {
            InitializeComponent();
            _fileMgr = p_fileMgr;
            _owner = p_owner;
            LoadData();
        }

        async void LoadData()
        {
            var tbl = await _fileMgr.GetChildFolders();
            _owner.RemoveSelection(tbl, _fileMgr);
            _lv.Data = tbl;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            var mgr = (IFileMgr)Activator.CreateInstance(_fileMgr.GetType());
            mgr.FolderID = e.Row.ID;
            mgr.FolderName = e.Row.Str("name");
            _host.NaviTo(new MoveFilePage(mgr, _owner));
        }

        void OnSelect(object sender, Mi e)
        {
            _owner.MoveTo(_fileMgr);
        }

        #region INaviContent
        INaviHost _host;

        void INaviContent.AddToHost(INaviHost p_host)
        {
            _host = p_host;
            _host.Title = $"移到->{_fileMgr.FolderName} ";

            var menu = new Menu();
            Mi mi = new Mi { ID = "确认", Icon = Icons.正确 };
            mi.Click += OnSelect;
            menu.Items.Add(mi);
            _host.Menu = menu;
        }
        #endregion
    }
}
