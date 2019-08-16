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
    /// 移动连线命令
    /// </summary>
    public class SketchMoveLineCmd : BaseCommand
    {
        Sketch _owner;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_owner"></param>
        /// <param name="p_his"></param>
        public SketchMoveLineCmd(Sketch p_owner, CmdHistory p_his)
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
            LineMoveCmdArgs args = p_parameter as LineMoveCmdArgs;
            SLine line = args.Line;
            if (args.NewNode == null)
            {
                line.UpdateAnThumbPos(args.IsHeadNode);
                _owner.SelectionClerk.SelectLine(line);
                _owner.DeleteSelection();
            }
            else
            {
                if (args.IsHeadNode)
                {
                    line.HeaderID = args.NewNode.ID;
                    line.HeaderPort = args.NewPos;
                }
                else
                {
                    line.TailID = args.NewNode.ID;
                    line.TailPort = args.NewPos;
                }
            }
        }

        /// <summary>
        /// 执行撤消
        /// </summary>
        /// <param name="p_parameter"></param>
        protected override void DoUndo(object p_parameter)
        {
            LineMoveCmdArgs args = p_parameter as LineMoveCmdArgs;
            SLine line = args.Line;
            if (args.NewNode == null)
            {
                _owner.His.Undo();
            }
            else
            {
                if (args.IsHeadNode)
                {
                    line.HeaderID = args.OldNode.ID;
                    line.HeaderPort = args.OldPos;
                }
                else
                {
                    line.TailID = args.OldNode.ID;
                    line.TailPort = args.OldPos;
                }
            }
        }
    }

    internal class LineMoveCmdArgs
    {
        SLine _line;
        bool _isHeadNode;
        SNode _oldNode;
        SNode _newNode;
        LinkPortPosition _oldPos;
        LinkPortPosition _newPos;

        /// <summary>
        /// 无指向的线（将被删除）的线移动参数构造函数
        /// </summary>
        /// <param name="p_line"></param>
        /// <param name="p_isHeadNode"></param>
        /// <param name="p_oldNode"></param>
        /// <param name="p_oldPos"></param>
        public LineMoveCmdArgs(SLine p_line, bool p_isHeadNode, SNode p_oldNode, LinkPortPosition p_oldPos)
        {
            _line = p_line;
            _isHeadNode = p_isHeadNode;
            _oldNode = p_oldNode;
            _oldPos = p_oldPos;
        }

        /// <summary>
        /// 有指向节点的线移动参数构造函数
        /// </summary>
        /// <param name="p_line"></param>
        /// <param name="p_isHeadNode"></param>
        /// <param name="p_oldNode"></param>
        /// <param name="p_oldPos"></param>
        /// <param name="p_newNode"></param>
        /// <param name="p_newPos"></param>
        public LineMoveCmdArgs(SLine p_line, bool p_isHeadNode, SNode p_oldNode, LinkPortPosition p_oldPos, SNode p_newNode, LinkPortPosition p_newPos)
        {
            _line = p_line;
            _isHeadNode = p_isHeadNode;
            _oldNode = p_oldNode;
            _oldPos = p_oldPos;
            _newNode = p_newNode;
            _newPos = p_newPos;
        }

        public SLine Line
        {
            get { return _line; }
        }

        public bool IsHeadNode
        {
            get { return _isHeadNode; }
        }

        public SNode OldNode
        {
            get { return _oldNode; }
        }

        public SNode NewNode
        {
            get { return _newNode; }
        }

        public LinkPortPosition OldPos
        {
            get { return _oldPos; }
        }

        public LinkPortPosition NewPos
        {
            get { return _newPos; }
        }
    }
}
