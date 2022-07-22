#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Exceptions;
using Dt.Pdf.Utility.zlib;
using System.IO;
using System.Reflection;
#endregion

namespace Dt.Pdf.Utility
{
    public class FlateUtility
    {
        public static byte[] FlateDecode(byte[] inp, bool strict)
        {
            MemoryStream stream = new MemoryStream(inp);
            ZInflaterInputStream @this = new ZInflaterInputStream(stream);
            MemoryStream stream3 = new MemoryStream();
            byte[] buffer = new byte[strict ? 0xffc : 1];
            try
            {
                int num;
                while ((num = @this.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream3.Write(buffer, 0, num);
                }
                @this.Dispose();
                stream3.Dispose();
                return stream3.ToArray();
            }
            catch
            {
                if (strict)
                {
                    return null;
                }
                return stream3.ToArray();
            }
        }

        public static void FlateEncode(Stream rawData, Stream result)
        {
            ZDeflaterOutputStream stream = new ZDeflaterOutputStream(result);
            byte[] buffer = new byte[0x400];
            int count = 0;
            while ((count = rawData.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, count);
            }
            stream.Finish();
        }

        internal static Stream GetResource(string name)
        {
            Assembly assembly = typeof(FlateUtility).GetTypeInfo().Assembly;
            Stream manifestResourceStream = assembly.GetManifestResourceStream(string.Format("Dt.Pdf.Fonts.{0}", name));
            if (manifestResourceStream == null)
            {
                throw new PdfArgumentNullException("未发现Pdf所需的字体文件：" + name );
            }
            return manifestResourceStream;
        }
    }
}

