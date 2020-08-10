#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    internal partial class SpreadCharBaseContainer : FloatingObjectContainer
    {
        internal SpreadChartBaseView _chartBaseView;

        public SpreadCharBaseContainer(SpreadChartBase spreadChart, Control c1Chart, CellsPanel parentViewport) : base(spreadChart, parentViewport)
        {
            _chartBaseView = CreateView(spreadChart, c1Chart);
            _chartBaseView.ParentViewport = base.ParentViewport;
            _chartBaseView.HorizontalAlignment = HorizontalAlignment.Stretch;
            _chartBaseView.VerticalAlignment = VerticalAlignment.Stretch;
            base.Content = _chartBaseView;
        }

        internal virtual SpreadChartBaseView CreateView(SpreadChartBase spreadChart, Control c1Chart)
        {
            return null;
        }

        bool NeedRefreshDataSeriesOnAxisChanged(Axis changedAxis, string changed)
        {
            if ((changed != "Items") || (changedAxis.AutoMin && changedAxis.AutoMax))
            {
                return false;
            }
            return true;
        }

        internal override void Refresh(object parameter)
        {
            base.Refresh(parameter);
            if ((parameter == null) || !(parameter is ChartChangedBaseEventArgs))
            {
                _chartBaseView.RefreshChartContent();
            }
            else
            {
                ChartChangedBaseEventArgs args = parameter as ChartChangedBaseEventArgs;
                ChartArea chartArea = args.ChartArea;
                if (_chartBaseView != null)
                {
                    switch (chartArea)
                    {
                        case ChartArea.All:
                            _chartBaseView.RefreshChartContent();
                            return;

                        case ChartArea.Chart:
                            if (!args.Property.ToString().ToLower().Contains("axis") && (args.Property != "DataOrientation"))
                            {
                                _chartBaseView.RefreshChartArea();
                                _chartBaseView.RefreshChartTitle();
                                return;
                            }
                            _chartBaseView.RefreshChartContent();
                            return;

                        case ChartArea.PlotArea:
                            _chartBaseView.RefreshPlotArea();
                            return;

                        case ChartArea.DataSeries:
                        case ChartArea.DataPoint:
                        case ChartArea.DataLabel:
                        case ChartArea.DataMarker:
                            _chartBaseView.RefreshDataSeries();
                            return;

                        case ChartArea.AxisX:
                            _chartBaseView.RefreshAxisX();
                            if (!(args.Chart is SpreadChart) || !NeedRefreshDataSeriesOnAxisChanged((args.Chart as SpreadChart).AxisX, args.Property))
                            {
                                break;
                            }
                            _chartBaseView.RefreshDataSeries();
                            return;

                        case ChartArea.AxisY:
                            if ((args.Property != "LogBase") && (args.Property != "UseLogBase"))
                            {
                                _chartBaseView.RefreshAxisY();
                                if (!(args.Chart is SpreadChart) || !NeedRefreshDataSeriesOnAxisChanged((args.Chart as SpreadChart).AxisX, args.Property))
                                {
                                    break;
                                }
                                _chartBaseView.RefreshDataSeries();
                                return;
                            }
                            _chartBaseView.RefreshChartContent();
                            return;

                        case ChartArea.AxisZ:
                        case ChartArea.FloorWall:
                        case ChartArea.SideWall:
                        case ChartArea.BackWall:
                            break;

                        case ChartArea.Lengend:
                            _chartBaseView.RefreshChartLegend();
                            return;

                        case ChartArea.ChartTitle:
                            _chartBaseView.RefreshChartTitle();
                            return;

                        case ChartArea.AxisTitle:
                            _chartBaseView.RefreshAxisesTitle();
                            return;

                        default:
                            _chartBaseView.RefreshChartContent();
                            break;
                    }
                }
            }
        }
    }
}

