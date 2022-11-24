#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml.Media;
using System.Reflection;
using Windows.UI.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 格取值赋值的参数
    /// </summary>
    public class Mid
    {
        #region 成员变量
        FvCellBind _bind;
        object _val;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_owner">所属绑定</param>
        /// <param name="p_value">当前格的值，可能ID是以.分隔的多级子属性</param>
        internal Mid(FvCellBind p_owner, object p_value)
        {
            _bind = p_owner;
            _val = p_value;
        }
        #endregion

        #region 值
        /// <summary>
        /// 获取Fv的数据源
        /// </summary>
        public object Data => _bind.Data;

        /// <summary>
        /// 获取Fv的Row数据源
        /// </summary>
        public Row Row => _bind.Data as Row;

        /// <summary>
        /// 列名(字段名)
        /// </summary>
        public string ID => _bind.Cell.ID;

        /// <summary>
        /// 根据当前格的字符串值，为null时返回string.Empty！！！
        /// </summary>
        /// <returns>字符串值</returns>
        public string Str => ObjectEx.To<string>(_val);

        /// <summary>
        /// 当前格的值是否为1或true
        /// </summary>
        /// <returns>true 表示列值为空</returns>
        public bool Bool => ObjectEx.To<bool>(_val);

        /// <summary>
        /// 当前格的double值，为null时返回零即default(double)！！！
        /// </summary>
        /// <returns>double值</returns>
        public double Double => ObjectEx.To<double>(_val);

        /// <summary>
        /// 当前格的整数值，为null时返回零即default(int)！！！
        /// </summary>
        /// <returns>整数值</returns>
        public int Int => ObjectEx.To<int>(_val);

        /// <summary>
        /// 当前格的64位整数值，为null时返回零即default(long)！！！
        /// </summary>
        /// <returns>整数值</returns>
        public long Long => ObjectEx.To<long>(_val);

        /// <summary>
        /// 当前格的日期值，为null时返回DateTime.MinValue，即default(DateTime)！！！
        /// </summary>
        /// <returns>日期值</returns>
        public DateTime Date => ObjectEx.To<DateTime>(_val);

        /// <summary>
        /// 获取当前格的值，可能ID是以.分隔的多级子属性
        /// </summary>
        public object Val => _val;

        /// <summary>
        /// 获取当前格值的类型
        /// </summary>
        public Type ValType => (Type)_bind.ConverterParameter;

        /// <summary>
        /// 获取格值
        /// </summary>
        /// <param name="p_id">ID</param>
        /// <returns></returns>
        public object this[string p_id]
        {
            get
            {
                object val = null;
                if (_bind.Data is Row dr && dr.Contains(p_id))
                {
                    // 从Row取
                    val = dr[p_id];
                }
                else if (!p_id.Contains('.'))
                {
                    // 对象属性
                    var pi = _bind.Data.GetType().GetProperty(p_id, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null)
                        val = pi.GetValue(_bind.Data);
                }
                else
                {
                    // 解析以.分隔的多级子属性
                    string[] arr = ID.Split('.');
                    var type = _bind.Data.GetType();
                    PropertyInfo pi = null;
                    object tgt = null;

                    for (int i = 0; i < arr.Length; i++)
                    {
                        pi = type.GetProperty(arr[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (pi != null)
                        {
                            if (i == arr.Length - 1)
                                break;

                            type = pi.PropertyType;
                            if (i == 0)
                                tgt = pi.GetValue(_bind.Data);
                            else if (tgt != null)
                                tgt = pi.GetValue(tgt);
                            else
                                return null;
                        }
                        else
                        {
                            return null;
                        }
                    }

                    if (tgt != null && pi != null)
                        val = pi.GetValue(tgt);
                }
                return val;
            }
            set
            {
                if (_bind.Data is Row dr && dr.Contains(p_id))
                {
                    dr[p_id] = value;
                }
                else if (!p_id.Contains('.'))
                {
                    var pi = _bind.Data.GetType().GetProperty(p_id, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null)
                        pi.SetValue(_bind.Data, value);
                }
                else if (_bind.Source is PropertyView pv)
                {
                    pv.Val = value;
                }
            }
        }

        /// <summary>
        /// 获取某格的值
        /// </summary>
        /// <typeparam name="T">将值转换为指定的类型</typeparam>
        /// <param name="p_id">ID</param>
        /// <returns>指定类型的值</returns>
        public T GetVal<T>(string p_id)
        {
            return ObjectEx.To<T>(this[p_id]);
        }
        #endregion

        #region 样式
        public FvCell Cell  => _bind.Cell;

        /// <summary>
        /// 显示警告提示信息
        /// </summary>
        /// <param name="p_msg"></param>
        public void Warn(string p_msg)
        {
            _bind.Cell.Warn(p_msg);
        }

        /// <summary>
        /// 显示提示消息
        /// </summary>
        /// <param name="p_msg"></param>
        public void Msg(string p_msg)
        {
            _bind.Cell.Msg(p_msg);
        }
        /// <summary>
        /// 获取设置单元格前景画刷
        /// </summary>
        public Brush Foreground
        {
            get { return _bind.Cell.Foreground; }
            set { _bind.Cell.Foreground = value; }
        }

        /// <summary>
        /// 获取设置单元格字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return _bind.Cell.FontWeight; }
            set { _bind.Cell.FontWeight = value; }
        }

        /// <summary>
        /// 获取设置单元格文本样式
        /// </summary>
        public FontStyle FontStyle
        {
            get { return _bind.Cell.FontStyle; }
            set { _bind.Cell.FontStyle = value; }
        }
        #endregion
    }
}
