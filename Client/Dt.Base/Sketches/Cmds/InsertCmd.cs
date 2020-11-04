#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 插入命令
    /// </summary>
    public class SketchInsertCmd : BaseCommand
    {
        Sketch _owner;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_owner"></param>
        /// <param name="p_his"></param>
        public SketchInsertCmd(Sketch p_owner, CmdHistory p_his)
            : base(p_his)
        {
            _owner = p_owner;
            AllowExecute = true;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="p_parameter"></param>
        protected override void DoExecute(object p_parameter)
        {
            if (p_parameter is FrameworkElement elem)
            {
                _owner.Container.Children.Add(elem);
                _owner.UpdateLayout();
                _owner.SelectionClerk.Select(elem, false);
                _owner.OnAdded(new List<FrameworkElement> { elem });
            }
        }

        /// <summary>
        /// 执行撤消
        /// </summary>
        /// <param name="p_parameter"></param>
        protected override void DoUndo(object p_parameter)
        {
            if (p_parameter is FrameworkElement elem
                && _owner.Container.Children.Contains(elem))
            {
                _owner.Container.Children.Remove(elem);
                _owner.SelectionClerk.Clear();
                _owner.OnDeleted(new List<FrameworkElement> { elem });
            }
        }
    }
}
