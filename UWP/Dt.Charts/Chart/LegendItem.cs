#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public class LegendItem
    {
        object _item;
        string _lbl;
        Shape _line;
        Shape _sym;

        internal LegendItem(Shape sym, Shape line, string lbl, object item)
        {
            _sym = sym;
            _line = line;
            _lbl = lbl;
            _item = item;
        }

        internal void Clear()
        {
            _item = null;
            _lbl = null;
            _line = null;
            _sym = null;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Label))
            {
                return base.ToString();
            }
            return Label;
        }

        public object Item
        {
            get { return  _item; }
        }

        public string Label
        {
            get { return  _lbl; }
        }

        public Shape Line
        {
            get { return  _line; }
        }

        public Shape Symbol
        {
            get { return  _sym; }
        }
    }
}

