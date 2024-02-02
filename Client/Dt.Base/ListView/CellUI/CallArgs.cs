﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 绑定方式自定义单元格UI的参数
    /// </summary>
    public class CallArgs
    {
        #region 成员变量
        ViewItem _item;
        Dot _dot;
        #endregion

        #region 构造方法
        internal CallArgs(ViewItem p_item, Dot p_dot)
        {
            _item = p_item;
            _dot = p_dot;
        }
        #endregion

        #region 值
        /// <summary>
        /// 获取行的数据源
        /// </summary>
        public object Data => _item.Data;

        /// <summary>
        /// 获取Row数据源
        /// </summary>
        public Row Row => _item.Row;

        /// <summary>
        /// 列名(字段名)
        /// </summary>
        public string ID => _dot.ID;

        /// <summary>
        /// 获取设置格式串，null或空时按默认显示，如：时间格式、小数位格式、枚举类型名称
        /// <para>也是自定义单元格UI方法的参数</para>
        /// </summary>
        public string Format => _dot.Format;

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
        /// 获取当前单元格的值
        /// </summary>
        public object CellVal => _item[_dot.ID];

        /// <summary>
        /// 获取当前行某列的值
        /// </summary>
        /// <typeparam name="T">将值转换为指定的类型</typeparam>
        /// <param name="p_id">列名</param>
        /// <returns>指定类型的值</returns>
        public T GetVal<T>(string p_id)
        {
            return ObjectEx.To<T>(_item[p_id]);
        }
        #endregion

        #region 样式
        /// <summary>
        /// 当前单元格的Dot
        /// </summary>
        public Dot Dot => _dot;

        /// <summary>
        /// 当前行视图
        /// </summary>
        public ViewItem ViewItem => _item;

        /// <summary>
        /// 获取设置单元格前景画刷
        /// </summary>
        public Brush Foreground
        {
            get { return _dot.Foreground; }
            set { _dot.Foreground = value; }
        }

        /// <summary>
        /// 获取设置单元格背景画刷
        /// </summary>
        public Brush Background
        {
            get { return _dot.Background; }
            set { _dot.Background = value; }
        }

        /// <summary>
        /// 获取设置单元格字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return _dot.FontWeight; }
            set { _dot.FontWeight = value; }
        }

        /// <summary>
        /// 获取设置单元格文本样式
        /// </summary>
        public FontStyle FontStyle
        {
            get { return _dot.FontStyle; }
            set { _dot.FontStyle = value; }
        }

        /// <summary>
        /// 获取设置单元格文本大小
        /// </summary>
        public double FontSize
        {
            get { return _dot.FontSize; }
            set { _dot.FontSize = value; }
        }
        #endregion
    }
}
