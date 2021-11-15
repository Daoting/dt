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
    /// 选择移动到的目标文件夹
    /// </summary>
    public sealed partial class SelectFilePage : Mv
    {
        readonly IFileMgr _fileMgr;
        readonly SelectFileDlg _owner;

        public SelectFilePage(IFileMgr p_fileMgr, SelectFileDlg p_owner)
        {
            InitializeComponent();

            _fileMgr = p_fileMgr;
            _owner = p_owner;

            Title = _fileMgr.FolderName;
            LoadMenu();

            if (_owner.IsMultiSelection)
                _lv.SelectionMode = Base.SelectionMode.Multiple;
            _lv.View = new FileItemSelector((DataTemplate)Resources["FolderTemplate"], (DataTemplate)Resources["FileTemplate"]);
            LoadData();
        }

        async void LoadData()
        {
            if (string.IsNullOrEmpty(_owner.TypeFilter))
                _lv.Data = await _fileMgr.GetChildren();
            else
                _lv.Data = await _fileMgr.GetChildrenByType(_owner.TypeFilter);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.Row.Bool("IsFolder"))
            {
                _lv.ClearSelection();
                var mgr = (IFileMgr)Activator.CreateInstance(_fileMgr.GetType());
                mgr.FolderID = e.Row.ID;
                mgr.FolderName = e.Row.Str("name");
                mgr.Setting = _fileMgr.Setting;
                Forward(new SelectFilePage(mgr, _owner));
            }
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

        void OnSearch(object sender, Mi e)
        {
            Forward(new SelectSearchPage(_owner));
        }

        async void OnAdd(object sender, Mi e)
        {
            Dlg dlg = new Dlg();
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 600;
                dlg.Height = 600;
            }
            FolderPage page = new FolderPage(_fileMgr);
            dlg.Content = page;
            await dlg.ShowAsync();
            LoadData();
        }

        void LoadMenu()
        {
            var menu = new Menu();
            Mi mi = new Mi { ID = "确认", Icon = Icons.正确 };
            mi.Click += OnSelect;
            menu.Items.Add(mi);

            mi = new Mi { ID = "搜索", Icon = Icons.搜索 };
            mi.Click += OnSearch;
            menu.Items.Add(mi);

            if (_fileMgr.Setting.AllowEdit)
            {
                mi = new Mi { ID = "增加", Icon = Icons.加号 };
                mi.Click += OnAdd;
                menu.Items.Add(mi);
            }
            Menu = menu;
        }
    }
}
