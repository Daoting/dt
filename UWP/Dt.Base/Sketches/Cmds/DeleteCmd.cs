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
#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 删除命令
    /// </summary>
    public class SketchDeleteCmd : BaseCommand
    {
        Sketch _owner;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_owner"></param>
        /// <param name="p_his"></param>
        public SketchDeleteCmd(Sketch p_owner, CmdHistory p_his)
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
            List<FrameworkElement> items = (List<FrameworkElement>)p_parameter;
            foreach (var item in items)
            {
                _owner.Container.Children.Remove(item);
            }
            _owner.SelectionClerk.Clear();
            _owner.OnDeleted(items);
        }

        /// <summary>
        /// 执行撤消
        /// </summary>
        /// <param name="p_parameter"></param>
        protected override void DoUndo(object p_parameter)
        {
            List<FrameworkElement> items = (List<FrameworkElement>)p_parameter;
            foreach (var item in items)
            {
                _owner.Container.Children.Add(item);
            }
            _owner.OnAdded(items);
        }
    }
}
