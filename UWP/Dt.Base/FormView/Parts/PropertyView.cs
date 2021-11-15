#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.ComponentModel;
using System.Reflection;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 对象属性的视图包装类
    /// </summary>
    internal class PropertyView : INotifyPropertyChanged, ICell
    {
        ObjectView _objView;
        PropertyInfo _info;
        object _tgt;
        bool _isChanged = false;

        internal PropertyView(ObjectView p_objView, PropertyInfo p_info, object p_tgt)
        {
            _objView = p_objView;
            _info = p_info;
            _tgt = p_tgt;

            OriginalVal = Val;
            _objView.AddPropertyView(this);
        }

        /// <summary>
        /// 属性 Val,IsChanged 变化事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 获取数据项字段名
        /// </summary>
        public string ID
        {
            get { return _info.Name; }
        }

        /// <summary>
        /// 获取数据项值的类型
        /// </summary>
        public Type Type
        {
            get { return _info.PropertyType; }
        }

        /// <summary>
        /// 获取设置数据项值
        /// </summary>
        public object Val
        {
            get { return _info.GetValue(_tgt); }
            set
            {
                if (object.Equals(_info.GetValue(_tgt), value))
                    return;

                object val;
                if (value != null && !_info.PropertyType.IsAssignableFrom(value.GetType()))
                {
                    try
                    {
                        val = Convert.ChangeType(value, _info.PropertyType);
                    }
                    catch
                    {
                        throw new Exception($"PropertyView赋值异常：无法将【{value}】转换到【{_info.PropertyType}】类型！");
                    }
                }
                else
                {
                    val = value;
                }
                _info.SetValue(_tgt, val);

                IsChanged = !object.Equals(val, OriginalVal);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Val"));

                // 在上级中触发值改变事件
                _objView.OnValueChanged(this);
            }
        }

        /// <summary>
        /// 获取当前数据项是否已发生更改。
        /// </summary>
        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                if (_isChanged == value)
                    return;

                _isChanged = value;
                // 将状态更改通知到上级
                if (_isChanged)
                    _objView.IsChanged = true;
                else
                    _objView.CheckChanges();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChanged"));
            }
        }

        /// <summary>
        /// 获取该数据项未发生更改前的值
        /// </summary>
        /// <remark>
        /// 如果调用 AcceptChanges 方法，则 originalValue 属性的值将变成当前 Val 属性的值
        /// 如果调用了 RejectChanges 方法，则该数据项的值将变为当前 OriginalVal 属性的值
        /// </remark>
        public object OriginalVal { get; set; }

        /// <summary>
        /// 提交自上次调用以来对该数据项进行的所有更改
        /// </summary>
        public void AcceptChanges()
        {
            object val = Val;
            if (OriginalVal != val)
            {
                OriginalVal = val;
                _isChanged = false;
                // 若变化，触发IsChanged属性变化，但不逐级状态更改通知
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChanged"));
            }
        }

        /// <summary>
        /// 回滚自该表加载以来或上次调用 AcceptChanges 以来对该数据项进行的所有更改
        /// </summary>
        public void RejectChanges()
        {
            Val = OriginalVal;
        }

        /// <summary>
        /// 获取当前数据项的值
        /// </summary>
        /// <typeparam name="T">将值转换为指定的类型</typeparam>
        /// <returns>指定类型的值</returns>
        public T GetVal<T>()
        {
            object val = Val;
            if (val == null)
                return default(T);

            // 若指定类型和当前类型匹配
            Type type = typeof(T);
            if (type == _info.PropertyType)
                return (T)val;

            // 类型不同时执行转换
            object result = null;
            try
            {
                result = Convert.ChangeType(val, type);
            }
            catch
            {
                throw new Exception($"PropertyView异常：无法将【{val}】转换到【{type}】类型！");
            }
            return (T)result;
        }

        /// <summary>
        /// ID是否匹配给定列表中的任一名称，忽略大小写
        /// </summary>
        /// <param name="p_ids">一个或多个id名称</param>
        /// <returns>true 匹配任一</returns>
        public bool IsID(params string[] p_ids)
        {
            if (p_ids == null || p_ids.Length == 0)
                return false;

            var propID = _info.Name;
            foreach (var id in p_ids)
            {
                if (string.Equals(id, propID, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
