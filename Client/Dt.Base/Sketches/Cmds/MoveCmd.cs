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
    /// 移动命令
    /// </summary>
    public class SketchMoveCmd : BaseCommand
    {
        Sketch _owner;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_owner"></param>
        /// <param name="p_his"></param>
        public SketchMoveCmd(Sketch p_owner, CmdHistory p_his)
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
            UndoRedo((SketchMoveCmdArgs)p_parameter, false);
        }

        /// <summary>
        /// 执行撤消
        /// </summary>
        /// <param name="p_parameter"></param>
        protected override void DoUndo(object p_parameter)
        {
            UndoRedo((SketchMoveCmdArgs)p_parameter, true);
        }

        void UndoRedo(SketchMoveCmdArgs p_args, bool p_isUndo)
        {
            var clerk = _owner.SelectionClerk;
            clerk.Clear();
            clerk.Select(p_args.Items);
            if (p_isUndo)
                clerk.Move(-p_args.DeltaX, -p_args.DeltaY);
            else
                clerk.Move(p_args.DeltaX, p_args.DeltaY);
        }
    }

    /// <summary>
    /// 移动命令参数
    /// </summary>
    internal class SketchMoveCmdArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_items"></param>
        /// <param name="p_deltaX"></param>
        /// <param name="p_deltaY"></param>
        public SketchMoveCmdArgs(List<FrameworkElement> p_items, double p_deltaX, double p_deltaY)
        {
            Items = p_items;
            DeltaX = p_deltaX;
            DeltaY = p_deltaY;
        }

        /// <summary>
        /// 获取影响到的元素列表
        /// </summary>
        public List<FrameworkElement> Items { get; set; }

        /// <summary>
        /// 获取水平位移
        /// </summary>
        public double DeltaX { get; set; }

        /// <summary>
        /// 获取垂直位移
        /// </summary>
        public double DeltaY { get; set; }
    }
}
