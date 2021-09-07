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
    public sealed partial class EditFolder : Mv
    {
        IFileMgr _fileMgr;

        public EditFolder(IFileMgr p_fileMgr)
        {
            InitializeComponent();
            _fileMgr = p_fileMgr;
        }

        protected override void OnInit(object p_params)
        {
            Row row = CreateData();
            if (p_params is Row r)
            {
                row.InitVal(0, r.ID);
                row.InitVal(1, r["name"]);
            }
            _fv.Data = row;
        }

        protected override Task<bool> OnClosing()
        {
            if (_fv.Row.IsChanged)
                return Kit.Confirm("数据未保存，要放弃修改吗？");
            return Task.FromResult(true);
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
                Result = true;
                _fv.Data = CreateData();
            }
        }

        void OnAdd(object sender, Mi e)
        {
            _fv.Data = CreateData();
        }
    }
}
