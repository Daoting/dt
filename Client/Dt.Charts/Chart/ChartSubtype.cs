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
    [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Never)]
    public class ChartSubtype
    {
        object _conn;
        string _name;
        string _rname;
        string _ropts;
        object _sym;

        public ChartSubtype()
        {
        }

        internal ChartSubtype(string name, string rend, string rendopts)
        {
            _name = name;
            _rname = rend;
            _ropts = rendopts;
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

        public object Connection
        {
            get { return  _conn; }
            set { _conn = value; }
        }

        public string Name
        {
            get { return  _name; }
            set { _name = value; }
        }

        public string Renderer
        {
            get { return  _rname; }
            set { _rname = value; }
        }

        public string RendererOptions
        {
            get { return  _ropts; }
            set { _ropts = value; }
        }

        public object Symbol
        {
            get { return  _sym; }
            set { _sym = value; }
        }
    }
}

