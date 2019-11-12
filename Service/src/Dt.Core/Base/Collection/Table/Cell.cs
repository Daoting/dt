#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 基础数据项，不可独立构造使用
    /// 注意：string类型时值空为string.Empty，使用时无需考虑null的情况
    /// </summary>
    public partial class Cell : INotifyPropertyChanged
    {
        #region 成员变量
        object _val = null;
        bool _isChanged = false;

        /// <summary>
        /// 属性 Val,IsChanged 变化事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region 构造方法
        /// <summary>
        /// 指定字段名和类型，只可通过Row构造
        /// </summary>
        /// <param name="p_row">所属行</param>
        /// <param name="p_cellName">字段名，不可为空，作为键值</param>
        /// <param name="p_cellType">数据项值的类型</param>
        internal Cell(Row p_row, string p_cellName, Type p_cellType)
        {
            Row = p_row;
            ID = p_cellName.ToLower();
            Type = p_cellType;
            if (Type == typeof(string))
                _val = OriginalVal = string.Empty;
            Row.Cells.Add(this);
        }

        /// <summary>
        /// 指定字段名和初始值
        /// </summary>
        /// <param name="p_cellName">字段名</param>
        /// <param name="p_value">初始值</param>
        /// <param name="p_row">所属行</param>
        internal Cell(string p_cellName, object p_value, Row p_row)
        {
            Row = p_row;
            ID = p_cellName.ToLower();
            if (p_value == null)
            {
                Type = typeof(string);
                _val = OriginalVal = string.Empty;
            }
            else
            {
                Type = p_value.GetType();
                _val = OriginalVal = p_value;
            }
            Row.Cells.Add(this);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取数据项字段名
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// 获取数据项值的类型
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 获取设置数据项值
        /// </summary>
        public object Val
        {
            get { return _val; }
            set { SetValueInternal(value, true); }
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
                // 将状态更改通知到Row
                if (_isChanged)
                {
                    Row.IsChanged = true;
                }
                else
                {
                    Row.CheckChanges();
                }
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
        /// 获取当前列所属的行
        /// </summary>
        public Row Row { get; }

        /// <summary>
        /// 获取或设置用于存储与此对象相关的任意对象值
        /// </summary>
        public object Tag { get; set; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 提交自上次调用以来对该数据项进行的所有更改。
        /// </summary>
        public void AcceptChanges()
        {
            if (OriginalVal != _val)
            {
                OriginalVal = _val;
                _isChanged = false;
                // 若变化，触发IsChanged属性变化，但不逐级状态更改通知
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChanged"));
            }
        }

        /// <summary>
        /// 回滚自该表加载以来或上次调用 AcceptChanges 以来对该数据项进行的所有更改。
        /// </summary>
        public void RejectChanges()
        {
            Val = OriginalVal;
        }

        /// <summary>
        /// 设置单元格默认值，恢复IsChanged=false状态
        /// </summary>
        /// <param name="p_val"></param>
        public void InitVal(object p_val)
        {
            SetValueInternal(p_val, false);
            OriginalVal = _val;
            IsChanged = false;
        }

        /// <summary>
        /// 获取当前数据项的值
        /// <para>string类型为null时返回string.Empty</para>
        /// <para>其它类型为null时返回default(T)，即引用类型返回 null，数值类型返回零</para>
        /// <para>有其它需要时请自行处理</para>
        /// <para>另外，只提供从其它类型到int,double,DateTime,string的类型转换</para>
        /// </summary>
        /// <typeparam name="T">将值转换为指定的类型</typeparam>
        /// <returns>指定类型的值</returns>
        public T GetVal<T>()
        {
            return GetValInternal<T>(_val);
        }

        /// <summary>
        /// 获取原始值
        /// <para>string类型为null时返回string.Empty</para>
        /// <para>其它类型为null时返回default(T)，即引用类型返回 null，数值类型返回零</para>
        /// <para>有其它需要时请自行处理</para>
        /// <para>另外，只提供从其它类型到int,double,DateTime,string的类型转换</para>
        /// </summary>
        /// <typeparam name="T">将值转换为指定的类型</typeparam>
        /// <returns>指定类型的值</returns>
        public T GetOriginalVal<T>()
        {
            return GetValInternal<T>(OriginalVal);
        }

        /// <summary>
        /// 重置数据项的值类型
        /// </summary>
        /// <param name="p_tgtType">转换到的目标类型</param>
        /// <returns>true 重置成功</returns>
        public bool ResetType(Type p_tgtType)
        {
            if (p_tgtType == null || p_tgtType == Type)
                return true;

            if (_val == null)
            {
                // 类型的默认值，等同于default
                _val = p_tgtType.IsValueType ? Activator.CreateInstance(p_tgtType) : null;
            }
            else if (_val.GetType() == p_tgtType)
            {
                // 类型相同
            }
            else if (p_tgtType.IsEnum)
            {
                // 枚举类型
                if (_val is string str)
                    _val = (str == string.Empty) ? Enum.ToObject(p_tgtType, 0) : Enum.Parse(p_tgtType, str);
                else
                    _val = Enum.ToObject(p_tgtType, _val);
            }
            else if (p_tgtType.IsGenericType && p_tgtType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // 可空类型
                Type tp = p_tgtType.GetGenericArguments()[0];
                if (tp != _val.GetType())
                    _val = Convert.ChangeType(_val, tp);
            }
            else if (_val is IConvertible)
            {
                _val = Convert.ChangeType(_val, p_tgtType);
            }
            else
            {
                throw new Exception($"【{ID}】列重置类型异常：无法将【{_val}】转换到【{p_tgtType.Name}】类型！");
            }

            Type = p_tgtType;
            OriginalVal = _val;
            IsChanged = false;
            return true;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 内部赋值
        /// </summary>
        /// <param name="p_val"></param>
        /// <param name="p_checkChange">是否逐级检查IsChanged状态</param>
        void SetValueInternal(object p_val, bool p_checkChange)
        {
            // 过滤多次赋值现象，当cell的类型为string时，在给val赋值null时，将一直保持初始的string.Empty的值
            if (object.Equals(_val, p_val)
                || (Type == typeof(string) && (string)_val == string.Empty && p_val == null))
                return;

            if (p_val == null)
            {
                // 类型的默认值，等同于default
                _val = Type.IsValueType ? Activator.CreateInstance(Type) : null;
            }
            else if (p_val.GetType() == Type)
            {
                // 类型相同
                _val = p_val;
            }
            else if (Type.IsEnum)
            {
                // 枚举类型
                if (p_val is string str)
                    _val = (str == string.Empty) ? Enum.ToObject(Type, 0) : Enum.Parse(Type, str);
                else
                    _val = Enum.ToObject(Type, p_val);
            }
            else if (Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // 可空类型
                Type tp = Type.GetGenericArguments()[0];
                if (tp != p_val.GetType())
                    _val = Convert.ChangeType(p_val, tp);
                else
                    _val = p_val;
            }
            else if (p_val is IConvertible)
            {
                // 可转换
                _val = Convert.ChangeType(p_val, Type);
            }
            else
            {
                throw new Exception($"【{ID}】列值转换异常：无法将【{p_val}】转换到【{Type.Name}】类型！");
            }

            // 向上逐级更新IsChanged状态
            if (p_checkChange)
                IsChanged = !object.Equals(_val, OriginalVal);

            // 触发属性变化事件
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Val"));

            // 数据项值改变时统一在Table和Row中触发事件
            Row.OnValueChanged(this);
            if (Row.Table != null)
                Row.Table.OnValueChanged(this);
        }

        /// <summary>
        /// 将值转换为指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_val"></param>
        /// <returns></returns>
        T GetValInternal<T>(object p_val)
        {
            Type type = typeof(T);

            // null时
            if (p_val == null)
            {
                // 字符串返回Empty！！！
                if (type == typeof(string))
                    return (T)(object)string.Empty;

                // 可空类型
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return (T)(object)null;

                return default(T);
            }

            // 若指定类型和当前类型匹配
            if (type == Type)
                return (T)p_val;

            // 可空类型
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return (T)p_val;

            // bool特殊处理1
            if (type == typeof(bool))
            {
                string val = p_val.ToString().ToLower();
                bool suc = (val == "1" || val == "true");
                return (T)(object)suc;
            }

            // 非null时执行转换
            if (p_val is IConvertible)
                return (T)Convert.ChangeType(p_val, type);

            throw new Exception($"【{ID}】列值转换异常：无法将【{p_val}】转换到【{type.Name}】类型！");
        }
        #endregion
    }
}
