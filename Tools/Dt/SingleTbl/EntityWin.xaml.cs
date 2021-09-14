#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace $rootnamespace$
{
    public partial class $entityname$Win : Win
    {
        public $entityname$Win()
        {
            InitializeComponent();
        }

        public $entityname$List List => _list;

        public $entityname$Form Form => _form;

        public SearchMv Search => _search;

        void OnSearch(object sender, string e)
        {
            _list.OnSearch(e);
        }
    }
}