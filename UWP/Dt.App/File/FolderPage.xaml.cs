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
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.File
{
    /// <summary>
    /// 文件夹内容
    /// </summary>
    public sealed partial class FolderPage : Mv
    {
        readonly IFileMgr _fileMgr;

        public FolderPage(IFileMgr p_fileMgr)
        {
            InitializeComponent();
            _fileMgr = p_fileMgr;
            _lv.View = new FileItemSelector((DataTemplate)Resources["FolderTemplate"], (DataTemplate)Resources["FileTemplate"]);
            this.FirstLoaded(LoadData);

            Title = _fileMgr.FolderName;
            LoadMenu();
        }

        async void LoadData()
        {
            _lv.Data = await _fileMgr.GetChildren();
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (_lv.SelectionMode == Base.SelectionMode.Multiple || !e.Row.Bool("IsFolder"))
                return;

            var mgr = (IFileMgr)Activator.CreateInstance(_fileMgr.GetType());
            mgr.FolderID = e.Row.ID;
            mgr.FolderName = e.Row.Str("name");
            mgr.Setting = _fileMgr.Setting;
            Forward(new FolderPage(mgr));
        }

        void OnOpenedFile(object sender, FileItem e)
        {
            if (!_fileMgr.Setting.SaveHistory)
                return;

            Kit.RunAsync(async () =>
            {
                // 记录到本地已读文件目录
                var row = ((LvItem)e.DataContext).Row;
                var his = AtState.First<ReadFileHistory>("select * from ReadFileHistory where ID=@id", new { id = row.ID });
                if (his == null)
                    his = new ReadFileHistory(ID: row.ID, Info: row.Str("info"));
                his.LastReadTime = Kit.Now;
                if (await AtState.Save(his, false))
                    _fileMgr.Setting.OnOpenedFile?.Invoke();
            });
        }

        void OnSearch(object sender, Mi e)
        {
            Forward(new SearchFilePage(_fileMgr));
        }

        async void OnUpload(object sender, Mi e)
        {
            var files = await Kit.PickFiles();
            if (files == null || files.Count == 0)
                return;

            int cnt = 0;
            DateTime ctime = Kit.Now;
            foreach (var file in files)
            {
                Row row = new Row();
                row.AddCell("id", await AtCm.NewID());
                row.AddCell("parentid", _fileMgr.FolderID);
                row.AddCell("name", file.DisplayName);
                row.AddCell("isfolder", false);
                row.AddCell("extname", file.Ext.TrimStart('.'));
                row.AddCell<string>("info");
                row.AddCell("ctime", ctime);
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

        async void OnAddFolder(object sender, Mi e)
        {
            if (await Forward<bool>(new EditFolder(_fileMgr)))
                LoadData();
        }

        async void OnEditFolder(object sender, Mi e)
        {
            if (await Forward<bool>(new EditFolder(_fileMgr), e.Row))
                LoadData();
        }

        void OnSaveAs(object sender, Mi e)
        {
            GetFileItem(e.Row)?.SaveAs();
        }

        void OnShare(object sender, Mi e)
        {
            GetFileItem(e.Row)?.ShareFile();
        }

        void OnMove(object sender, Mi e)
        {
            MoveFiles(new List<Row> { e.Row });
        }

        void OnMultiMove(object sender, Mi e)
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
                Forward(new FolderPage(mgr));
                LoadData();
                OnCancelMulti(null, null);
            }
        }

        void OnDelete(object sender, Mi e)
        {
            DeleteFiles(new List<Row> { e.Row });
        }

        void OnMultiDelete(object sender, Mi e)
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
                //    if (!row.Bool("IsFolder"))
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

        void OnMenuOpening(object sender, AsyncCancelEventArgs e)
        {
            Row row = (Row)_m.TargetData;
            if (row.Bool("isfolder"))
            {
                if (_fileMgr.Setting.AllowEdit)
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
            else if (_fileMgr.Setting.AllowEdit)
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
        void OnMultiMode(object sender, Mi e)
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

        void OnSelectAll(object sender, Mi e)
        {
            _lv.SelectAll();
        }

        void OnCancelMulti(object sender, Mi e)
        {
            Menu = _menu;
            _lv.SelectionMode = Base.SelectionMode.Single;
        }

        void LoadMenu()
        {
            _menu = new Menu();
            Mi mi = new Mi { ID = "搜索", Icon = Icons.搜索, ShowInPhone = VisibleInPhone.Icon };
            mi.Click += OnSearch;
            _menu.Items.Add(mi);

            if (_fileMgr.Setting.AllowEdit)
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
            if (((LvItem)item).Row.Bool("IsFolder"))
                return _folder;
            return _file;
        }
    }
}
