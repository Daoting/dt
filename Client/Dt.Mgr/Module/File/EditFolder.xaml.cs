﻿#region 文件描述
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

namespace Dt.Mgr.Module
{
    public sealed partial class EditFolder : Tab
    {
        IFileMgr _fileMgr;

        public EditFolder(IFileMgr p_fileMgr)
        {
            InitializeComponent();
            _fileMgr = p_fileMgr;
        }

        protected override void OnFirstLoaded()
        {
            Row row = CreateData();
            if (NaviParams is Row r)
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
            row.Add<long>("id", -1);
            row.Add("name", "新目录");
            return row;
        }

        async void OnSave(Mi e)
        {
            if (_fv.ExistNull("name"))
                return;

            Row row = _fv.Row;
            if (await _fileMgr.SaveFolder(row.ID, row.Str("name")))
            {
                row.AcceptChanges();
                Result = true;
            }
        }

        void OnAdd(Mi e)
        {
            _fv.Data = CreateData();
        }
    }
}
