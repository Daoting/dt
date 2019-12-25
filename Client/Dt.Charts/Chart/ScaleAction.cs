#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Charts
{
    public class ScaleAction : Action
    {
        double _wheelFactor;

        public ScaleAction() : base(ActionType.Scale)
        {
            _wheelFactor = 0.25;
        }

        public Dt.Charts.MouseWheelDirection MouseWheelDirection { get; set; }

        public double MouseWheelFactor
        {
            get { return  _wheelFactor; }
            set { _wheelFactor = value; }
        }

        internal override bool SupportMouseWheel
        {
            get { return  (MouseWheelDirection != Dt.Charts.MouseWheelDirection.None); }
        }
    }
}

