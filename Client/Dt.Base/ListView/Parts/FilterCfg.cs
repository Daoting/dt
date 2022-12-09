﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Reflection;
using Windows.UI.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 筛选框配置
    /// </summary>
    public class FilterCfg
    {
        readonly TextBox _tb;

        public FilterCfg()
        {
            _tb = new TextBox
            {
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = Res.浅灰2,
                Background = Res.WhiteBrush,
                Padding = new Thickness(10, 9, 10, 8),
                PlaceholderText = "筛选",
            };
            _tb.Loaded += (s, e) =>
            {
                // 确保每次移除又添加后能继续输入，非虚拟行或有分组时数据变化会清除所以元素
                _tb.Focus(FocusState.Programmatic);
                _tb.Select(_tb.Text.Length, 0);
            };
        }

        /// <summary>
        /// 获取设置筛选框水印提示
        /// </summary>
        public string Placeholder
        {
            get { return _tb.PlaceholderText; }
            set { _tb.PlaceholderText = value; }
        }

        /// <summary>
        /// 筛选框
        /// </summary>
        public TextBox FilterBox => _tb;

        /// <summary>
        /// 获取设置文本内容变化时是否执行查询，默认false
        /// </summary>
        public bool IsRealtime { get; set; }

        public string FilterCols { get; set; }

        /// <summary>
        /// 自定义筛选，外部重置数据源的方式
        /// </summary>
        public Action<string> MyFilter { get; set; }

        /// <summary>
        /// 默认过滤方法
        /// </summary>
        /// <param name="p_obj"></param>
        /// <returns></returns>
        internal bool DoDefaultFilter(object p_obj)
        {
            var txt = _tb.Text;
            if (p_obj is Row row)
            {
                foreach (var c in row.Cells)
                {
                    if (string.IsNullOrEmpty(FilterCols) || FilterCols.Contains(c.ID, StringComparison.OrdinalIgnoreCase))
                    {
                        if (c.GetVal<string>().Contains(txt))
                            return true;
                    }
                }
            }
            else
            {
                var props = p_obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var p in props)
                {
                    if (string.IsNullOrEmpty(FilterCols) || FilterCols.Contains(p.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        var val = p.GetValue(p_obj, null);
                        if (val != null && val.ToString().Contains(txt, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
