using Dt.Base;
using Dt.Base.Tools;
using Microsoft.UI.Xaml.Controls;
using System.Text;

namespace Demo.UI
{
    public sealed partial class TestInvokeDemo : Win
    {
        public TestInvokeDemo()
        {
            InitializeComponent();
            
            _one.Output = new StringBuilder();
            
            var sb = new StringBuilder();
            _par.Output = sb;
            _child.Output = sb;

            sb = new StringBuilder();
            _first.Output = sb;
            _sec.Output = sb;
            _third.Output = sb;
        }

        void OnTest(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var txt = _one.Output.ToString() + "\r\n\r\n\r\n"
                + _par.Output.ToString() + "\r\n\r\n\r\n"
                + _first.Output.ToString();
            
            _result.Text = txt;
        }
    }
}
