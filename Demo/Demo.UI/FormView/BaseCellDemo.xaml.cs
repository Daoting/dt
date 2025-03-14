﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Demo.UI
{
    public partial class BaseCellDemo : Win
    {
        Table _tbl;
        int _rowNum;

        public BaseCellDemo()
        {
            InitializeComponent();
            _fv.CellClick += (e) => FvDesignKit.LoadCellProps(e, _pbCell);
            _fv.Changed += (e) => CellDemoKit.OnChanged(_fv, e);
            _pbFv.Data = _fv;
            CreateTable();
            OnRow(null, null);
        }

        void CreateTable()
        {
            _tbl = new Table
            {
                { "txt" },
                { "txtinput" },
                { "txtlong" },
                { "txtpy" },
                { "tgtpy" },
                { "num", typeof(double) },
                { "pwd" },

                { "date", typeof(DateTime) },
                { "time", typeof(DateTime) },
                { "datetime", typeof(DateTime) },
                { "datestr" },
                { "datetouch", typeof(DateTime) },

                { "check", typeof(bool) },
                { "checkint", typeof(int) },
                { "checkstr" },
                { "checkswitch", typeof(bool) },
                { "checktitle", typeof(bool) },

                { "masknum", typeof(double) },
                { "maskregex" },
                { "maskreg" },
                { "masksimple" },
                { "maskdate", typeof(DateTime) },
                { "numcust", typeof(double) },

                { "tip" },
                { "tipdate", typeof(DateTime) },
                { "tipnum", typeof(double) },
                { "tipclick" },
            };
        }

        void OnRow(object sender, RoutedEventArgs e)
        {
            _rowNum++;
            DateTime now = DateTime.Now;
            _fv.Data = _tbl.AddRow(new
            {
                txt = $"文本{_rowNum}",
                txtlong = "\r\nab\r\ncd\r\n\r\n",
                txtpy = "",
                num = _rowNum,
                mask = "",
                pwd = "",

                date = now,
                time = now,
                datetime = now,
                datestr = "2015-06-19",
                datetouch = now,

                check = true,
                checkint = 1,
                checkstr = "男",
                checkswitch = true,
                checktitle = true,

                masknum = 12345d,
                maskregex = "321124321",
                maskreg = "321124321",
                masksimple = "321124321",
                maskdate = now,
                numcust = 4321,

                tip = "只读信息内容",
                tipdate = now,
                tipnum = 367d,
                tipclick = "共5人",
            });
        }

        void OnObj(object sender, RoutedEventArgs e)
        {
            _rowNum++;
            DateTime now = DateTime.Now;
            _fv.Data = new CellData
            {
                Txt = $"文本{_rowNum}",
                TxtLong = "长文本",
                Txtpy = "",
                Num = _rowNum,
                Pwd = "123",

                Date = now,
                Time = now,
                DateTime = now,
                DateStr = "2015-06-19",
                DateTouch = now,

                Check = true,
                CheckInt = 1,
                Checkstr = "男",
                CheckSwitch = true,

                masknum = 12345d,
                maskregex = "321124321",
                maskreg = "321124321",
                masksimple = "321124321",
                maskdate = now,
                numcust = 4321,

                Tip = "只读信息内容",
                TipDate = now,
                TipNum = 367d,
                TipClick = "共5人",
            };
        }

        void OnNull(object sender, RoutedEventArgs e)
        {
            _fv.Data = null;
        }

        void OnWarn(object sender, RoutedEventArgs e)
        {
            _fv["txt"].Warn("警告提示信息内容警告提示信息内容警告提示信息内容警告提示信息内容");
        }

        void OnMsg(object sender, RoutedEventArgs e)
        {
            _fv["date"].Msg("提示消息内容");
        }

        int _cnt = 0;
        void OnToggleLink()
        {
            _link.Title = $"切换内容{++_cnt}";
        }

        void OnPyChanged(FvCell arg1, object e)
        {
            if (e != null)
                _fv.Row["tgtpy"] = Kit.GetPinYin(e.ToString());
        }

        void OnNum(object sender, RoutedEventArgs e)
        {
            ((CText)_fv["txtinput"]).InputScope = new InputScope { Names = { new InputScopeName { NameValue = InputScopeNameValue.Number } } };
        }

        void OnPhoneNum(object sender, RoutedEventArgs e)
        {
            ((CText)_fv["txtinput"]).InputScope = new InputScope { Names = { new InputScopeName { NameValue = InputScopeNameValue.TelephoneNumber } } };
        }

        void OnDefaultInput(object sender, RoutedEventArgs e)
        {
            ((CText)_fv["txtinput"]).InputScope = new InputScope { Names = { new InputScopeName { NameValue = InputScopeNameValue.Default } } };
        }


        void BaseCellDemo_Click(object sender, TappedRoutedEventArgs e)
        {
            ((CTip)_fv["tipnum"]).Click -= BaseCellDemo_Click;
        }

        void OnTipClick(object sender, TappedRoutedEventArgs e)
        {
            ((CTip)_fv["tipnum"]).Click += BaseCellDemo_Click;
        }

        class CellData
        {
            public string Txt { get; set; }
            public string TxtInput { get; set; }
            public string TxtLong { get; set; }
            public string Txtpy { get; set; }
            public double Num { get; set; }
            public string Mask { get; set; }
            public string Pwd { get; set; }
            public string Html { get; set; }

            public DateTime Date { get; set; }
            public DateTime Time { get; set; }
            public DateTime DateTime { get; set; }
            public string DateStr { get; set; }
            public DateTime DateTouch { get; set; }

            public bool Check { get; set; }
            public int CheckInt { get; set; }
            public string Checkstr { get; set; }
            public bool CheckSwitch { get; set; }

            public double masknum { get; set; }
            public string maskregex { get; set; }
            public string maskreg { get; set; }
            public string masksimple { get; set; }
            public DateTime maskdate { get; set; }
            public double numcust { get; set; }

            public string Tip { get; set; }
            public DateTime TipDate { get; set; }
            public double TipNum { get; set; }
            public string TipClick { get; set; }
        }
    }
}