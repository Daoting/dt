#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a border line pool to manager the border lines. 
    /// </summary>
    internal class BorderLinesPool
    {
        Dictionary<Windows.UI.Color, SolidColorBrush> _brushCache = new Dictionary<Windows.UI.Color, SolidColorBrush>();
        UIElementCollection _elements;
        int _seek = -1;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.Data.BorderLinesPool" /> class.
        /// </summary>
        /// <param name="elements">The border line elements.</param>
        public BorderLinesPool(UIElementCollection elements)
        {
            _elements = elements;
        }

        /// <summary>
        /// Collects all the border lines from the seek index.
        /// </summary>
        public void Collect()
        {
            int num = _elements.Count;
            for (int i = _seek + 1; i < num; i++)
            {
                _elements.RemoveAt(_seek + 1);
            }
        }

        /// <summary>
        /// Gets the cached solid brush from a color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The <see cref="T:System.Windows.Media.SolidColorBrush" /> according to the color.</returns>
        public SolidColorBrush GetSolidBrush(Windows.UI.Color color)
        {
            SolidColorBrush brush;
            if (!_brushCache.TryGetValue(color, out brush))
            {
                brush = new SolidColorBrush(color);
                _brushCache.Add(color, brush);
            }
            return brush;
        }

        /// <summary>
        /// Pops a border line.
        /// </summary>
        /// <returns></returns>
        public UIElement Pop()
        {
            int num = _elements.Count;
            if (_seek >= (num - 1))
            {
                ComboLine line = new ComboLine();
                _elements.Add(line);
                _seek++;
                return line;
            }
            _seek++;
            return (UIElement)_elements[_seek];
        }

        /// <summary>
        /// Resets the seek point to begin.
        /// </summary>
        public void Reset()
        {
            _seek = -1;
        }
    }
}

