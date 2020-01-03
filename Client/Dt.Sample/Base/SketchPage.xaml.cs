#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Sample
{
    public sealed partial class SketchPage : PageWin
    {
        public SketchPage()
        {
            InitializeComponent();
            _import.Click += OnImport;
            _sketch.Tapped += OnSketchTapped;
        }
       
        void OnSketchTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
           
        }

        void OnImport(object sender, RoutedEventArgs e)
        {
            string proDef = "<Sketch><Node id=\"8c6056b1ddee44aca84144aeb8ab06d2\" title=\"填写请假单\" shape=\"开始\" left=\"160\" top=\"20\" width=\"160\" height=\"60\" /><Node id=\"c3f6523bc547438c91f50eda57e0762a\" title=\"完成\" shape=\"结束\" background=\"#FF9D9D9D\" borderbrush=\"#FF969696\" left=\"200\" top=\"520\" width=\"80\" height=\"60\" /><Node id=\"a2969f5c25384682be5dd7eb77830327\" title=\"部门主管审批\" shape=\"任务\" left=\"160\" top=\"120\" width=\"160\" height=\"60\" /><Node id=\"af26cb303450429eb1142b6fab285d0e\" title=\"总经理审批\" shape=\"任务\" left=\"160\" top=\"320\" width=\"160\" height=\"60\" /><Node id=\"e0e9563cd2494b1ea31c1b58545b536b\" title=\"通知请假人\" shape=\"任务\" left=\"160\" top=\"420\" width=\"160\" height=\"60\" /><Line id=\"435a22b8e5424a719b54b396aab97b45\" headerid=\"8c6056b1ddee44aca84144aeb8ab06d2\" bounds=\"230,80,20,40\" headerport=\"4\" tailid=\"a2969f5c25384682be5dd7eb77830327\" tailport=\"0\" /><Line id=\"4b5bd045011d4078b8f15b9592b11e01\" headerid=\"e0e9563cd2494b1ea31c1b58545b536b\" bounds=\"230,480,20,40\" headerport=\"4\" tailid=\"c3f6523bc547438c91f50eda57e0762a\" tailport=\"0\" /><Line id=\"43c068037ed941eab9e2888efda6f9ca\" headerid=\"af26cb303450429eb1142b6fab285d0e\" bounds=\"230,380,20,40\" headerport=\"4\" tailid=\"e0e9563cd2494b1ea31c1b58545b536b\" tailport=\"0\" /><Line id=\"f8ff1c851e4f4a01bfd83827ed03b6f2\" headerid=\"a2969f5c25384682be5dd7eb77830327\" bounds=\"320,150,40,310\" tailid=\"e0e9563cd2494b1ea31c1b58545b536b\" tailport=\"2\" /><Line id=\"af4686c663d14c3db30646918cfaca6e\" headerid=\"a2969f5c25384682be5dd7eb77830327\" bounds=\"230,180,20,140\" headerport=\"4\" tailid=\"af26cb303450429eb1142b6fab285d0e\" tailport=\"0\" /></Sketch>";
            _sketch.ReadXml(proDef);
            // 下面用于调试不同平台时，sketch内交互的效果。
            int y = 20;
            foreach (SNode item in _sketch.Container.Children.Where(p => p is SNode))
            {
               // 下面方法要在ios中才能调试使用
               // _sketch.BringSubviewToFront(item);
                
                item.Tapped += Item_Tapped;
                item.Click += Item_Click;
                Button button = new Button() { Content = y.ToString(),Height = 40};
                (_sketch.Container as Canvas).Children.Add(button);
                Canvas.SetTop(button, y += 60);
                button.Tapped += Button_Tapped;
            }
        }

        void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
           
        }

        void Item_Click(object sender, System.EventArgs e)
        {
            int i = _sketch.Container.Children.IndexOf(sender as SNode);
        }

        void Item_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            int i = _sketch.Container.Children.IndexOf(sender as SNode);
        }
    }
}
