#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Threading.Tasks;
#endregion

namespace Dt.App.File
{
    public sealed partial class EditFolderDlg : Dlg
    {
        bool _needRefresh;
        IFileMgr _fileMgr;

        public EditFolderDlg()
        {
            InitializeComponent();
        }

        public async Task<bool> Show(IFileMgr p_fileMgr, Row p_row)
        {
            _fileMgr = p_fileMgr;
            Row row = CreateData();
            if (p_row != null)
            {
                row.InitVal(0, p_row.ID);
                row.InitVal(1, p_row["name"]);
            }
            _fv.Data = row;
            await ShowAsync();
            return _needRefresh;
        }

        Row CreateData()
        {
            Row row = new Row();
            row.AddCell<long>("id", -1);
            row.AddCell("name", "新目录");
            return row;
        }

        async void OnSave(object sender, Mi e)
        {
            if (_fv.ExistNull("name"))
                return;

            Row row = _fv.Row;
            if (await _fileMgr.SaveFolder(row.ID, row.Str("name")))
            {
                _needRefresh = true;
                _fv.Data = CreateData();
            }
        }

        void OnAdd(object sender, Mi e)
        {
            _fv.Data = CreateData();
        }

        protected override Task<bool> OnClosing()
        {
            if (_fv.Row.IsChanged)
                return AtKit.Confirm("数据未保存，要放弃修改吗？");
            return Task.FromResult(true);
        }
    }
}
