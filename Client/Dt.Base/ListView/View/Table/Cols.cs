#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Windows.UI;
using Windows.UI.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列集合
    /// </summary>
    public class Cols : Nl<object>
    {
        /// <summary>
        /// 列宽变化事件
        /// </summary>
        public event Action ColWidthChanged;

        /// <summary>
        /// 列集合变化、Col.ID变化、显示/隐藏列 等引起的重新加载事件
        /// </summary>
        public event Action Reloading;

        /// <summary>
        /// 列布局变化事件，调整列宽、列序、隐藏列后触发
        /// </summary>
        public event Action LayoutChanged;
        
        /// <summary>
        /// 获取设置点击列头是否可以排序
        /// </summary>
        public bool AllowSorting { get; set; } = true;

        /// <summary>
        /// 获取设置是否隐藏行号，默认false
        /// </summary>
        public bool HideIndex { get; set; }

        #region 显示/隐藏列
        /// <summary>
        /// 隐藏名称列表中的列
        /// </summary>
        /// <param name="p_ids">列ID</param>
        public void Hide(params string[] p_ids)
        {
            _isVisibleChanging = true;
            try
            {
                foreach (string name in p_ids)
                {
                    if (string.IsNullOrEmpty(name))
                        continue;

                    var item = (from col in this
                                let c = col as Col
                                where c != null && string.Equals(name, c.ID, StringComparison.OrdinalIgnoreCase)
                                select c).FirstOrDefault();
                    if (item != null && item.Visibility == Visibility.Visible)
                        item.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
            finally
            {
                _isVisibleChanging = false;
                OnReloading();
            }
        }

        /// <summary>
        /// 除显示名称列表中的列外，其它都隐藏，列表空时隐藏所有！
        /// </summary>
        /// <param name="p_ids">无值时隐藏所有列</param>
        public void HideExcept(params string[] p_ids)
        {
            _isVisibleChanging = true;
            try
            {
                foreach (var col in this.OfType<Col>())
                {
                    if (p_ids.Contains(col.ID))
                    {
                        col.Visibility = Visibility.Visible;
                    }
                    else if (col.Visibility == Visibility.Visible)
                    {
                        col.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch { }
            finally
            {
                _isVisibleChanging = false;
                OnReloading();
            }
        }

        /// <summary>
        /// 显示名称列表中的列
        /// </summary>
        /// <param name="p_ids">列ID</param>
        public void Show(params string[] p_ids)
        {
            _isVisibleChanging = true;
            try
            {
                foreach (string name in p_ids)
                {
                    if (string.IsNullOrEmpty(name))
                        continue;

                    var item = (from col in this.OfType<Col>()
                                where string.Equals(name, col.ID, StringComparison.OrdinalIgnoreCase)
                                select col).FirstOrDefault();
                    if (item != null && item.Visibility == Visibility.Collapsed)
                        item.Visibility = Visibility.Visible;
                }
            }
            catch { }
            finally
            {
                _isVisibleChanging = false;
                OnReloading();
            }
        }

        /// <summary>
        /// 除隐藏名称列表中的列外，其它都显示，列表空时显示所有！
        /// </summary>
        /// <param name="p_ids">无值时显示所有列</param>
        public void ShowExcept(params string[] p_ids)
        {
            _isVisibleChanging = true;
            try
            {
                foreach (var col in this.OfType<Col>())
                {
                    if (p_ids.Contains(col.ID))
                    {
                        col.Visibility = Visibility.Collapsed;
                    }
                    else if (col.Visibility == Visibility.Collapsed)
                    {
                        col.Visibility = Visibility.Visible;
                    }
                }
            }
            catch { }
            finally
            {
                _isVisibleChanging = false;
                OnReloading();
            }
        }
        #endregion
        
        /// <summary>
        /// 列总宽
        /// </summary>
        internal double TotalWidth { get; set; }

        /// <summary>
        /// 列宽失效，触发重新测量布局
        /// </summary>
        internal void OnColWidthChanged()
        {
            FixWidth(_maxWidth);
            ColWidthChanged?.Invoke();
        }
        
        /// <summary>
        /// 列集合变化、Col.ID变化等引起的重新加载
        /// </summary>
        internal void OnReloading()
        {
            if (!_isVisibleChanging)
            {
                FixWidth(_maxWidth);
                Reloading?.Invoke();
            }
        }

        /// <summary>
        /// 调整列宽、列序、隐藏列后触发事件
        /// </summary>
        internal void OnLayoutChanged()
        {
            LayoutChanged?.Invoke();
        }

        /// <summary>
        /// 更新水平位置及总宽
        /// </summary>
        internal void FixWidth(double p_maxWidth)
        {
            _maxWidth = p_maxWidth;
            TotalWidth = 0;
            double totalStar = 0;

            foreach (Col col in this.OfType<Col>())
            {
                if (col.Visibility == Visibility.Collapsed)
                    continue;
                
                var str = col.Width;
                double w = 0;

                if (string.IsNullOrEmpty(str)
                    || str.Equals("auto", StringComparison.OrdinalIgnoreCase))
                {
                    // auto 或 未指定时
                    w = _defaultWidth;
                }
                else if (double.TryParse(str, out var width))
                {
                    w = width;
                }
                else if (str.EndsWith('*'))
                {
                    // star
                    if (str.Length == 1)
                    {
                        totalStar += 1;
                    }
                    else if (double.TryParse(str.TrimEnd('*'), out var per))
                    {
                        totalStar += per;
                    }
                    else
                    {
                        w = _defaultWidth;
                    }
                }
                else
                {
                    w = _defaultWidth;
                }

                col.Left = TotalWidth;
                col.ActualWidth = w;
                TotalWidth += w;
            }

            // 列宽中有*，重新计算 ActualWidth Left
            if (totalStar > 0)
            {
                double unit = p_maxWidth > TotalWidth ? (p_maxWidth - TotalWidth) / totalStar : 0;
                // *按最小120宽
                unit = Math.Max(unit, _defaultWidth);
                TotalWidth = 0;

                foreach (Col col in this.OfType<Col>())
                {
                    if (col.Visibility == Visibility.Collapsed)
                        continue;
                    
                    if (col.ActualWidth > 0)
                    {
                        col.Left = TotalWidth;
                        TotalWidth += col.ActualWidth;
                    }
                    else
                    {
                        // star
                        var str = col.Width;
                        double per = 0;
                        if (str.Length == 1)
                        {
                            per = 1;
                        }
                        else if (double.TryParse(str.TrimEnd('*'), out per))
                        {
                        }

                        col.Left = TotalWidth;
                        col.ActualWidth = Math.Floor(per * unit);
                        TotalWidth += col.ActualWidth;
                    }
                }
            }
        }

        /// <summary>
        /// 列头最多占用的行数
        /// </summary>
        internal int GetLineCount()
        {
            int cnt;
            int maxCnt = 1;
            foreach (Col col in this.OfType<Col>())
            {
                if (col.Visibility == Visibility.Visible
                    && !string.IsNullOrEmpty(col.Title)
                    && (cnt = col.Title.Split('\u000A').Length) > maxCnt)
                {
                    maxCnt = cnt;
                }
            }
            return maxCnt;
        }

        /// <summary>
        /// 锁定列集合，集合变化Reloading
        /// </summary>
        internal void LockCols()
        {
            CollectionChanged -= OnColsCollectionChanged;
            CollectionChanged += OnColsCollectionChanged;

            foreach (var col in this.OfType<Col>())
            {
                col.Owner = this;
            }
        }

        internal void WriteJson(Utf8JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            foreach (var col in this.OfType<Col>())
            {
                p_writer.WriteStartObject();

                p_writer.WritePropertyName("ID");
                p_writer.WriteStringValue(col.ID);
                if (!string.IsNullOrEmpty(col.Title))
                {
                    p_writer.WritePropertyName("Title");
                    p_writer.WriteStringValue(col.Title);
                }
                if (!string.IsNullOrEmpty(col.Width))
                {
                    p_writer.WritePropertyName("Width");
                    p_writer.WriteStringValue(col.Width);
                }
                if (!col.AllowSorting)
                {
                    p_writer.WritePropertyName("AllowSorting");
                    p_writer.WriteBooleanValue(col.AllowSorting);
                }
                if (!string.IsNullOrEmpty(col.Call))
                {
                    p_writer.WritePropertyName("Call");
                    p_writer.WriteStringValue(col.Call);
                }
                if (!string.IsNullOrEmpty(col.Format))
                {
                    p_writer.WritePropertyName("Format");
                    p_writer.WriteStringValue(col.Format);
                }
                if (col.Visibility == Visibility.Collapsed)
                {
                    p_writer.WritePropertyName("Visibility");
                    p_writer.WriteBooleanValue(false);
                }
                
                if (col.Foreground != Res.默认前景)
                {
                    p_writer.WritePropertyName("Foreground");
#if WIN
                    p_writer.WriteStringValue(col.Foreground.Color.ToString());
#else
                    p_writer.WriteStringValue(ColorToStr(col.Foreground.Color));
#endif
                }
                if (col.Background != Res.TransparentBrush)
                {
                    p_writer.WritePropertyName("Background");
#if WIN
                    p_writer.WriteStringValue(col.Background.Color.ToString());
#else
                    p_writer.WriteStringValue(ColorToStr(col.Background.Color));
#endif
                }
                if (col.FontWeight != FontWeights.Normal)
                {
                    p_writer.WritePropertyName("FontWeight");
                    p_writer.WriteNumberValue(col.FontWeight.Weight);
                }
                if (col.FontStyle != FontStyle.Normal)
                {
                    p_writer.WritePropertyName("FontStyle");
                    p_writer.WriteNumberValue((int)col.FontStyle);
                }
                if (col.FontSize != 16d)
                {
                    p_writer.WritePropertyName("FontSize");
                    p_writer.WriteNumberValue(col.FontSize);
                }

                p_writer.WriteEndObject();
            }
            p_writer.WriteEndArray();
        }

        internal void ReadJson(ref Utf8JsonReader p_reader)
        {
            // 外层 [
            p_reader.Read();
            while (p_reader.Read() && p_reader.TokenType == JsonTokenType.StartObject)
            {
                Col col = new Col();
                while (p_reader.Read() && p_reader.TokenType == JsonTokenType.PropertyName)
                {
                    switch (p_reader.GetString())
                    {
                        case "ID":
                            col.ID = p_reader.ReadAsString();
                            break;

                        case "Title":
                            col.Title = p_reader.ReadAsString();
                            break;

                        case "Width":
                            col.Width = p_reader.ReadAsString();
                            break;

                        case "AllowSorting":
                            col.AllowSorting = p_reader.ReadAsBool();
                            break;

                        case "Call":
                            col.Call = p_reader.ReadAsString();
                            break;

                        case "Format":
                            col.Format = p_reader.ReadAsString();
                            break;

                        case "Visibility":
                            col.Visibility = p_reader.ReadAsBool() ? Visibility.Visible : Visibility.Collapsed;
                            break;
                            
                        case "Foreground":
                            var str = p_reader.ReadAsString();
                            try
                            {
                                Color color = Color.FromArgb(
                                    byte.Parse(str.Substring(1, 2), NumberStyles.HexNumber),
                                    byte.Parse(str.Substring(3, 2), NumberStyles.HexNumber),
                                    byte.Parse(str.Substring(5, 2), NumberStyles.HexNumber),
                                    byte.Parse(str.Substring(7, 2), NumberStyles.HexNumber));
                                col.Foreground = new SolidColorBrush(color);
                            }
                            catch { }
                            break;

                        case "Background":
                            var str1 = p_reader.ReadAsString();
                            try
                            {
                                Color color = Color.FromArgb(
                                    byte.Parse(str1.Substring(1, 2), NumberStyles.HexNumber),
                                    byte.Parse(str1.Substring(3, 2), NumberStyles.HexNumber),
                                    byte.Parse(str1.Substring(5, 2), NumberStyles.HexNumber),
                                    byte.Parse(str1.Substring(7, 2), NumberStyles.HexNumber));
                                col.Background = new SolidColorBrush(color);
                            }
                            catch { }
                            break;

                        case "FontWeight":
                            col.FontWeight = new FontWeight((ushort)p_reader.ReadAsInt());
                            break;

                        case "FontStyle":
                            col.FontStyle = (FontStyle)p_reader.ReadAsInt();
                            break;

                        case "FontSize":
                            col.FontSize = p_reader.ReadAsInt();
                            break;
                    }
                }
                Add(col);
            }
        }

        void OnColsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var col in e.NewItems.OfType<Col>())
                {
                    col.Owner = this;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var col in this.OfType<Col>())
                {
                    col.Owner = this;
                }
            }
            OnReloading();
        }

        static string ColorToStr(Color p_color)
        {
            StringBuilder sb = new StringBuilder("#");
            sb.Append(p_color.A == 0 ? "00" : System.Convert.ToString(p_color.A, 16));
            sb.Append(p_color.R == 0 ? "00" : System.Convert.ToString(p_color.R, 16));
            sb.Append(p_color.G == 0 ? "00" : System.Convert.ToString(p_color.G, 16));
            sb.Append(p_color.B == 0 ? "00" : System.Convert.ToString(p_color.B, 16));
            return sb.ToString();
        }

        // 6个中文字
        const double _defaultWidth = 120d;
        double _maxWidth;
        bool _isVisibleChanging;
    }
}
