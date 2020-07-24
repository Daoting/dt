#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    internal class FloatingObjectLayout
    {
        public FloatingObjectLayout(string name, double x, double y, double with, double height)
        {
            Name = name;
            X = x;
            Y = y;
            Width = with;
            Height = height;
        }

        public double Height { get; set; }

        public string Name { get; set; }

        public double Width { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}

