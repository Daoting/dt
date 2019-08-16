#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 调整大小命令
    /// </summary>
    public class SketchResizeCmd : BaseCommand
    {
        Sketch _owner;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_owner"></param>
        /// <param name="p_his"></param>
        public SketchResizeCmd(Sketch p_owner, CmdHistory p_his)
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
            ResizeArgs args = p_parameter as ResizeArgs;
            SetLoc(args.Node, args.Target, args.NewRect);
        }

        /// <summary>
        /// 执行撤消
        /// </summary>
        /// <param name="p_parameter"></param>
        protected override void DoUndo(object p_parameter)
        {
            ResizeArgs args = p_parameter as ResizeArgs;
            SetLoc(args.Node, args.Target, args.OldRect);
        }

        /// <summary>
        /// 设置新位置
        /// </summary>
        /// <param name="p_node"></param>
        /// <param name="p_taget"></param>
        /// <param name="p_rect"></param>
        void SetLoc(NodeSelector p_node, FrameworkElement p_taget, Rect p_rect)
        {
            if (p_rect.Left > 0 && p_rect.Top > 0
                && p_rect.Width > 0 && p_rect.Height > 0)
            {
                //位置节点与目标相同，同步更新位置节点
                if (p_node.Target == p_taget)
                {
                    Canvas.SetLeft(p_node, p_rect.Left);
                    Canvas.SetTop(p_node, p_rect.Top);
                    p_node.Width = p_rect.Width;
                    p_node.Height = p_rect.Height;
                }
                Canvas.SetLeft(p_taget, p_rect.Left);
                Canvas.SetTop(p_taget, p_rect.Top);
                p_taget.Width = p_rect.Width;
                p_taget.Height = p_rect.Height;
                _owner.UpdateLayout();
                _owner.RefreshLinkLines(p_taget);


                if (p_node.Owner.SelectedNodes.Count == 0)
                    p_node.Owner.SelectionClerk.Select(p_taget, false);
                else
                    p_node.Target = p_taget;
            }
        }
    }

    /// <summary>
    /// 调整大小参数
    /// </summary>
    public class ResizeArgs
    {
        NodeSelector _node;
        Rect _oldRect;
        Rect _newRect;
        FrameworkElement _target;

        /// <summary>
        /// 调整大小参数
        /// </summary>
        /// <param name="p_node">位置点</param>
        /// <param name="p_oldRect">调整前位置</param>
        /// <param name="p_newRect">新位置</param>
        public ResizeArgs(NodeSelector p_node, Rect p_oldRect, Rect p_newRect)
        {
            _node = p_node;
            _oldRect = p_oldRect;
            _newRect = p_newRect;
            _target = p_node.Target;
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        public NodeSelector Node
        {
            get { return _node; }
        }

        /// <summary>
        /// 获取操作的元素
        /// </summary>
        public FrameworkElement Target
        {
            get { return _target; }
        }

        /// <summary>
        /// 获取设置调整前的位置及大小
        /// </summary>
        public Rect OldRect
        {
            get { return _oldRect; }
            set { _oldRect = value; }
        }

        /// <summary>
        /// 获取设置新位置及大小 
        /// </summary>
        public Rect NewRect
        {
            get { return _newRect; }
            set { _newRect = value; }
        }
    }
}
