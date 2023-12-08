﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-11-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 文件夹内容
    /// </summary>
    public sealed partial class FolderPage : Tab
    {
        IFileMgr _fileMgr;

        public FolderPage()
        {
            InitializeComponent();
            _lv.View = new FileItemSelector((DataTemplate)Resources["FolderTemplate"], (DataTemplate)Resources["FileTemplate"]);
        }

        protected override void OnFirstLoaded()
        {
            _fileMgr = (IFileMgr)NaviParams;
            Title = _fileMgr.FolderName;
            LoadMenu();
            LoadData();
        }

        async void LoadData()
        {
            _lv.Data = await _fileMgr.GetChildren();
        }

        void OnItemClick(ItemClickArgs e)
        {
            if (_lv.SelectionMode == Base.SelectionMode.Multiple || !e.Row.Bool("is_folder"))
                return;

            var mgr = (IFileMgr)Activator.CreateInstance(_fileMgr.GetType());
            mgr.FolderID = e.Row.ID;
            mgr.FolderName = e.Row.Str("name");
            mgr.Setting = _fileMgr.Setting;
            Forward(new FolderPage(), mgr);
        }

        void OnOpenedFile(object sender, FileItem e)
        {
            if (!_fileMgr.Setting.SaveHistory)
                return;

            Kit.RunAsync(async () =>
            {
                // 记录到本地已读文件目录
                var row = ((LvItem)e.DataContext).Row;
                var his = await ReadFileHistoryX.GetByID(row.ID);
                if (his == null)
                    his = new ReadFileHistoryX(ID: row.ID, Info: row.Str("info"));
                his.LastReadTime = Kit.Now;
                if (await his.Save(false))
                    _fileMgr.Setting.OnOpenedFile?.Invoke();
            });
        }

        void OnSearch(Mi e)
        {
            Forward(new SearchFilePage(_fileMgr));
        }

        async void OnUpload(Mi e)
        {
            var files = await Kit.PickFiles();
            if (files == null || files.Count == 0)
                return;

            int cnt = 0;
            DateTime ctime = Kit.Now;
            foreach (var file in files)
            {
                Row row = new Row();
                row.Add("id", await FilePubX.NewID());
                row.Add("parent_id", _fileMgr.FolderID);
                row.Add("name", file.DisplayName);
                row.Add("is_folder", false);
                row.Add("ext_name", file.Ext.TrimStart('.'));
                row.Add<string>("info");
                row.Add("ctime", ctime);
                _lv.Data.Add(row);

                FileList fl;
                var elem = _lv.GetRowUI(_lv.Data.Count - 1);
                if (elem == null || (fl = elem.FindChildByType<FileList>()) == null)
                {
                    _lv.Data.Remove(row);
                    continue;
                }

                bool suc = await fl.UploadFiles(new List<FileData> { file });
                if (suc)
                {
                    row["info"] = fl.Data;
                    suc = await _fileMgr.SaveFile(row);
                }

                if (!suc)
                    _lv.Data.Remove(row);
                else
                    cnt++;
            }
            Kit.Msg($"成功上传{cnt}个文件");
        }

        async void OnAddFolder(Mi e)
        {
            if (await Forward<bool>(new EditFolder(_fileMgr)))
                LoadData();
        }

        async void OnEditFolder(Mi e)
        {
            if (await Forward<bool>(new EditFolder(_fileMgr), e.Row))
                LoadData();
        }

        void OnSaveAs(Mi e)
        {
            GetFileItem(e.Row)?.SaveAs();
        }

        void OnShare(Mi e)
        {
            GetFileItem(e.Row)?.ShareFile();
        }

        void OnMove(Mi e)
        {
            MoveFiles(new List<Row> { e.Row });
        }

        void OnMultiMove(Mi e)
        {
            MoveFiles(_lv.SelectedRows);
        }

        async void MoveFiles(IEnumerable<Row> p_rows)
        {
            var dlg = new MoveFileDlg();
            if (await dlg.Show(_fileMgr, p_rows))
            {
                var mgr = (IFileMgr)Activator.CreateInstance(_fileMgr.GetType());
                mgr.FolderID = dlg.Target.FolderID;
                mgr.FolderName = dlg.Target.FolderName;
                mgr.Setting = _fileMgr.Setting;
                Forward(new FolderPage(), mgr);
                LoadData();
                OnCancelMulti(null);
            }
        }

        void OnDelete(Mi e)
        {
            DeleteFiles(new List<Row> { e.Row });
        }

        void OnMultiDelete(Mi e)
        {
            DeleteFiles(_lv.SelectedRows);
        }

        async void DeleteFiles(IEnumerable<Row> p_rows)
        {
            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
            }
            else if (await _fileMgr.Delete(p_rows))
            {
                // 不删除实际文件，其他位置可能已引用！
                //foreach (var row in p_rows)
                //{
                //    if (!row.Bool("is_folder"))
                //    {
                //        // 删除文件
                //        var fi = GetFileItem(row);
                //        if (fi != null)
                //            await fi.Delete();
                //    }
                //}
                LoadData();
            }
        }

        FileItem GetFileItem(Row p_row)
        {
            int index = _lv.Data.IndexOf(p_row);
            if (index < 0)
                return null;

            FileList fl;
            var elem = _lv.GetRowUI(index);
            if (elem == null || (fl = elem.FindChildByType<FileList>()) == null)
                return null;

            return fl.Items.FirstOrDefault();
        }

        async void OnMenuOpening(object sender, AsyncCancelArgs e)
        {
            Row row = (Row)_m.TargetData;
            if (row.Bool("is_folder"))
            {
                if (await _fileMgr.Setting.AllowEdit())
                {
                    _m["保存"].Visibility = Visibility.Collapsed;
                    _m["分享"].Visibility = Visibility.Collapsed;
                    _m["重命名"].Visibility = Visibility.Visible;
                    _m["移到"].Visibility = Visibility.Visible;
                    _m["删除"].Visibility = Visibility.Visible;
                }
                else
                {
                    // 不显示
                    e.Cancel = true;
                }
            }
            else if (await _fileMgr.Setting.AllowEdit())
            {
                _m["保存"].Visibility = Visibility.Visible;
                _m["分享"].Visibility = Visibility.Visible;
                _m["重命名"].Visibility = Visibility.Collapsed;
                _m["移到"].Visibility = Visibility.Visible;
                _m["删除"].Visibility = Visibility.Visible;
            }
            else
            {
                _m["保存"].Visibility = Visibility.Visible;
                _m["分享"].Visibility = Visibility.Visible;
                _m["重命名"].Visibility = Visibility.Collapsed;
                _m["移到"].Visibility = Visibility.Collapsed;
                _m["删除"].Visibility = Visibility.Collapsed;
            }
        }

        Menu _menu;
        Menu _menuMulti;
        void OnMultiMode(Mi e)
        {
            if (_menuMulti == null)
            {
                _menuMulti = new Menu();
                Mi mi = new Mi { ID = "移到", Icon = Icons.导入 };
                mi.Click += OnMultiMove;
                _menuMulti.Items.Add(mi);

                mi = new Mi { ID = "删除", Icon = Icons.删除 };
                mi.Click += OnMultiDelete;
                _menuMulti.Items.Add(mi);

                mi = new Mi { ID = "全选", Icon = Icons.正确 };
                mi.Click += OnSelectAll;
                _menuMulti.Items.Add(mi);

                mi = new Mi { ID = "取消", Icon = Icons.复选未选 };
                mi.Click += OnCancelMulti;
                _menuMulti.Items.Add(mi);
            }
            Menu = _menuMulti;
            _lv.SelectionMode = Base.SelectionMode.Multiple;
        }

        void OnSelectAll(Mi e)
        {
            _lv.SelectAll();
        }

        void OnCancelMulti(Mi e)
        {
            Menu = _menu;
            _lv.SelectionMode = Base.SelectionMode.Single;
        }

        async void LoadMenu()
        {
            _menu = new Menu();
            Mi mi = new Mi { ID = "搜索", Icon = Icons.搜索, ShowInPhone = VisibleInPhone.Icon };
            mi.Click += OnSearch;
            _menu.Items.Add(mi);

            if (await _fileMgr.Setting.AllowEdit())
            {
                mi = new Mi { ID = "上传文件", Icon = Icons.曲别针 };
                mi.Click += OnUpload;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "新建文件夹", Icon = Icons.加号 };
                mi.Click += OnAddFolder;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "选择", Icon = Icons.全选 };
                mi.Click += OnMultiMode;
                _menu.Items.Add(mi);
            }
            Menu = _menu;
        }
    }

    internal class FileItemSelector : DataTemplateSelector
    {
        readonly DataTemplate _folder;
        readonly DataTemplate _file;

        public FileItemSelector(DataTemplate p_folder, DataTemplate p_file)
        {
            _folder = p_folder;
            _file = p_file;
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (((LvItem)item).Row.Bool("is_folder"))
                return _folder;
            return _file;
        }
    }
}
