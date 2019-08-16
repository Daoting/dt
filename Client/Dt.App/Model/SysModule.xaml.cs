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
using System.Threading.Tasks;
#endregion

namespace Dt.App.Model
{
    [View("系统模块")]
    public partial class SysModule : Win
    {
        public SysModule()
        {
            InitializeComponent();
            LoadSys();
        }

        #region 子系统
        async void LoadSys()
        {
            string id = _lvSys.SelectedItem == null ? null : _lvSys.SelectedRow.Str("id");
            _lvSys.Data = await AtCm.Query("模块-所有子系统");

            if (id != null)
            {
                var select = (from row in (Table)_lvSys.Data
                              where row.Str("id") == id
                              select row).FirstOrDefault();
                if (select != null)
                    _lvSys.SelectedItem = select;
            }
        }

        async void OnSysClick(object sender, ItemClickArgs e)
        {
            var row = await AtCm.GetRow("模块-id子系统", new Dict { { "id", e.Row.Str("id") } });
            row.Table.Name = "dt_subsys";
            _fvSys.Data = row;
            LoadModule();
            NaviTo("模块列表,子系统");
        }

        void OnNaviAdd(object sender, Mi e)
        {
            NaviTo("子系统");
            OnSysAdd(sender, e);
        }

        void OnSysMoveUp(object sender, Mi e)
        {
            Row row = e.TargetRow;
            Table tbl = (Table)_lvSys.Data;
            int index = tbl.IndexOf(row);
            if (index > 0)
                ChangeDispidx(row, tbl[index - 1]);
        }

        void OnSysMoveDown(object sender, Mi e)
        {
            Row row = e.TargetRow;
            Table tbl = (Table)_lvSys.Data;
            int index = tbl.IndexOf(row);
            if (index > -1 && index < tbl.Count - 1)
                ChangeDispidx(row, tbl[index + 1]);
        }

        void OnSysListDel(object sender, Mi e)
        {
            Row row = e.TargetRow;
            row.Table.Name = "dt_subsys";
            DelSysRow(row);
        }

        async void ChangeDispidx(Row p_row, Row p_tgt)
        {
            Table tbl = new Table { { "id" }, { "dispidx", typeof(int) } };
            tbl.Name = "dt_subsys";

            var save = tbl.NewRow(p_row.Str("id"));
            save.AcceptChanges();
            save["dispidx"] = p_tgt.Int("dispidx");

            save = tbl.NewRow(p_tgt.Str("id"));
            save.AcceptChanges();
            save["dispidx"] = p_row.Int("dispidx");

            if (await AtCm.Save(tbl, false))
                LoadSys();
        }

        async void OnSysAdd(object sender, Mi e)
        {
            _fvSys.Data = Table.Create("dt_subsys").NewRow(new
            {
                id = AtKit.NewID,
                name = "新子系统",
                dispidx = await AtCm.GetSeqVal("sq_subsys"),
            });
        }

        async void OnSysSave(object sender, Mi e)
        {
            if (_fvSys.ExistNull("name"))
                return;

            if (await AtCm.Save(_fvSys.Row))
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

            int count = await AtCm.GetScalar<int>("模块-子系统模块数", new Dict { { "subsysid", row.Str("id") } });
            if (count > 0)
            {
                AtKit.Warn("子系统含模块无法删除！");
                return;
            }

            if (await AtCm.Delete(row))
            {
                _fvSys.Data = null;
                LoadSys();
            }
        }
        #endregion

        async void LoadModule()
        {
            string id = _lvModule.SelectedItem == null ? null : _lvModule.SelectedRow.Str("id");
            Row subsys = _lvSys.SelectedRow;
            if (subsys != null)
                _lvModule.Data = await AtCm.Query("模块-子系统模块", new { subsysid = subsys.Str("id") });
            else
                _lvModule.Data = await AtCm.Query("模块-所有模块");

            if (id != null)
            {
                var select = (from row in (Table)_lvModule.Data
                              where row.Str("id") == id
                              select row).FirstOrDefault();
                if (select != null)
                    _lvModule.SelectedItem = select;
            }
        }

        async void OnModuleClick(object sender, ItemClickArgs e)
        {
            var row = await AtCm.GetRow("模块-id模块", new { id = e.Row.Str("id") });
            row.Table.Name = "dt_module";
            _fvModule.Data = row;
            NaviTo("模块");
        }

        void OnNaviModuleAdd(object sender, Mi e)
        {
            NaviTo("模块");
            OnModuleAdd(sender, e);
        }

        void OnModuleMoveUp(object sender, Mi e)
        {
            Row row = e.TargetRow;
            Table tbl = (Table)_lvModule.Data;
            int index = tbl.IndexOf(row);
            if (index > 0)
                ChangeModuleDispidx(row, tbl[index - 1]);
        }

        void OnModuleMoveDown(object sender, Mi e)
        {
            Row row = e.TargetRow;
            Table tbl = (Table)_lvModule.Data;
            int index = tbl.IndexOf(row);
            if (index > -1 && index < tbl.Count - 1)
                ChangeModuleDispidx(row, tbl[index + 1]);
        }

        void OnModuleListDel(object sender, Mi e)
        {
            Row row = e.TargetRow;
            row.Table.Name = "dt_module";
            DelModuleRow(row);
        }

        void OnAllModule(object sender, Mi e)
        {
            _lvSys.SelectedItem = null;
            LoadModule();
        }

        async void ChangeModuleDispidx(Row p_row, Row p_tgt)
        {
            Table tbl = new Table { { "id" }, { "dispidx", typeof(int) } };
            tbl.Name = "dt_module";

            var save = tbl.NewRow(p_row.Str("id"));
            save.AcceptChanges();
            save["dispidx"] = p_tgt.Int("dispidx");

            save = tbl.NewRow(p_tgt.Str("id"));
            save.AcceptChanges();
            save["dispidx"] = p_row.Int("dispidx");

            if (await AtCm.Save(tbl, false))
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
            var tbl = Table.Create("dt_module");
            tbl.Add("subsysname");
            _fvModule.Data = tbl.NewRow(new
            {
                id = AtKit.NewID,
                name = "新模块",
                subsysid = subsys.Str("id"),
                subsysname = subsys.Str("name"),
                dispidx = await AtCm.GetSeqVal("sq_module"),
            });
        }

        async void OnModuleSave(object sender, Mi e)
        {
            if (_fvModule.ExistNull("name"))
                return;

            if (await AtCm.Save(_fvModule.Row))
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

            if (await AtCm.Delete(p_row))
            {
                _fvModule.Data = null;
                LoadModule();
            }
        }

        async void OnLoadSubsys(object sender, AsyncEventArgs e)
        {
            using (e.Wait())
            {
                ((CList)sender).Data = await AtCm.Query("模块-所有子系统");
            }
        }
    }
}