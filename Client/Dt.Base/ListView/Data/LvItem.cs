#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 视图行，Row/object 和 ListRow/GridRow 的中间对象
    /// </summary>
    public partial class LvItem : ViewItem
    {
        #region 静态内容
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(LvItem),
            new PropertyMetadata(false, OnIsSelectedChanged));

        static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LvItem)d).OnPropertyChanged("IsSelected");
        }
        #endregion

        #region 成员变量
        Lv _owner;
        int _index;
        Dictionary<string, object> _cacheUI;
        #endregion

        #region 构造方法
        public LvItem(Lv p_lv, object p_data, int p_index)
            : base(p_data)
        {
            _owner = p_lv;
            _index = p_index;
        }
        #endregion

        /// <summary>
        /// 获取当前行是否为选择状态
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// 获取行索引
        /// </summary>
        public int Index
        {
            get { return _index; }
            internal set
            {
                if (_index != value)
                {
                    _index = value;
                    OnPropertyChanged("Index");
                    OnPropertyChanged("FullIndex");
                }
            }
        }

        /// <summary>
        /// 获取完整行索引
        /// </summary>
        public string FullIndex
        {
            get { return $"{_index}/{_owner.Rows.Count}"; }
        }

        /// <summary>
        /// 宿主
        /// </summary>
        protected override IViewItemHost Host
        {
            get { return _owner; }
        }

        /// <summary>
        /// 单击行
        /// </summary>
        internal void OnClick()
        {
            if (_owner.SelectionMode == SelectionMode.Multiple)
            {
                // 多选时切换选择状态
                var old = _owner.SelectedLvItems.LastOrDefault();
                if (IsSelected)
                    _owner.SelectedLvItems.Remove(this);
                else
                    _owner.SelectedLvItems.Add(this);
                _owner.OnItemClick(Data, (old != null) ? old.Data : null);
            }
            else
            {
                // 单选
                if (IsSelected)
                {
                    _owner.OnItemClick(Data, Data);
                }
                else
                {
                    object old = _owner.SelectedItem;
                    _owner.OnToggleSelected(this);
                    _owner.OnItemClick(Data, old);
                }
            }
        }

        internal void OnDoubleClick()
        {
            _owner.OnItemDoubleClick(Data);
        }

        #region 重写
        protected override void OnInit()
        {
            if (_owner.IsVir)
                _cacheUI = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        protected override void AddCacheUI(string p_key, object p_ui)
        {
            if (_cacheUI != null)
                _cacheUI[p_key] = p_ui;
        }

        protected override bool GetCacheUI(string p_key, out object p_ui)
        {
            if (_cacheUI != null && _cacheUI.TryGetValue(p_key, out p_ui))
                return true;

            p_ui = null;
            return false;
        }

        protected override void ClearCacheUI()
        {
            // 清除缓存，再次绑定时重新生成
            if (_cacheUI != null)
                _cacheUI.Clear();
        }
        #endregion
    }
}