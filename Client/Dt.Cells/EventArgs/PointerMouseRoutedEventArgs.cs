#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    internal class PointerMouseRoutedEventArgs
    {
        private PointerRoutedEventArgs _instance;

        public PointerMouseRoutedEventArgs(PointerRoutedEventArgs instance)
        {
            this._instance = instance;
        }

        public Windows.Foundation.Point GetPosition(UIElement element)
        {
            return this._instance.GetCurrentPoint(element).Position;
        }

        public PointerRoutedEventArgs Instance
        {
            get { return  this._instance; }
        }
    }
}

