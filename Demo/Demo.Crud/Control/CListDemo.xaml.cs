#region 文件描述
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
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Demo.Crud
{
    public partial class CListDemo : Win
    {
        public CListDemo()
        {
            InitializeComponent();
            _fv.Data = new Row
            {
                { "code", "汉族" },
                { "degree", "本科" },
                { "menu", typeof(string) },
                { "role", typeof(string) },
                { "parentid", typeof(long) },
                { "parent", typeof(string) },
                { "child1", typeof(string) },
                { "child2", typeof(string) },
                { "maxparent", typeof(string) },
                { "localmenu", typeof(string) },
            };
            CList ls = new CList();
            var a = ls.Sql;

        }
    }
}