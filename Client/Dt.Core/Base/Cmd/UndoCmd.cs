#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 撤消命令
    /// </summary>
    public class UndoCmd : BaseCommand
    {
        CmdHistory _his;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_his"></param>
        public UndoCmd(CmdHistory p_his)
        {
            if (p_his != null)
            {
                _his = p_his;
                _his.CmdChanged += OnCmdChanged;
                AllowExecute = _his.CanUndo;
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="p_parameter"></param>
        protected override void DoExecute(object p_parameter)
        {
            _his.Undo();
        }

        void OnCmdChanged(object sender, EventArgs e)
        {
            AllowExecute = _his.CanUndo;
        }
    }
}