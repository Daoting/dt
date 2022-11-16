#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自定义单元格UI的参数
    /// </summary>
    public class Env
    {
        #region 成员变量
        ViewItem _item;
        Dot _dot;
        #endregion

        #region 构造方法
        internal Env(ViewItem p_item, Dot p_dot)
        {
            _item = p_item;
            _dot = p_dot;
        }
        #endregion

        #region UI
        /// <summary>
        /// 自定义单元格UI
        /// </summary>
        public UIElement UI { get; set; }

        /// <summary>
        /// 格式串，自定义单元格UI方法的参数
        /// </summary>
        public string Format => _dot.Format;

        /// <summary>
        /// 列名(字段名)
        /// </summary>
        public string ID => _dot.ID;

        /// <summary>
        /// 当前行视图
        /// </summary>
        public ViewItem ViewItem => _item;

        /// <summary>
        /// 当前单元格的Dot
        /// </summary>
        public Dot Dot => _dot;

        /// <summary>
        /// 获取设置单元格前景画刷
        /// </summary>
        public Brush Foreground
        {
            get { return Root == null ? _dot.Foreground : Root.Foreground; }
            set { GetRoot().Foreground = value; }
        }

        /// <summary>
        /// 获取设置单元格背景画刷
        /// </summary>
        public Brush Background
        {
            get { return Root == null ? _dot.Background : Root.Background; }
            set { GetRoot().Background = value; }
        }

        /// <summary>
        /// 获取设置单元格字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return Root == null ? _dot.FontWeight : Root.FontWeight; }
            set { GetRoot().FontWeight = value; }
        }

        /// <summary>
        /// 获取设置单元格文本样式
        /// </summary>
        public FontStyle FontStyle
        {
            get { return Root == null ? _dot.FontStyle : Root.FontStyle; }
            set { GetRoot().FontStyle = value; }
        }

        /// <summary>
        /// 获取设置单元格文本大小
        /// </summary>
        public double FontSize
        {
            get { return Root == null ? _dot.FontSize : Root.FontSize; }
            set { GetRoot().FontSize = value; }
        }

        /// <summary>
        /// 获取设置单元格内边距
        /// </summary>
        public Thickness Padding
        {
            get { return Root == null ? _dot.Padding : Root.Padding; }
            set { GetRoot().Padding = value; }
        }

        /// <summary>
        /// 获取设置单元格边距
        /// </summary>
        public Thickness Margin
        {
            get { return Root == null ? _dot.Margin : Root.Margin; }
            set { GetRoot().Margin = value; }
        }

        /// <summary>
        /// 以默认方式生成单元格UI
        /// </summary>
        /// <returns></returns>
        public UIElement CreateDefaultUI()
        {
            return _item.CreateDefaultUI(_dot);
        }
        #endregion

        #region 数据源
        /// <summary>
        /// 获取行的数据源
        /// </summary>
        public object Data => _item.Data;

        /// <summary>
        /// 获取Row数据源
        /// </summary>
        public Row Row => _item.Row;

        /// <summary>
        /// 获取当前单元格的值
        /// </summary>
        public object CellVal => _item[_dot.ID];

        /// <summary>
        /// 根据当前单元格的字符串值，为null时返回string.Empty！！！
        /// </summary>
        /// <returns>字符串值</returns>
        public string Str => GetVal<string>(_dot.ID);

        /// <summary>
        /// 当前单元格的值是否为1或true
        /// </summary>
        /// <returns>true 表示列值为空</returns>
        public bool Bool => GetVal<bool>(_dot.ID);

        /// <summary>
        /// 当前单元格的double值，为null时返回零即default(double)！！！
        /// </summary>
        /// <returns>double值</returns>
        public double Double => GetVal<double>(_dot.ID);

        /// <summary>
        /// 当前单元格的整数值，为null时返回零即default(int)！！！
        /// </summary>
        /// <returns>整数值</returns>
        public int Int => GetVal<int>(_dot.ID);

        /// <summary>
        /// 当前单元格的64位整数值，为null时返回零即default(long)！！！
        /// </summary>
        /// <returns>整数值</returns>
        public long Long => GetVal<long>(_dot.ID);

        /// <summary>
        /// 当前单元格的日期值，为null时返回DateTime.MinValue，即default(DateTime)！！！
        /// </summary>
        /// <returns>日期值</returns>
        public DateTime Date => GetVal<DateTime>(_dot.ID);

        /// <summary>
        /// 获取当前行某列的值
        /// </summary>
        /// <typeparam name="T">将值转换为指定的类型</typeparam>
        /// <param name="p_id">列名</param>
        /// <returns>指定类型的值</returns>
        public T GetVal<T>(string p_id)
        {
            object val = _item[p_id];
            Type tgtType = typeof(T);

            // null时
            if (val == null)
            {
                // 字符串返回Empty！！！
                if (tgtType == typeof(string))
                    return (T)(object)string.Empty;

                // 值类型，可空类型Nullable<>也是值类型
                if (tgtType.IsValueType)
                    return (T)Activator.CreateInstance(tgtType);

                return default(T);
            }

            // 若指定类型和当前类型匹配
            if (tgtType.IsAssignableFrom(val.GetType()))
                return (T)val;

            // 枚举类型
            if (tgtType.IsEnum)
            {
                if (val is string str)
                    return (str == string.Empty) ? (T)Enum.ToObject(tgtType, 0) : (T)Enum.Parse(tgtType, str);
                return (T)Enum.ToObject(tgtType, val);
            }

            // 可空类型
            if (tgtType.IsGenericType && tgtType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type tp = tgtType.GetGenericArguments()[0];
                // 参数类型不同时先转换
                if (tp != val.GetType())
                    return (T)Convert.ChangeType(val, tp);
                return (T)val;
            }

            // bool特殊处理
            if (tgtType == typeof(bool))
            {
                string str = val.ToString().ToLower();
                return (T)(object)(str == "1" || str == "true");
            }

            // 执行转换
            if (val is IConvertible)
                return (T)Convert.ChangeType(val, tgtType);

            throw new Exception($"【{p_id}】列值转换异常：无法将【{val}】转换到【{tgtType.Name}】类型！");
        }
        #endregion

        #region 内部
        /// <summary>
        /// 承载样式的根元素，设置样式时自动创建
        /// </summary>
        internal ContentPresenter Root { get; set; }

        ContentPresenter GetRoot()
        {
            if (Root == null)
                Root = new ContentPresenter();
            return Root;
        }
        #endregion
    }
}
