#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using Microsoft.UI.Xaml.Media;
using System.Xml;
using Windows.UI;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 图片
    /// </summary>
    public class RptImage : RptItem
    {
        public RptImage(RptPart p_owner)
            : base(p_owner)
        { }
        
        /// <summary>
        /// 图片内容
        /// </summary>
        public byte[] ImgData { get; set; }

        /// <summary>
        /// 构造报表项实例
        /// </summary>
        public override Task Build()
        {
            _part.Inst.Body.AddChild(new RptImageInst(this));
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// 输出图表
        /// </summary>
        /// <param name="p_ws"></param>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        public void Render(Worksheet p_ws, int p_row, int p_col)
        {
            if (ImgData != null && ImgData.Length > 0)
            {
                var pic = p_ws.AddPicture(
                    Guid.NewGuid().ToString().Substring(0, 6),
                    new MemoryStream(ImgData),
                    p_row,
                    0,
                    p_col,
                    0,
                    p_row + RowSpan,
                    0,
                    p_col + ColSpan,
                    0);
                // 锁定禁止拖动缩放
                pic.Locked = true;
            }
            else
            {
                var cell = p_ws[p_row, p_col];
                cell.ColumnSpan = ColSpan;
                cell.RowSpan = RowSpan;
                cell.Background = new SolidColorBrush(Color.FromArgb(0XFF, 0XE0, 0XE0, 0XE0));
                cell.VerticalAlignment = CellVerticalAlignment.Center;
                cell.HorizontalAlignment = CellHorizontalAlignment.Center;
                cell.FontFamily = Res.IconFont;
                cell.FontSize = 40;
                cell.Text = "\uE08A";
            }
        }
        
        /// <summary>
        /// 选择要插入的图片文件
        /// </summary>
        /// <returns></returns>
        public async void SelectImage()
        {
            var filePicker = Kit.GetFileOpenPicker();
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".gif");
            var file = await filePicker.PickSingleFileAsync();
            if (file == null)
                return;

            using (var stream = await file.OpenStreamForReadAsync())
            {
                byte[] buffer = new byte[stream.Length];
                await stream.ReadExactlyAsync(buffer);
                ImgData = buffer;
            }
            Root?.OnImageChanged(this);
        }

        /// <summary>
        /// 清除图片内容
        /// </summary>
        public void ClearImage()
        {
            ImgData = null;
            Root?.OnImageChanged(this);
        }
        
        #region xml
        /// <summary>
        /// 加载xml时解析表达式
        /// </summary>
        /// <param name="p_reader"></param>
        public override void ReadXml(XmlReader p_reader)
        {
            _data.ReadXml(p_reader);
            var con = p_reader.ReadElementContentAsString();
            if (!string.IsNullOrEmpty(con))
            {
                ImgData = Convert.FromBase64String(con);
            }
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Image");
            WritePosition(p_writer);
            if (ImgData != null && ImgData.Length > 0)
            {
                p_writer.WriteString(Convert.ToBase64String(ImgData));
            }
            p_writer.WriteEndElement();
        }
        #endregion
    }
}
