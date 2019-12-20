#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    public partial class Excel : Control
    {
        /// <summary>
        /// 打印时的遮罩内容
        /// </summary>
        public static readonly DependencyProperty PrintMaskProperty = DependencyProperty.Register(
            "PrintMask",
            typeof(object),
            typeof(Excel),
            new PropertyMetadata(null));

        /// <summary>
        /// 报表项开始拖放事件
        /// </summary>
        public event EventHandler ItemStartDrag
        {
            add { View.ItemStartDrag += value; }
            remove { View.ItemStartDrag -= value; }
        }

        /// <summary>
        /// 报表项拖放结束事件
        /// </summary>
        public event EventHandler<CellEventArgs> ItemDropped
        {
            add { View.ItemDropped += value; }
            remove { View.ItemDropped -= value; }
        }

        /// <summary>
        /// 获取设置是否显示修饰层
        /// </summary>
        public bool ShowDecoration
        {
            get { return View.ShowDecoration; }
            set { View.ShowDecoration = value; }
        }

        /// <summary>
        /// 获取设置页面大小，修饰层画线用
        /// </summary>
        public Size PaperSize
        {
            get { return View.PaperSize; }
            set { View.PaperSize = value; }
        }

        /// <summary>
        /// 获取设置修饰区域
        /// </summary>
        public CellRange DecorationRange
        {
            get { return View.DecorationRange; }
            set { View.DecorationRange = value; }
        }

        /// <summary>
        /// 获取打印时的遮罩内容
        /// </summary>
        public object PrintMask
        {
            get { return GetValue(PrintMaskProperty); }
            internal set { SetValue(PrintMaskProperty, value); }
        }

        /// <summary>
        /// 打印Sheet内容
        /// </summary>
        /// <param name="p_printInfo">打印设置</param>
        /// <param name="p_sheetIndex">要打印的Sheet索引，-1表示当前活动Sheet</param>
        /// <param name="p_title">标题</param>
        public void Print(PrintInfo p_printInfo = null, int p_sheetIndex = -1, string p_title = null)
        {
            // 超出打印范围
            if (p_sheetIndex >= SheetCount)
                return;

            // Sheet索引
            int index = ActiveSheetIndex;
            if (p_sheetIndex > -1)
                index = p_sheetIndex;
            ExcelPrinter printer = new ExcelPrinter(this, p_printInfo == null ? new PrintInfo() : p_printInfo, index);
            string jobName = string.IsNullOrEmpty(p_title) ? Sheets[index].Name : p_title;
            printer.Print(jobName);
        }
    }
}

