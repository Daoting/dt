﻿#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace $rootnamespace$
{
    public sealed partial class $maincls$$relatedcls$List : Mv
    {
        long _id;

        public $maincls$$relatedcls$List()
        {
            InitializeComponent();
            Menu["移除"].Bind(IsEnabledProperty, _lv, "HasSelected");
        }

        public void Update(long p_id)
        {
            _id = p_id;
            Menu["添加"].IsEnabled = true;
            Refresh();
        }

        public void Clear()
        {
            _id = -1;
            Menu["添加"].IsEnabled = false;
            _lv.Data = null;
        }

        void Refresh()
        {
            //_lv.Data = await AtCm.Query("a-关联b", new { id = _id });
        }

        void OnAdd(object sender, Mi e)
        {
            
        }

        void OnRemove(object sender, Mi e)
        {
            DoRemove(_lv.SelectedRows);
        }

        void OnRemove2(object sender, Mi e)
        {
            if (_lv.SelectionMode == Base.SelectionMode.Multiple)
                DoRemove(_lv.SelectedRows);
            else
                DoRemove(new List<Row> { e.Row });
        }

        void DoRemove(IEnumerable<Row> p_rows)
        {
            
        }

        void OnSelectAll(object sender, Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Multiple;
            Menu.Hide("添加", "选择");
            Menu.Show("移除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Single;
            Menu.Show("添加", "选择");
            Menu.Hide("移除", "全选", "取消");
        }

        $maincls$Win _win => ($maincls$Win)_tab.OwnWin;
    }
}
