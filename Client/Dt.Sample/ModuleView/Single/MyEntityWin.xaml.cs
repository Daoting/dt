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
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Sample
{
    public partial class MyEntityWin : Win
    {
        public MyEntityWin()
        {
            InitializeComponent();
        }

        public MyEntityList List => _list;

        public MyEntityForm Form => _form;

        public SearchMv Search => _search;

        void OnSearch(object sender, string e)
        {
            _list.OnSearch(e);
        }
    }
}