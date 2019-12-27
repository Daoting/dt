#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System.Collections.Generic;
using System.ComponentModel;
#endregion

namespace Dt.Charts
{
    public class ChartSubtype
    {
        public ChartSubtype()
        {
        }

        internal ChartSubtype(string name, string rend, string rendopts)
        {
            Name = name;
            Renderer = rend;
            RendererOptions = rendopts;
        }

        internal void Apply(Chart chart)
        {
            chart.BeginUpdate();
            IRenderer renderer = chart.Renderers.GetRenderer(Renderer);
            if (renderer != null)
            {
                renderer.Options = RendererOptions;
                BaseRenderer renderer2 = renderer as BaseRenderer;
                if (renderer2 != null)
                {
                    renderer2.Symbol = Symbol;
                    renderer2.Connection = Connection;
                }
                if (chart.Data != null)
                {
                    chart.Data.Renderer = renderer;
                    using (IEnumerator<DataSeries> enumerator = chart.Data.Children.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.ClearDataCache();
                        }
                    }
                }
                if ((chart.View != null) && chart.View.Inverted)
                {
                    renderer2.Inverted = !renderer2.Inverted;
                }
            }
            chart.EndUpdate();
        }

        public object Connection { get; set; }

        public string Name { get; set; }

        public string Renderer { get; set; }

        public string RendererOptions { get; set; }

        public object Symbol { get; set; }
    }
}

