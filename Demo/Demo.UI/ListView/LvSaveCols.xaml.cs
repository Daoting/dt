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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public partial class LvSaveCols : Win
    {
        public LvSaveCols()
        {
            InitializeComponent();

            _lv.Data = SampleData.CreatePersonsTbl(100);
        }
    }

    [LvCall]
    public class SaveColsCall
    {
        public static void Format(Env e)
        {
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                TextAlignment = TextAlignment.Center,
            };
            e.UI = tb;
            e.Set += c =>
            {
                tb.Text = c.Bool ? "男" : "女";
            };
        }
    }

}