#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Charts
{
    internal class Renderers
    {
        PieRenderer _p2;
        Dt.Charts.Renderer2D _r2;
        RadarRenderer _rr;

        public IRenderer GetRenderer(string name)
        {
            switch (name)
            {
                case "Renderer2D":
                    return Renderer2D;

                case "Pie":
                    return Pie;

                case "Radar":
                    return Radar;
            }
            return null;
        }

        public PieRenderer Pie
        {
            get
            {
                if (_p2 == null)
                {
                    _p2 = new PieRenderer();
                }
                return _p2;
            }
        }

        public RadarRenderer Radar
        {
            get
            {
                if (_rr == null)
                {
                    _rr = new RadarRenderer();
                }
                return _rr;
            }
        }

        public Dt.Charts.Renderer2D Renderer2D
        {
            get
            {
                if (_r2 == null)
                {
                    _r2 = new Dt.Charts.Renderer2D();
                }
                return _r2;
            }
        }
    }
}

