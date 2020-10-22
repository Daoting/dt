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
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.App.File
{
    /// <summary>
    /// 文件夹内容
    /// </summary>
    public sealed partial class FolderPage : UserControl, INaviContent
    {
        readonly IFileMgr _fileMgr;

        public FolderPage(IFileMgr p_fileMgr)
        {
            InitializeComponent();
            _fileMgr = p_fileMgr;
            _lv.View = new FileItemSelector((DataTemplate)Resources["FolderTemplate"], (DataTemplate)Resources["FileTemplate"]);
            this.FirstLoaded(LoadData);
        }

        async void LoadData()
        {
            _lv.Data = await _fileMgr.GetFiles();
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.Row.Bool("IsFolder"))
            {
                var mgr = (IFileMgr)Activator.CreateInstance(_fileMgr.GetType());
                mgr.FolderID = e.Row.Long("id");
                mgr.FolderName = e.Row.Str("name");
                Host.NaviTo(new FolderPage(mgr));
            }
        }

        void OnSearch(object sender, Mi e)
        {
            Host.NaviTo(new SearchFilePage(_fileMgr));
        }

        async void OnUpload(object sender, Mi e)
        {
            var files = await CrossKit.PickFiles();
            if (files == null || files.Count == 0)
                return;

            int cnt = 0;
            DateTime ctime = AtSys.Now;
            foreach (var file in files)
            {
                Row row = new Row();
                row.AddCell("id", await AtCm.NewFlagID(0));
                row.AddCell("parentid", _fileMgr.FolderID);
                row.AddCell("name", file.DisplayName);
                row.AddCell("isfolder", false);
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
            AtKit.Msg($"成功上传{cnt}个文件");
        }

        async void OnAddFolder(object sender, Mi e)
        {
            if (await new EditFolderDlg().Show(_fileMgr, null))
                LoadData();
        }

        async void OnEditFolder(object sender, Mi e)
        {
            if (await new EditFolderDlg().Show(_fileMgr, e.Row))
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

        async void OnMove(object sender, Mi e)
        {
            var dlg = new MoveFileDlg();
            if (await dlg.Show(_fileMgr, e.Row))
            {
                var mgr = (IFileMgr)Activator.CreateInstance(_fileMgr.GetType());
                mgr.FolderID = dlg.Target.Long("id");
                mgr.FolderName = dlg.Target.Str("name");
                Host.NaviTo(new FolderPage(mgr));
            }
        }

        async void OnDelete(object sender, Mi e)
        {
            if (!(await AtKit.Confirm($"确认要删除【{e.Row.Str("name")}】吗？")))
            {
                AtKit.Msg("已取消删除！");
            }
            else if (await _fileMgr.Delete(e.Row))
            {
                if (!e.Row.Bool("IsFolder"))
                {
                    // 删除文件
                    var fi = GetFileItem(e.Row);
                    if (fi != null)
                        await fi.Delete();
                }
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
                if (_fileMgr.AllowEdit)
                {
                    _m["另存为"].Visibility = Visibility.Collapsed;
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
            else if (_fileMgr.AllowEdit)
            {
                _m["另存为"].Visibility = Visibility.Visible;
                _m["分享"].Visibility = Visibility.Visible;
                _m["重命名"].Visibility = Visibility.Collapsed;
                _m["移到"].Visibility = Visibility.Visible;
                _m["删除"].Visibility = Visibility.Visible;
            }
            else
            {
                _m["另存为"].Visibility = Visibility.Visible;
                _m["分享"].Visibility = Visibility.Visible;
                _m["重命名"].Visibility = Visibility.Collapsed;
                _m["移到"].Visibility = Visibility.Collapsed;
                _m["删除"].Visibility = Visibility.Collapsed;
            }
        }

        #region INaviContent
        public INaviHost Host { get; set; }

        Menu _menu;
        public Menu HostMenu
        {
            get
            {
                if (_menu == null)
                {
                    _menu = new Menu();
                    Mi mi = new Mi { ID = "搜索", Icon = Icons.搜索, ShowInPhone = VisibleInPhone.Icon };
                    mi.Click += OnSearch;
                    _menu.Items.Add(mi);

                    if (_fileMgr.AllowEdit)
                    {
                        mi = new Mi { ID = "上传文件", Icon = Icons.曲别针 };
                        mi.Click += OnUpload;
                        _menu.Items.Add(mi);

                        mi = new Mi { ID = "新建文件夹", Icon = Icons.加号 };
                        mi.Click += OnAddFolder;
                        _menu.Items.Add(mi);
                    }
                }
                return _menu;
            }
        }

        public string HostTitle
        {
            get { return _fileMgr.FolderName; }
        }
        #endregion
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
