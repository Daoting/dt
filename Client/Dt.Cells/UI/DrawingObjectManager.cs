#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// The <see cref="T:GrapeCity.Windows.SpreadSheet.UI.DrawingObjectManager" /> class is used to attach
    /// a <see cref="T:GrapeCity.Windows.SpreadSheet.UI.DrawingObjectProvider" />  to  <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /> control. 
    /// </summary>
    public partial class DrawingObjectManager : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.DrawingObjectManager.DrawingObjecProvider" /> attached property. 
        /// </summary>
        public static readonly DependencyProperty DrawingObjectProviderProperty = DependencyProperty.RegisterAttached("DrawingObjectProvider", (Type) typeof(IDrawingObjectProvider), (Type) typeof(Excel), new PropertyMetadata(null));

        /// <summary>
        /// Returns the value of the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.DrawingObjectManager.IDrawingObjectProvider" /> attached property
        /// for a given <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /> control.
        /// </summary>
        /// <param name="element">A <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /></param>
        /// <returns>An instance of custom <see cref="T:GrapeCity.Windows.SpreadSheet.UI.IDrawingObjectProvider" /></returns>
        public static IDrawingObjectProvider GetDrawingObjectProvider(Excel element)
        {
            return (IDrawingObjectProvider) element.GetValue(DrawingObjectProviderProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.DrawingObjectManager.IDrawingObjectProvider" /> attached property
        /// for a given <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /> control.
        /// </summary>
        /// <param name="element">A <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /></param>
        /// <param name="value">An instance of custom <see cref="T:GrapeCity.Windows.SpreadSheet.UI.IDrawingObjectProvider" /></param>
        public static void SetDrawingObjectProvider(Excel element, IDrawingObjectProvider value)
        {
            element.SetValue(DrawingObjectProviderProperty, value);
        }
    }
}

