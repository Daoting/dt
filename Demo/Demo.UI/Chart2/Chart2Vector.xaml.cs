#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using ScottPlot;
using ScottPlot.AxisRules;
using ScottPlot.Hatches;
using ScottPlot.Plottables;
#endregion

namespace Demo.UI
{
    public partial class Chart2Vector : Win
    {
        public Chart2Vector()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(10);
                double[] ys = Generate.Consecutive(10);

                // create a collection of vectors
                List<RootedCoordinateVector> vectors = new();
                for (int i = 0; i < xs.Length; i++)
                {
                    for (int j = 0; j < ys.Length; j++)
                    {
                        // point on the grid
                        Coordinates pt = new(xs[i], ys[j]);

                        // direction & magnitude
                        float dX = (float)ys[j];
                        float dY = -9.81f / 0.5f * (float)Math.Sin(xs[i]);
                        System.Numerics.Vector2 v = new(dX, dY);

                        // add to the collection
                        RootedCoordinateVector vector = new(pt, v);
                        vectors.Add(vector);
                    }
                }

                // plot the collection of rooted vectors as a vector field
                _c.Add.VectorField(vectors);
            }
        }

        void OnColor(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                RootedCoordinateVector[] vectors = Generate.SampleVectors();
                var vf = _c.Add.VectorField(vectors);
                vf.Colormap = new ScottPlot.Colormaps.Turbo();
            }
        }
    }
}