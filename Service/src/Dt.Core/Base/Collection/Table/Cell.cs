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
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 基础数据项，不可独立构造使用
    /// 注意：string类型时值空为string.Empty，使用时无需考虑null的情况
    /// </summary>
#if !SERVER
    [Windows.UI.Xaml.Data.Bindable]
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
        /// 获取设置数据项值，主要绑定用！非绑定时设置值请使用 SetVal 方法，内部在处理Hook异常时有区别！！！
        /// </summary>
        public object Val
        {
            get { return _val; }
            set { SetValueInternal(value, true, true); }
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

        /// <summary>
        /// 设置值时的外部钩子，通常为业务处理方法，可通过触发异常的方式使赋值失败
        /// 钩子方法规范：
        /// 私有方法，SetXXX中的XXX为Cell.ID
        /// 一个入参，和Cell.Type相同
        /// 无返回值，不允许外部动态改变赋值，因出问题不好查找，绑定时UI也未回绑
        /// </summary>
        public MethodInfo Hook { get; set; }
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
            SetValueInternal(p_val, false, false);
            OriginalVal = _val;
            IsChanged = false;
        }

        /// <summary>
        /// 非绑定时设置单元格值，和Val属性在处理Hook异常时有区别！！！
        /// </summary>
        /// <param name="p_val"></param>
        public void SetVal(object p_val)
        {
            SetValueInternal(p_val, true, false);
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

        /// <summary>
        /// ID是否匹配给定列表中的任一名称，忽略大小写
        /// </summary>
        /// <param name="p_ids">一个或多个id名称</param>
        /// <returns>true 匹配任一</returns>
        public bool IsID(params string[] p_ids)
        {
            if (p_ids == null || p_ids.Length == 0)
                return false;

            foreach (var id in p_ids)
            {
                if (string.Equals(id, ID, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 内部赋值
        /// </summary>
        /// <param name="p_val"></param>
        /// <param name="p_checkChange">是否逐级检查IsChanged状态</param>
        /// <param name="p_isBinding">是否为绑定状态</param>
        void SetValueInternal(object p_val, bool p_checkChange, bool p_isBinding)
        {
            // 过滤多次赋值现象，当cell的类型为string时，在给val赋值null时，将一直保持初始的string.Empty的值
            if (object.Equals(_val, p_val)
                || (Type == typeof(string) && (string)_val == string.Empty && p_val == null))
                return;

            // 类型不同时转换
            object val = GetValInternal(p_val, Type);

            // 外部钩子通常为业务校验、领域事件等，校验失败时触发异常使赋值失败
            if (Hook != null)
            {
#if SERVER
                // 服务端无绑定
                Hook.Invoke(Row, new object[] { val });
#else
                if (!p_isBinding)
                {
                    // 无绑定时不catch钩子抛出的异常，统一在未处理异常中提示警告信息
                    Hook.Invoke(Row, new object[] { val });
                }
                else
                {
                    try
                    {
                        // 绑定时钩子抛出的异常被UWP内部catch，无法统一提示警告信息，故先catch
                        Hook.Invoke(Row, new object[] { p_val });
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is KnownException kex)
                            Kit.Warn(kex.Message);
                        else
                            Kit.Warn(ex.Message);

                        // 通知UI重置原值
                        if (PropertyChanged != null)
                        {
#if !UWP
                            // uno变态，必须完整执行一遍赋值，触发两次属性值变化，否则UI不重置！！！浪费半天
                            var old = OriginalVal;
                            OriginalVal = _val;
                            _val = p_val;
                            PropertyChanged(this, new PropertyChangedEventArgs("Val"));
                            _val = OriginalVal;
                            OriginalVal = old;
#endif
                            // 立即调用时无效！
                            Kit.RunAsync(() => PropertyChanged(this, new PropertyChangedEventArgs("Val")));
                        }
                        // 直接返回，赋值失败
                        return;
                    }
                }
#endif
            }

            // 成功赋值
            _val = val;

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
        /// <param name="p_val">值</param>
        /// <param name="p_tgtType">目标类型</param>
        /// <returns>转换结果</returns>
        object GetValInternal(object p_val, Type p_tgtType)
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

            throw new Exception($"【{ID}】列值转换异常：无法将【{p_val}】转换到【{p_tgtType.Name}】类型！");
        }
        #endregion
    }
}
