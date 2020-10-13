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

namespace Dt.Base.Report
{
    internal class SaveCmd : BaseCommand
    {
        RptDesignInfo _owner;

        public SaveCmd(RptDesignInfo p_owner)
        {
            _owner = p_owner;
            UpdateAllowExecute();
            _owner.DirtyChanged += (sender, e) => UpdateAllowExecute();
        }

        protected override void DoExecute(object p_parameter)
        {
            _owner.SaveTemplate();
        }

        void UpdateAllowExecute()
        {
            AllowExecute = _owner.IsDirty;
        }
    }
}
