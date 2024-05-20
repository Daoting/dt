#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Charts;
using Dt.Core;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class SparklineForm : UserControl
    {
        RptDesignInfo _info;
        RptSparkline _item;

        public SparklineForm(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
        }

        internal void LoadItem(RptSparkline p_item)
        {
            if (_item == p_item)
                return;

            _item = p_item;
            _item.Data.Changed -= OnValueChanged;
            _fv.Data = _item.Data;
            _item.Data.Changed += OnValueChanged;

            string dataSrc = _item.Data.Str("tbl");
            if (string.IsNullOrEmpty(dataSrc))
            {
                // 如果未选中唯一的数据源，默认选中
                var ls = _item.Root.Data.DataSet;
                if (ls != null && ls.Count == 1)
                {
                    // 使用initVal 避免重做撤消命令产生冗余操作。
                    _item.Data.InitVal("tbl", ls[0].Str("name"));
                    DataDropBox(ls[0].Str("name"));
                }
            }
            else
            {
                for (int i = 0; i < _item.Root.Data.DataSet.Count; i++)
                {
                    if (_item.Root.Data.DataSet[i].Str("name") == dataSrc)
                    {
                        DataDropBox(dataSrc);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 数据项发生变化事件响应函数。
        /// 注意：字段值变化未校验数据类型。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnValueChanged(object sender, Cell e)
        {
            if (e.ID == "tbl")
            {
                _item.Data["field"] = "";
                DataDropBox(e.Str);
            }
        }

        /// <summary>
        /// 根据数据源名称，初始化字段的数据源项目。
        /// </summary>
        /// <param name="p_dsName"></param>
        void DataDropBox(string p_dsName)
        {
            // 数据源改变，关系到列的内容全部改变。
            var dtCols = _item.Root.Data.GetColsData(p_dsName);
            ((CList)_fv["field"]).Data = dtCols;
        }
    }
}
