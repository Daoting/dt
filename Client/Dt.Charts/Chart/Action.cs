#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.System;
using Windows.UI.Xaml;
#endregion

namespace Dt.Charts
{
    public abstract partial class Action : DependencyObject
    {
        Dt.Charts.ActionType _atype;
        VirtualKeyModifiers _mkeys;

        internal Action(Dt.Charts.ActionType atype)
        {
            _atype = atype;
        }

        internal Dt.Charts.ActionType ActionType
        {
            get { return  _atype; }
        }

        public VirtualKeyModifiers Modifiers
        {
            get { return  _mkeys; }
            set { _mkeys = value; }
        }

        internal virtual bool SupportMouseWheel
        {
            get { return  false; }
        }
    }
}

