#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    public class ZoomAction : Action
    {
        Brush _fill;
        Brush _stroke;

        public ZoomAction() : base(ActionType.Zoom)
        {
        }

        public Brush Fill
        {
            get { return  _fill; }
            set { _fill = value; }
        }

        public Brush Stroke
        {
            get { return  _stroke; }
            set { _stroke = value; }
        }
    }
}

