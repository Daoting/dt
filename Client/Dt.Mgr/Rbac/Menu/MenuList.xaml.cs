#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class MenuList : Tab
    {
        #region 构造方法
        public MenuList()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public async void Update()
        {
            // 记录已选择的节点
            var m = _tv.Selected<MenuX>();
            long id = m == null ? -1 : m.ID;
            _tv.Data = await MenuX.Query("where 1=1 order by dispidx");

            object select = null;
            if (id > 0)
            {
                select = (from row in (Table)_tv.Data
                          where row.ID == id
                          select row).FirstOrDefault();
            }
            _tv.SelectedItem = (select == null) ? _tv.FixedRoot : select;
        }

        /// <summary>
        /// 互换两行的显示位置，确保包含 id,dispidx 列
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_src"></param>
        /// <param name="p_tgt"></param>
        /// <returns>true 互换成功</returns>
        public static Task<bool> ExchangeDispidx<TEntity>(TEntity p_src, TEntity p_tgt)
            where TEntity : Entity
        {
            var tbl = new Table<TEntity> { { "id", typeof(long) }, { "dispidx", typeof(int) } };

            var save = tbl.AddRow(new { id = p_src.ID });
            save.AcceptChanges();
            save["dispidx"] = p_tgt["dispidx"];

            save = tbl.AddRow(new { id = p_tgt.ID });
            save.AcceptChanges();
            save["dispidx"] = p_src["dispidx"];

            return tbl.Save(false);
        }
        #endregion

        #region 初始化 
        protected override void OnFirstLoaded()
        {
            MenuX m = new MenuX(ID: 0, Name: "菜单", IsGroup: true, Icon: "主页");
            m.AddCell("parentname", "");
            _tv.FixedRoot = m;

            Update();
        }
        #endregion

        #region 交互
        void OnItemClick(object sender, ItemClickArgs e)
        {
            _win.Form.Update(e.Row.ID);
            NaviTo(new List<Tab> { _win.Form, _win.RoleList, });
        }

        void OnMoveUp(object sender, Mi e)
        {
            var src = e.Data.To<MenuX>();
            if (src.ID == 0)
                return;

            var tgt = _tv.GetTopBrother(src) as MenuX;
            if (tgt != null)
                Exchange(src, tgt);
        }

        void OnMoveDown(object sender, Mi e)
        {
            var src = e.Data.To<MenuX>();
            if (src.ID == 0)
                return;

            var tgt = _tv.GetFollowingBrother(src) as MenuX;
            if (tgt != null)
                Exchange(src, tgt);
        }

        async void Exchange(MenuX src, MenuX tgt)
        {
            if (await ExchangeDispidx(src, tgt))
            {
                Update();
            }
            else
            {
                Kit.Warn("菜单调序失败！");
            }
        }

        void OnRefresh(object sender, Mi e)
        {
            RbacDs.UpdateSqliteFile("menu");
        }
        #endregion

        #region 内部
        MenuWin _win => (MenuWin)OwnWin;
        #endregion
    }
}