#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-12-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// Undo命令
    /// </summary>
    public class FvUndoCmd : BaseCommand
    {
        Fv _owner;

        public FvUndoCmd(Fv p_owner)
        {
            _owner = p_owner;
            UpdateAllowExecute();
            _owner.Dirty += (sender, e) => UpdateAllowExecute();
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="p_parameter"></param>
        protected override void DoExecute(object p_parameter)
        {
            _owner.RejectChanges();
        }

        void UpdateAllowExecute()
        {
            AllowExecute = _owner.IsDirty;
        }
    }
}
