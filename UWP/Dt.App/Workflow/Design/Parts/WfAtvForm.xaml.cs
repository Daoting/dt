#region 文件描述
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
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 普通活动模板表单
    /// </summary>
    public sealed partial class WfAtvForm : UserControl
    {
        public WfAtvForm()
        {
            InitializeComponent();
        }

        public void LoadNode(SNode p_node, Table<WfdAtvroleObj> p_atvRoles)
        {
            if (p_node.Tag != _fv.Data)
            {
                _propBox.LoadNode(p_node);
                _fv.Data = p_node.Tag;
                _atvRole.LoadRoles(p_node.ID, p_atvRoles);
                LoadExecDrop((WfdAtvObj)p_node.Tag);
            }
        }

        void LoadExecDrop(WfdAtvObj p_atv)
        {
            var items = from item in p_atv.Table.OfType<WfdAtvObj>()
                        where item != p_atv && item.Type != WfdAtvType.Sync && item.Type != WfdAtvType.Finish
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
