#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xamarin.Essentials;
#endregion

namespace Dt.Base.Tools
{
    /// <summary>
    /// 本地库
    /// </summary>
    public sealed partial class LocalDbView : Win
    {
        public LocalDbView()
        {
            InitializeComponent();
            _lvDb.Data = SqliteDbs.GetAllDbInfo();
        }

        void OnDbClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
            {
                _lvTbl.Data = GetDb().QueryTblsName();
                _lvData.Data = null;
            }
            NaviTo("表");
        }

#if WIN
        async void OnBackup(object sender, Mi e)
        {
            var row = e.Row;
            var picker = Kit.GetFileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("sqlite文件", new List<string>() { ".db" });
            var fileName = row.Str("name") + ".db";
            picker.SuggestedFileName = fileName;
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(Kit.DataPath);
                var temp = await folder.TryGetItemAsync(fileName) as StorageFile;
                if (temp != null)
                {
                    await temp.CopyAndReplaceAsync(file);
                    Kit.Msg("文件备份成功！");
                }
            }
        }
#elif ANDROID
        void OnBackup(object sender, Mi e)
        {
            var row = e.Row;
            try
            {
                var dbFile = Path.Combine(Kit.DataPath, row.Str("name") + ".db");
                var tgtName = $"{row.Str("name")}-{Guid.NewGuid().ToString().Substring(0, 8)}.db";
                File.Copy(dbFile, Path.Combine(IOUtil.GetDownloadsPath(), tgtName));
                Kit.Msg("已保存到下载目录：\r\n" + tgtName);
            }
            catch
            {
                Kit.Warn("文件保存失败！");
            }
        }
#elif IOS
        void OnBackup(object sender, Mi e)
        {
            ShareFile(e);
        }
#elif WASM
        async void OnBackup(object sender, Mi e)
        {
            var row = e.Row;
            var picker = Kit.GetFileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("sqlite文件", new List<string>() { ".db" });
            var fileName = row.Str("name") + ".db";
            picker.SuggestedFileName = fileName;
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                var dbFile = Path.Combine(Kit.DataPath, row.Str("name") + ".db");
                var data = File.ReadAllBytes(dbFile);
                //Log.Debug($"长度：{data.Length}");
                //Log.Debug($"路径：{file.Path}");

                // 此方法无法正常保存
                //File.WriteAllBytes(file.Path, data);

                try
                {
                    using (var stream = await file.OpenStreamForWriteAsync())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    Kit.Msg("文件保存成功！");
                }
                catch
                {
                    Kit.Warn("文件保存失败！");
                }
            }
        }
#endif

        void OnShare(object sender, Mi e)
        {
            ShareFile(e);
        }

        async void ShareFile(Mi e)
        {
            try
            {
                var row = e.Row;
                var dbFile = Path.Combine(Kit.DataPath, row.Str("name") + ".db");
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "分享文件",
                    File = new ShareFile(dbFile)
                });
            }
            catch
            {
                Kit.Warn("暂未实现");
            }
        }

        void OnTblClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _lvData.Data = GetDb().Query($"select * from '{e.Row.Str("name")}'");
            NaviTo("数据");
        }

        async void OnDel(object sender, Mi e)
        {
            if (_lvDb.SelectedRow.Str("name") == "model")
            {
                Kit.Warn("模型库禁止删除数据！");
                return;
            }

            if (!await Kit.Confirm($"确认要删除这{_lvData.SelectedCount}行吗？"))
                return;

            var db = GetDb();
            var tblName = _lvTbl.SelectedRow.Str(0);
            var pk = db.GetScalar<string>($"select name from pragma_table_info('{tblName}') where pk=1");
            if (string.IsNullOrEmpty(pk))
            {
                Kit.Warn("该表无主键！");
                return;
            }

            List<Dict> ls = new List<Dict>();
            foreach (var row in _lvData.SelectedRows)
            {
                ls.Add(new Dict { { pk, row[pk] } });
            }
            if (db.BatchExecute($"delete from '{tblName}' where {pk}=@{pk}", ls) > 0)
            {
                _lvData.DeleteSelection();
            }
        }

        SqliteConnectionEx GetDb()
        {
            return SqliteDbs.GetDb(_lvDb.SelectedRow.Str("name"));
        }

    }
}
