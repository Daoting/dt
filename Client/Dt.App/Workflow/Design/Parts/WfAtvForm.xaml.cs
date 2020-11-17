﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Linq;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 普通活动模板表单
    /// </summary>
    public sealed partial class WfAtvForm
    {
        public WfAtvForm()
        {
            InitializeComponent();
        }

        public void LoadNode(SNode p_node, Table<WfdAtvrole> p_atvRoles)
        {
            if (p_node.Tag != _fv.Data)
            {
                _propBox.LoadNode(p_node);
                _fv.Data = p_node.Tag;
                _atvRole.LoadRoles(p_node.ID, p_atvRoles);
                LoadExecDrop((WfdAtv)p_node.Tag);
            }
        }

        void LoadExecDrop(WfdAtv p_atv)
        {
            var items = from item in p_atv.Table.OfType<WfdAtv>()
                        where item != p_atv && item.Type != 2 && item.Type != 3
                        select item;
            Nl<IDStr> ls = new Nl<IDStr>();
            foreach (var atv in items)
            {
                ls.Add(new IDStr() { ID = atv.ID.ToString(), Str = atv.Name });
            }
            ((CList)_fv["execatvid"]).Data = ls;
        }
    }
}