#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2024-03-15 ����
******************************************************************************/
#endregion

#region ��������
using Dt.Cells.Data;
using Dt.Cells.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        /// <summary>
        /// ����Pdf
        /// <para>1.ֻ�ṩStreamʱ����ȫ��Sheet</para>
        /// <para>2.����ÿ��Sheet.PrintInfo�ɿ���ֽ�Ŵ�С���߾ࡢ��ͷ��ͷ��ֻ���������</para>
        /// </summary>
        /// <param name="p_stream">��д����</param>
        /// <param name="p_sheetIndexes">Ҫ������sheet������null��ȫ������</param>
        /// <param name="p_settings">����pdf�Ļ�������</param>
        /// <returns></returns>
        public Task SavePdf(Stream p_stream, int[] p_sheetIndexes = null, PdfExportSettings p_settings = null)
        {
            var taskSrc = new TaskCompletionSource<bool>();
            DispatcherQueue.TryEnqueue(async () =>
            {
                // ����Chart��Sheet
                var ls = GetSheetsExistsChart(p_sheetIndexes);
                if (ls != null && ls.Count > 0)
                {
                    using (Defer())
                    {
                        await AllChartToImgs(ls);
                        Workbook.SavePdf(p_stream, p_sheetIndexes, p_settings);
                        ClearChartImgs(ls);
                    }
                }
                else
                {
                    Workbook.SavePdf(p_stream, p_sheetIndexes, p_settings);
                }
                taskSrc.TrySetResult(true);
            });
            return taskSrc.Task;
        }

        /// <summary>
        /// ֻ��������Sheet��ָ������
        /// </summary>
        /// <param name="p_stream"></param>
        /// <param name="p_range"></param>
        /// <param name="p_sheetIndex">Ҫ������Sheet������-1��ʾ��ǰ�Sheet</param>
        /// <param name="p_settings"></param>
        /// <returns></returns>
        public Task SavePdf(Stream p_stream, CellRange p_range, int p_sheetIndex = -1, PdfExportSettings p_settings = null)
        {
            int index = p_sheetIndex;
            if (index == -1)
                index = ActiveSheetIndex;
            
            if (p_range != null)
            {
                // ��������
                var pi = Sheets[index].PrintInfo;
                pi.RowStart = p_range.Row;
                pi.RowEnd = p_range.Row + p_range.RowCount - 1;
                pi.ColumnStart = p_range.Column;
                pi.ColumnEnd = p_range.Column + p_range.ColumnCount - 1;
            }
            return SavePdf(p_stream, new int[] { index }, p_settings);
        }
        
        /// <summary>
        /// ����Chart��Sheet
        /// </summary>
        /// <param name="p_sheetIndexs"></param>
        /// <returns></returns>
        List<int> GetSheetsExistsChart(int[] p_sheetIndexs)
        {
            var snap = ExcelKit.SnapBorder;
            if (snap == null)
                return null;

            List<int> ls = new List<int>();
            if (p_sheetIndexs == null || p_sheetIndexs.Length <= 0)
            {
                for (int i = 0; i < this.Sheets.Count; i++)
                {
                    if (Sheets[i].Charts.Count > 0)
                        ls.Add(i);
                }
            }
            else
            {
                for (int i = 0; i < p_sheetIndexs.Length; i++)
                {
                    int index = p_sheetIndexs[i];
                    if (index >= 0 && index < Sheets.Count)
                    {
                        if (Sheets[index].Charts.Count > 0)
                            ls.Add(index);
                    }
                }
            }
            return ls;
        }
        
        /// <summary>
        /// ������ChartתͼƬ����ͬλ��
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        async Task AllChartToImgs(List<int> p_index)
        {
            var snap = ExcelKit.SnapBorder;
            foreach (int i in p_index)
            {
                await ChartToImg(Sheets[i], snap);
            }
        }

        /// <summary>
        /// ������ɵ���ʱͼƬ
        /// </summary>
        /// <param name="p_index"></param>
        void ClearChartImgs(List<int> p_index)
        {
            foreach (int i in p_index)
            {
                var ws = Sheets[i];
                foreach (var c in ws.Charts)
                {
                    ws.RemovePicture("temp-" + c.Name);
                }
            }
        }
        
        async Task ChartToImg(Worksheet p_ws, Border p_snap)
        {
            foreach (var c in p_ws.Charts)
            {
                var view = new SpreadChartView(c, new Chart());
                view.Width = c.Size.Width;
                view.Height = c.Size.Height;
                p_snap.Child = view;

                RenderTargetBitmap bmp = new RenderTargetBitmap();
                await bmp.RenderAsync(view);
                var pixelBuffer = await bmp.GetPixelsAsync();
                var ms = new MemoryStream();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ms.AsRandomAccessStream());

                // �������������
                float dpi = (float)view.XamlRoot.RasterizationScale * 96;
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)bmp.PixelWidth,
                    (uint)bmp.PixelHeight,
                    dpi,
                    dpi,
                    pixelBuffer.ToArray());
                await encoder.FlushAsync();

                p_ws.AddPicture(
                    "temp-" + c.Name,
                    ms,
                    c.Location.X,
                    c.Location.Y,
                    c.Size.Width,
                    c.Size.Height);
            }
        }
    }
}