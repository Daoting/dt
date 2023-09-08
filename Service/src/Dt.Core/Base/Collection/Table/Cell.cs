﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 基础数据项，不可独立构造使用
    /// 注意：string类型时值空为string.Empty，使用时无需考虑null的情况
    /// </summary>
#if !SERVER
    [Microsoft.UI.Xaml.Data.Bindable]
#endif
    public partial class Cell : INotifyPropertyChanged
    {
        #region 成员变量
        object _val;
        bool _isChanged = false;

        /// <summary>
        /// 属性 Val,IsChanged 变化事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region 构造方法
        /// <summary>
        /// 指定字段名、类型、初始值(可选)，只可通过Row构造
        /// </summary>
        /// <param name="p_row">所属行</param>
        /// <param name="p_cellName">字段名，不可为空，作为键值</param>
        /// <param name="p_cellType">数据项值的类型</param>
        /// <param name="p_value">初始值，null时取default值</param>
        internal Cell(Row p_row, string p_cellName, Type p_cellType, object p_value = null)
        {
            Row = p_row;
            ID = p_cellName;
            Type = p_cellType;
            _val = OriginalVal = GetValInternal(p_value, Type);
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
            set { SetValueInternal(value, false); }
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

        #region 值属性
        /// <summary>
        /// 获取当前新值的字符串值，为null时返回string.Empty！！！
        /// </summary>
        public string Str => GetVal<string>();

        /// <summary>
        /// 获取当前新值是否为1或true
        /// </summary>
        public bool Bool => GetVal<bool>();

        /// <summary>
        /// 获取当前新值的double值，为null时返回零即default(double)！！！
        /// </summary>
        public double Double => GetVal<double>();

        /// <summary>
        /// 获取当前新值的整数值，为null时返回零即default(int)！！！
        /// </summary>
        public int Int => GetVal<int>();

        /// <summary>
        /// 获取当前新值的64位整数值，为null时返回零即default(long)！！！
        /// </summary>
        public long Long => GetVal<long>();

        /// <summary>
        /// 获取当前新值的日期值，为null时返回DateTime.MinValue，即default(DateTime)！！！
        /// </summary>
        public DateTime Date => GetVal<DateTime>();

        /// <summary>
        /// 获取当前新字符串值是否为空
        /// </summary>
        public bool IsEmpty => GetVal<string>() == "";

        /// <summary>
        /// 获取当前新值是否为0
        /// </summary>
        public bool IsZero => GetVal<string>() == "0";

        /// <summary>
        /// 获取字符串按utf8编码的字节长度
        /// </summary>
        public int Utf8Length => Kit.GetUtf8Length(GetVal<string>());

        /// <summary>
        /// 获取字符串按gb2312编码的字节长度
        /// </summary>
        public int Gb2312Length => Kit.GetGb2312Length(GetVal<string>());

        /// <summary>
        /// 获取字符串按Unicode编码的字节长度
        /// </summary>
        public int UnicodeLength => Kit.GetUnicodeLength(GetVal<string>());
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
        /// 设置单元格默认值，恢复IsChanged的false状态，不调用Entity的Hook
        /// </summary>
        /// <param name="p_val"></param>
        public void InitVal(object p_val)
        {
            SetValueInternal(p_val, true);
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
            return (T)GetValInternal(_val, typeof(T));
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
            return (T)GetValInternal(OriginalVal, typeof(T));
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
            else if (p_tgtType.IsAssignableFrom(_val.GetType()))
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
        /// <param name="p_initVal">是否正在通过InitVal设置默认值，true时不检查IsChanged状态、不调用Entity的Hook</param>
        void SetValueInternal(object p_val, bool p_initVal)
        {
            // 过滤多次赋值现象，当cell的类型为string时，在给val赋值null时，将一直保持初始的string.Empty的值
            if (object.Equals(_val, p_val)
                || (Type == typeof(string) && (string)_val == string.Empty && p_val == null))
                return;

            // 类型不同时转换
            object val = GetValInternal(p_val, Type);

            // 调用Entity外部钩子，通常为业务校验，校验失败时触发异常使赋值失败
            bool hookChangedVal = false;
            if (!p_initVal
                && Row is Entity entity
                && entity.GetCellHook(ID) is Action<CellValChangingArgs> hook)
            {
                var args = new CellValChangingArgs(this, val);
#if SERVER
                // 服务端无绑定
                hook(args);
#else
                if (PropertyChanged == null)
                {
                    // 无绑定时不catch钩子抛出的异常，统一在未处理异常中提示警告信息
                    hook(args);
                }
                else
                {
                    try
                    {
                        // 绑定时钩子抛出异常会造成绑定失败：无法将值从目标保存回源，故先catch，然后触发Val属性变化重绑回原值
                        hook(args);
                    }
                    catch
                    {
                        // 通知UI重置原值
#if !WIN
                        // uno变态，必须完整执行一遍赋值，触发两次属性值变化，否则UI不重置！！！浪费半天
                        var old = OriginalVal;
                        OriginalVal = _val;
                        _val = p_val;
                        PropertyChanged(this, new PropertyChangedEventArgs("Val"));
                        _val = OriginalVal;
                        OriginalVal = old;
#endif
                        // 立即调用时无效！
                        Kit.RunInQueue(() => PropertyChanged(this, new PropertyChangedEventArgs("Val")));

                        // 直接返回，赋值失败
                        return;
                    }
                }
#endif
                // 钩子可能已修改值，用钩子的值
                if (val != args.NewVal)
                {
                    val = args.NewVal;
                    hookChangedVal = true;
                }
            }

            // 成功赋值
            _val = val;

            // 向上逐级更新IsChanged状态
            if (!p_initVal)
                IsChanged = !object.Equals(_val, OriginalVal);

            // 触发属性变化事件
            if (PropertyChanged != null)
            {
                if (hookChangedVal)
                {
                    // 外部钩子修改值后，立即调用时无效！
                    Kit.RunInQueue(() => PropertyChanged(this, new PropertyChangedEventArgs("Val")));
                }
                else
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Val"));
                }
            }

            // 数据项值改变时统一在Table和Row中触发事件
            Row.OnValueChanged(this);
            if (Row.Table != null)
                Row.Table.OnValueChanged(this);
        }

        /// <summary>
        /// 将值转换为指定类型
        /// </summary>
        /// <param name="p_val">值</param>
        /// <param name="p_tgtType">目标类型</param>
        /// <returns>转换结果</returns>
        internal static object GetValInternal(object p_val, Type p_tgtType)
        {
            // null时
            if (p_val == null)
            {
                // 字符串返回Empty！！！
                if (p_tgtType == typeof(string))
                    return string.Empty;

                // 值类型，可空类型Nullable<>也属值类型
                if (p_tgtType.IsValueType)
                    return Activator.CreateInstance(p_tgtType);

                return null;
            }

            // 若指定类型和当前类型匹配
            if (p_tgtType.IsAssignableFrom(p_val.GetType()))
                return p_val;

            // 空字符串转其他类型
            if (p_val is string && (string)p_val == string.Empty)
            {
                // 值类型，相当于default
                if (p_tgtType.IsValueType)
                    return Activator.CreateInstance(p_tgtType);
                return null;
            }

            // 枚举类型
            if (p_tgtType.IsEnum)
            {
                if (p_val is string str)
                    return (str == string.Empty) ? Enum.ToObject(p_tgtType, 0) : Enum.Parse(p_tgtType, str);
                return Enum.ToObject(p_tgtType, p_val);
            }

            // 可空类型
            if (p_tgtType.IsGenericType && p_tgtType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type tp = p_tgtType.GetGenericArguments()[0];
                // 参数类型不同时先转换
                if (tp != p_val.GetType())
                    return Convert.ChangeType(p_val, tp);
                return p_val;
            }

            if (p_tgtType == typeof(byte[]))
            {
                // base64 -> byte[]
                return Convert.FromBase64String(p_val.ToString());
            }

            // bool特殊处理
            if (p_tgtType == typeof(bool))
            {
                string val = p_val.ToString().ToLower();
                return (val == "1" || val == "true");
            }

            // 执行转换
            if (p_val is IConvertible)
                return Convert.ChangeType(p_val, p_tgtType);

            throw new Exception($"Cell列值转换异常：无法将【{p_val}】转换到【{p_tgtType.Name}】类型！");
        }
        #endregion
    }

    /// <summary>
    /// Cell值变化时提供给钩子回调的参数
    /// </summary>
    public class CellValChangingArgs
    {
        Cell _cell;
        object _newVal;

        internal CellValChangingArgs(Cell p_cell, object p_newVal)
        {
            _cell = p_cell;
            _newVal = p_newVal;
        }

        /// <summary>
        /// 获取设置Cell当前的新值
        /// </summary>
        public object NewVal
        {
            get { return _newVal; }
            set
            {
                if (_newVal != value)
                {
                    // 类型不同时转换
                    _newVal = Cell.GetValInternal(value, _cell.Type);
                }
            }
        }

        /// <summary>
        /// 获取当前新值的字符串值，为null时返回string.Empty！！！
        /// </summary>
        public string Str => GetVal<string>();

        /// <summary>
        /// 获取当前新值是否为1或true
        /// </summary>
        public bool Bool => GetVal<bool>();

        /// <summary>
        /// 获取当前新值的double值，为null时返回零即default(double)！！！
        /// </summary>
        public double Double => GetVal<double>();

        /// <summary>
        /// 获取当前新值的整数值，为null时返回零即default(int)！！！
        /// </summary>
        public int Int => GetVal<int>();

        /// <summary>
        /// 获取当前新值的64位整数值，为null时返回零即default(long)！！！
        /// </summary>
        public long Long => GetVal<long>();

        /// <summary>
        /// 获取当前新值的日期值，为null时返回DateTime.MinValue，即default(DateTime)！！！
        /// </summary>
        public DateTime Date => GetVal<DateTime>();

        /// <summary>
        /// 获取当前新字符串值是否为空
        /// </summary>
        public bool IsEmpty => GetVal<string>() == "";

        /// <summary>
        /// 获取当前新值是否为0
        /// </summary>
        public bool IsZero => GetVal<string>() == "0";

        /// <summary>
        /// 获取字符串按utf8编码的字节长度
        /// </summary>
        public int Utf8Length => Kit.GetUtf8Length(GetVal<string>());

        /// <summary>
        /// 获取字符串按gb2312编码的字节长度
        /// </summary>
        public int Gb2312Length => Kit.GetGb2312Length(GetVal<string>());

        /// <summary>
        /// 获取字符串按Unicode编码的字节长度
        /// </summary>
        public int UnicodeLength => Kit.GetUnicodeLength(GetVal<string>());

        /// <summary>
        /// 获取当前新值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetVal<T>()
        {
            return (T)Cell.GetValInternal(_newVal, typeof(T));
        }
    }
}
