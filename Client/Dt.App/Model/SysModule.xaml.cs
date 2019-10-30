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
using System.Linq;
#endregion

namespace Dt.App.Model
{
    [View("系统模块")]
    public partial class SysModule : Win
    {
        readonly CmDa _daSubsys = new CmDa("cm_subsys");
        readonly CmDa _daModule = new CmDa("cm_module");

        public SysModule()
        {
            InitializeComponent();
            LoadSys();
        }

        #region 子系统
        async void LoadSys()
        {
            long id = _lvSys.SelectedItem == null ? -1 : _lvSys.SelectedRow.ID;
            _lvSys.Data = await _daSubsys.Query("模块-所有子系统");

            if (id > 0)
            {
                var select = (from row in (Table)_lvSys.Data
                              where row.ID == id
                              select row).FirstOrDefault();
                if (select != null)
                    _lvSys.SelectedItem = select;
            }
        }

        async void OnSysClick(object sender, ItemClickArgs e)
        {
            _fvSys.Data = await _daSubsys.GetRow("模块-id子系统", new { ID = e.Row.ID });
            LoadModule();
            NaviTo("模块列表,子系统");
        }

        void OnNaviAdd(object sender, Mi e)
        {
            NaviTo("子系统");
            OnSysAdd(sender, e);
        }

        async void OnSysMoveUp(object sender, Mi e)
        {
            Row row = e.TargetRow;
            Table tbl = (Table)_lvSys.Data;
            int index = tbl.IndexOf(row);
            if (index > 0)
            {
                if (await _daSubsys.ExchangeDispidx(row, tbl[index - 1]))
                    LoadSys();
            }
        }

        async void OnSysMoveDown(object sender, Mi e)
        {
            Row row = e.TargetRow;
            Table tbl = (Table)_lvSys.Data;
            int index = tbl.IndexOf(row);
            if (index > -1 && index < tbl.Count - 1)
            {
                if (await _daSubsys.ExchangeDispidx(row, tbl[index + 1]))
                    LoadSys();
            }
        }

        void OnSysListDel(object sender, Mi e)
        {
            DelSysRow(e.TargetRow);
        }

        async void OnSysAdd(object sender, Mi e)
        {
            var ids = await _daSubsys.NewIDAndSeq("sq_subsys");
            _fvSys.Data = _daSubsys.NewRow(new
            {
                id = ids[0],
                name = "新子系统",
                dispidx = ids[1],
            });
        }

        async void OnSysSave(object sender, Mi e)
        {
            if (_fvSys.ExistNull("name"))
                return;

            if (await _daSubsys.Save(_fvSys.Row))
                LoadSys();
        }

        void OnSysDel(object sender, Mi e)
        {
            DelSysRow(_fvSys.Row);
        }

        async void DelSysRow(Row row)
        {
            if (!await AtKit.Confirm("确认要删除吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            if (row.IsAdded)
            {
                _fvSys.Data = _lvSys.SelectedItem;
                return;
            }

            int count = await _daSubsys.GetScalar<int>("模块-子系统模块数", new { subsysid = row.ID });
            if (count > 0)
            {
                AtKit.Warn("子系统含模块无法删除！");
                return;
            }

            if (await _daSubsys.Delete(row))
            {
                _fvSys.Data = null;
                LoadSys();
            }
        }
        #endregion

        async void LoadModule()
        {
            long id = _lvModule.SelectedItem == null ? -1 : _lvModule.SelectedRow.ID;
            Row subsys = _lvSys.SelectedRow;
            if (subsys != null)
                _lvModule.Data = await _daModule.Query("模块-子系统模块", new { subsysid = subsys.ID });
            else
                _lvModule.Data = await _daModule.Query("模块-所有模块");

            if (id > 0)
            {
                var select = (from row in (Table)_lvModule.Data
                              where row.ID == id
                              select row).FirstOrDefault();
                if (select != null)
                    _lvModule.SelectedItem = select;
            }
        }

        async void OnModuleClick(object sender, ItemClickArgs e)
        {
            _fvModule.Data = await _daModule.GetRow("模块-id模块", new { id = e.Row.ID });
            NaviTo("模块");
        }

        void OnNaviModuleAdd(object sender, Mi e)
        {
            NaviTo("模块");
            OnModuleAdd(sender, e);
        }

        async void OnModuleMoveUp(object sender, Mi e)
        {
            Row row = e.TargetRow;
            Table tbl = (Table)_lvModule.Data;
            int index = tbl.IndexOf(row);
            if (index > 0)
            {
                if (await _daModule.ExchangeDispidx(row, tbl[index - 1]))
                    LoadModule();
            }
        }

        async void OnModuleMoveDown(object sender, Mi e)
        {
            Row row = e.TargetRow;
            Table tbl = (Table)_lvModule.Data;
            int index = tbl.IndexOf(row);
            if (index > -1 && index < tbl.Count - 1)
            {
                if (await _daSubsys.ExchangeDispidx(row, tbl[index + 1]))
                    LoadModule();
            }
        }

        void OnModuleListDel(object sender, Mi e)
        {
            DelModuleRow(e.TargetRow);
        }

        void OnAllModule(object sender, Mi e)
        {
            _lvSys.SelectedItem = null;
            LoadModule();
        }

        async void OnModuleAdd(object sender, Mi e)
        {
            var data = _lvSys.Data as Table;
            if (data == null || data.Count == 0)
            {
                AtKit.Warn("无子系统！");
                return;
            }

            Row subsys = _lvSys.SelectedRow ?? data[0];
            var ids = await _daModule.NewIDAndSeq("sq_module");
            var row = _daModule.NewRow(new
            {
                id = ids[0],
                name = "新模块",
                subsysid = subsys.ID,
                dispidx = ids[1],
            });
            row.AddCell("subsysname", subsys.Str("name"));
            _fvModule.Data = row;
        }

        async void OnModuleSave(object sender, Mi e)
        {
            if (_fvModule.ExistNull("name"))
                return;

            if (await _daModule.Save(_fvModule.Row))
                LoadModule();
        }

        void OnModuleDel(object sender, Mi e)
        {
            DelModuleRow(_fvModule.Row);
        }

        async void DelModuleRow(Row p_row)
        {
            if (!await AtKit.Confirm("确认要删除吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            if (p_row.IsAdded)
            {
                _fvModule.Data = _lvModule.SelectedItem;
                return;
            }

            if (await _daModule.Delete(p_row))
            {
                _fvModule.Data = null;
                LoadModule();
            }
        }

        async void OnLoadSubsys(object sender, AsyncEventArgs e)
        {
            using (e.Wait())
            {
                ((CList)sender).Data = await _daSubsys.Query("模块-所有子系统");
            }
        }
    }
}