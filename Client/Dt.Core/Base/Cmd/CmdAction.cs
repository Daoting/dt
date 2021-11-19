#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间

#endregion

namespace Dt.Core
{
    /// <summary>
    /// 可撤消、重做的命令动作描述类
    /// </summary>
    public class CmdAction
    {
        BaseCommand _cmd;
        object _args;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_cmd"></param>
        /// <param name="p_args"></param>
        public CmdAction(BaseCommand p_cmd, object p_args)
        {
            _cmd = p_cmd;
            _args = p_args;
        }

        /// <summary>
        /// 撤消
        /// </summary>
        public void Undo()
        {
            _cmd.Undo(_args);
        }

        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            _cmd.Redo(_args);
        }
    }
}