#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-12-12 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// CList功能扩展基类
    /// </summary>
    public abstract class CListEx
    {
        protected CList _owner;
        protected ListDlg _dlg;
        protected string _params;

        public void Init(CList p_owner, ListDlg p_dlg, string p_params)
        {
            _owner = p_owner;
            _dlg = p_dlg;
            _params = p_params;
            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        public abstract Task<INotifyList> GetData();
    }
}