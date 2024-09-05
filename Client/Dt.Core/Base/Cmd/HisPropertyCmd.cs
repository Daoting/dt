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
    /// 可撤消和重做的属性变化历史命令
    /// </summary>
    public class HisPropertyCmd : BaseCommand
    {
        bool _isSetting = false;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_his"></param>
        public HisPropertyCmd(CmdHistory p_his)
            : base(p_his)
        {
            AllowExecute = true;
        }

        /// <summary>
        /// 获取是否正在重置属性值
        /// </summary>
        public bool IsSetting
        {
            get { return _isSetting; }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="p_parameter"></param>
        protected override void DoExecute(object p_parameter)
        {
            HisPropertyCmdArgs args;
            if ((args = p_parameter as HisPropertyCmdArgs) != null)
            {
                _isSetting = true;
                args.Redo();
                _isSetting = false;
            }
        }

        /// <summary>
        /// 执行撤消
        /// </summary>
        /// <param name="p_parameter"></param>
        protected override void DoUndo(object p_parameter)
        {
            HisPropertyCmdArgs args;
            if ((args = p_parameter as HisPropertyCmdArgs) != null)
            {
                _isSetting = true;
                args.Undo();
                _isSetting = false;
            }
        }
    }

    /// <summary>
    /// 属性变化历史命令参数
    /// </summary>
    public class HisPropertyCmdArgs
    {
        /// <summary>
        /// 获取命令的目标对象
        /// </summary>
        public Action Redo { get; set; }

        /// <summary>
        /// 获取当前命令的目标对象属性变化参数
        /// </summary>
        public Action Undo { get; set; }
    }
}