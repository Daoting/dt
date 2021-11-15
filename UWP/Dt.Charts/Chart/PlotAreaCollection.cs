#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.ObjectModel;
#endregion

namespace Dt.Charts
{
    public class PlotAreaCollection : ObservableCollection<PlotArea>
    {
        ObservableCollection<PlotAreaColumnDefinition> _cols = new ObservableCollection<PlotAreaColumnDefinition>();
        ObservableCollection<PlotAreaRowDefinition> _rows = new ObservableCollection<PlotAreaRowDefinition>();

        internal PlotAreaCollection()
        {
        }

        internal double[] CalculateHeights(double height, int nrows)
        {
            if (nrows <= 0)
            {
                throw new ArgumentException("nrows");
            }
            Windows.UI.Xaml.GridLength[] glens = new Windows.UI.Xaml.GridLength[nrows];
            for (int i = 0; i < nrows; i++)
            {
                if (i < RowDefinitions.Count)
                {
                    glens[i] = RowDefinitions[i].Height;
                }
                else
                {
                    glens[i] = Windows.UI.Xaml.GridLength.Auto;
                }
            }
            return CalculateLengths(height, nrows, glens);
        }

        internal double[] CalculateLengths(double width, int ncols, Windows.UI.Xaml.GridLength[] glens)
        {
            double[] numArray = new double[ncols];
            double num = 0.0;
            double num2 = 0.0;
            for (int i = 0; i < ncols; i++)
            {
                if (glens[i].IsAbsolute)
                {
                    numArray[i] = glens[i].Value;
                    num += numArray[i];
                }
                else if (glens[i].IsStar)
                {
                    num2 += glens[i].Value;
                }
                else if (glens[i].IsAuto)
                {
                    num2++;
                }
            }
            double num4 = width - num;
            double num5 = num4 / num2;
            for (int j = 0; j < ncols; j++)
            {
                if (glens[j].IsStar)
                {
                    numArray[j] = num5 * glens[j].Value;
                }
                else if (glens[j].IsAuto)
                {
                    numArray[j] = num5;
                }
                if (numArray[j] < 0.0)
                {
                    numArray[j] = 0.0;
                }
            }
            return numArray;
        }

        internal double[] CalculateWidths(double width, int ncols)
        {
            if (ncols <= 0)
            {
                throw new ArgumentException("ncols");
            }
            Windows.UI.Xaml.GridLength[] glens = new Windows.UI.Xaml.GridLength[ncols];
            for (int i = 0; i < ncols; i++)
            {
                if (i < ColumnDefinitions.Count)
                {
                    glens[i] = ColumnDefinitions[i].Width;
                }
                else
                {
                    glens[i] = Windows.UI.Xaml.GridLength.Auto;
                }
            }
            return CalculateLengths(width, ncols, glens);
        }

        public ObservableCollection<PlotAreaColumnDefinition> ColumnDefinitions
        {
            get { return  _cols; }
        }

        public ObservableCollection<PlotAreaRowDefinition> RowDefinitions
        {
            get { return  _rows; }
        }
    }
}

