#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System.Linq;
#endregion

namespace Dt.App.Model
{
    [View("用户账号")]
    public partial class UserAccount : Win
    {
        readonly CmDa _daGrp = new CmDa("cm_usergrp");

        public UserAccount()
        {
            InitializeComponent();
            Load();
        }

        void Load()
        {
            var row = _daGrp.NewRow(new { name = "用户分组" });
            row.AddCell("parentname", "");
            _tv.FixedRoot = row;
            LoadTreeData();
        }

        async void LoadTreeData()
        {
            // 记录已选择的节点
            long id = _tv.SelectedItem == null ? -1 : _tv.SelectedRow.ID;
            _tv.Data = await _daGrp.Query("用户-分组树");

            object select = null;
            if (id > 0)
            {
                select = (from row in (Table)_tv.Data
                          where row.ID == id
                          select row).FirstOrDefault();
            }
            _tv.SelectedItem = (select == null) ? _tv.FixedRoot : select;
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.Row.ID > 0)
                _fvGrp.Data = await _daGrp.GetRow("用户-id分组", new { ID = e.Row.ID });
            else
                _fvGrp.Data = null;
            //NaviTo("菜单项,菜单授权");
        }

        async void OnMoveUp(object sender, Mi e)
        {
            Row row = e.TargetRow;
            Row tgt = _tv.GetTopBrother(row) as Row;
            if (tgt != null && await _daGrp.ExchangeDispidx(row, tgt))
                LoadTreeData();
        }

        async void OnMoveDown(object sender, Mi e)
        {
            Row row = e.TargetRow;
            Row tgt = _tv.GetFollowingBrother(row) as Row;
            if (tgt != null && await _daGrp.ExchangeDispidx(row, tgt))
                LoadTreeData();
        }

        void OnListDel(object sender, Mi e)
        {
            //DelMenuRow(e.TargetRow);
        }

        async void OnGrpAdd(object sender, Mi e)
        {
            var ids = await _daGrp.NewIDAndSeq("sq_usergrp");
            var row = _daGrp.NewRow(new
            {
                id = ids[0],
                name = "新分组",
                parentid = _tv.SelectedRow.ID,
                dispidx = ids[1],
            });
            row.AddCell("parentname", _tv.SelectedRow.Str("name"));
            _fvGrp.Data = row;
        }

        async void OnGrpSave(object sender, Mi e)
        {
            if (_fvGrp.ExistNull("name"))
                return;

            if (await _daGrp.Save(_fvGrp.Row))
                LoadTreeData();
        }

        void OnGrpDel(object sender, Mi e)
        {
            //DelSysRow(_fvSys.Row);
        }
    }
}