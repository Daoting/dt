#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class FvObjData : Win
    {
        FvObjCls _obj;

        public FvObjData()
        {
            InitializeComponent();
            _obj = new FvObjCls(this);
            _fv.Data = _obj;
        }

        void OnTgt1(object sender, RoutedEventArgs e)
        {
            _fv.Data = _tb;
        }

        void OnTgt2(object sender, RoutedEventArgs e)
        {
            _fv.Data = _obj;
        }

        void OnHeight(object sender, RoutedEventArgs e)
        {
            if (_fv.Data == _tb)
            {
                _tb.Height = _tb.ActualHeight - 1;
                Output();
            }
            else
            {
                _obj.ActualHeight++;
            }
        }

        void OnFontSize(object sender, RoutedEventArgs e)
        {
            if (_fv.Data == _tb)
            {
                _tb.FontSize++;
                Output();
            }
            else
            {
                _obj.FontSize--;
            }
        }

        void OnName(object sender, RoutedEventArgs e)
        {
            if (_fv.Data == _tb)
            {
                _tb.Name += "1";
                Output();
            }
            else
            {
                _obj.Name += "2";
            }
        }

        void Output()
        {
            _tbOut.Text = $"目标文本框\r\n高度：{_tb.ActualHeight}\r\n字体大小：{_tb.FontSize}\r\n名称：{_tb.Name}";
        }

        public class FvObjCls : ViewModel
        {
            FvObjData _owner;
            Thickness _borderThickness;
            double _actualHeight;
            double _fontSize;
            string _name;
            HorizontalAlignment _horizontalAlignment;
            bool _isEnabled;

            public FvObjCls(FvObjData p_owner)
            {
                _owner = p_owner;
                _borderThickness = new Thickness(20);
                _actualHeight = 50;
                _fontSize = 20;
                _name = "非绑定";
            }

            public Thickness BorderThickness
            {
                get { return _borderThickness; }
            }

            public double ActualHeight
            {
                get { return _actualHeight; }
                set
                {
                    if (SetProperty(ref _actualHeight, value))
                        Output();
                }
            }

            public double FontSize
            {
                get { return _fontSize; }
                set
                {
                    if (SetProperty(ref _fontSize, value))
                        Output();
                }
            }

            public string Name
            {
                get { return _name; }
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        Output();
                    }
                }
            }

            public HorizontalAlignment HorizontalAlignment
            {
                get { return _horizontalAlignment; }
                set
                {
                    if (SetProperty(ref _horizontalAlignment, value))
                        Output();
                }
            }

            public bool IsEnabled
            {
                get { return _isEnabled; }
                set
                {
                    if (SetProperty(ref _isEnabled, value))
                        Output();
                }
            }

            void Output()
            {
                _owner._tbOut.Text = $"普通对象\r\n高度：{_actualHeight}\r\n字体大小：{_fontSize}\r\n名称：{_name}\r\n对齐方式：{_horizontalAlignment}\r\n可用：{_isEnabled}";
            }
        }
    }
}