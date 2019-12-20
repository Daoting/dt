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

        public SpreadCharBaseContainer(SpreadChartBase spreadChart, Control c1Chart, GcViewport parentViewport) : base(spreadChart, parentViewport)
        {
            this._chartBaseView = this.CreateView(spreadChart, c1Chart);
            this._chartBaseView.ParentViewport = base.ParentViewport;
            this._chartBaseView.HorizontalAlignment = HorizontalAlignment.Stretch;
            this._chartBaseView.VerticalAlignment = VerticalAlignment.Stretch;
            base.Content = this._chartBaseView;
        }

        internal virtual SpreadChartBaseView CreateView(SpreadChartBase spreadChart, Control c1Chart)
        {
            return null;
        }

        private bool NeedRefreshDataSeriesOnAxisChanged(Axis changedAxis, string changed)
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
                this._chartBaseView.RefreshChartContent();
            }
            else
            {
                ChartChangedBaseEventArgs args = parameter as ChartChangedBaseEventArgs;
                ChartArea chartArea = args.ChartArea;
                if (this._chartBaseView != null)
                {
                    switch (chartArea)
                    {
                        case ChartArea.All:
                            this._chartBaseView.RefreshChartContent();
                            return;

                        case ChartArea.Chart:
                            if (!args.Property.ToString().ToLower().Contains("axis") && (args.Property != "DataOrientation"))
                            {
                                this._chartBaseView.RefreshChartArea();
                                this._chartBaseView.RefreshChartTitle();
                                return;
                            }
                            this._chartBaseView.RefreshChartContent();
                            return;

                        case ChartArea.PlotArea:
                            this._chartBaseView.RefreshPlotArea();
                            return;

                        case ChartArea.DataSeries:
                        case ChartArea.DataPoint:
                        case ChartArea.DataLabel:
                        case ChartArea.DataMarker:
                            this._chartBaseView.RefreshDataSeries();
                            return;

                        case ChartArea.AxisX:
                            this._chartBaseView.RefreshAxisX();
                            if (!(args.Chart is SpreadChart) || !this.NeedRefreshDataSeriesOnAxisChanged((args.Chart as SpreadChart).AxisX, args.Property))
                            {
                                break;
                            }
                            this._chartBaseView.RefreshDataSeries();
                            return;

                        case ChartArea.AxisY:
                            if ((args.Property != "LogBase") && (args.Property != "UseLogBase"))
                            {
                                this._chartBaseView.RefreshAxisY();
                                if (!(args.Chart is SpreadChart) || !this.NeedRefreshDataSeriesOnAxisChanged((args.Chart as SpreadChart).AxisX, args.Property))
                                {
                                    break;
                                }
                                this._chartBaseView.RefreshDataSeries();
                                return;
                            }
                            this._chartBaseView.RefreshChartContent();
                            return;

                        case ChartArea.AxisZ:
                        case ChartArea.FloorWall:
                        case ChartArea.SideWall:
                        case ChartArea.BackWall:
                            break;

                        case ChartArea.Lengend:
                            this._chartBaseView.RefreshChartLegend();
                            return;

                        case ChartArea.ChartTitle:
                            this._chartBaseView.RefreshChartTitle();
                            return;

                        case ChartArea.AxisTitle:
                            this._chartBaseView.RefreshAxisesTitle();
                            return;

                        default:
                            this._chartBaseView.RefreshChartContent();
                            break;
                    }
                }
            }
        }
    }
}

