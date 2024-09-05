#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-11 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.ComponentModel;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 普通对象的视图包装类
    /// </summary>
    internal class ObjectView : INotifyPropertyChanged
    {
        readonly List<PropertyView> _props = new List<PropertyView>();
        bool _isChanged;
        bool _delayCheckChanges;

        /// <summary>
        /// 内部属性值发生变化
        /// </summary>
        public event EventHandler<PropertyView> Changed;

        /// <summary>
        /// 属性 IsChanged 变化事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 获取对象的属性是否发生更改。
        /// </summary>
        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                if (_isChanged != value)
                {
                    _isChanged = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChanged"));
                }
            }
        }

        /// <summary>
        /// 提交自上次调用以来对该行进行的所有更改。
        /// </summary>
        public void AcceptChanges()
        {
            foreach (var prop in _props)
            {
                prop.AcceptChanges();
            }
            IsChanged = false;
        }

        /// <summary>
        /// 回滚自该表加载以来或上次调用 AcceptChanges 以来对该行进行的所有更改。
        /// </summary>
        public void RejectChanges()
        {
            _delayCheckChanges = true;
            foreach (var prop in _props)
            {
                prop.RejectChanges();
            }
            IsChanged = false;
            _delayCheckChanges = false;
        }

        /// <summary>
        /// 检查所有属性值是否有变化，同时更新IsChanged属性
        /// </summary>
        internal void CheckChanges()
        {
            if (_delayCheckChanges)
                return;

            bool changed = false;
            foreach (var prop in _props)
            {
                if (prop.IsChanged)
                {
                    changed = true;
                    break;
                }
            }
            IsChanged = changed;
        }

        internal void AddPropertyView(PropertyView p_prop)
        {
            _props.Add(p_prop);
        }

        /// <summary>
        /// 触发属性值变化事件
        /// </summary>
        /// <param name="p_view"></param>
        internal void OnValueChanged(PropertyView p_view)
        {
            Changed?.Invoke(this, p_view);
        }
    }
}
