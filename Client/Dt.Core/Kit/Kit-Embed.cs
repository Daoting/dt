#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-06-19 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Diagnostics;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 嵌入资源文件工具
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 返回当前调用所属程序集中的嵌入文件的文件流，需要在外部关闭流，文件在项目的 Bag 目录
        /// </summary>
        /// <param name="p_fileName">文件名，不包含Bag，当文件在Bag子目录时，需要添加子目录前缀。如：Excel.1040.xlsx，文件1040.xlsx在Bag\Excel目录</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Stream GetBagFileStream(string p_fileName)
        {
            // 获取调用堆栈信息
            var st = new StackTrace();
            if (st.FrameCount < 2)
                throw new Exception("获取嵌入资源文件流时，异常位置未知");

            var assembly = st.GetFrame(1).GetMethod().DeclaringType.Assembly;
            return GetEmbedStream(assembly, "Bag." + p_fileName);
        }

        /// <summary>
        /// 返回程序集中的嵌入文件的文件流，需要在外部关闭流
        /// </summary>
        /// <param name="p_asm">程序集</param>
        /// <param name="p_filePath">文件名，包含目录，如：Bag.Excel.1040.xlsx，Bag\Excel目录的1040.xlsx文件</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static Stream GetEmbedStream(Assembly p_asm, string p_filePath)
        {
            if (p_asm == null)
                throw new ArgumentNullException(nameof(p_asm), "嵌入文件的程序集不能为空");
            if (string.IsNullOrEmpty(p_filePath))
                throw new ArgumentNullException(nameof(p_filePath), "嵌入文件名不能为空");
            
            Stream stream = p_asm.GetManifestResourceStream(p_asm.GetName().Name + "." + p_filePath);
            if (stream == null)
                throw new Exception("未找到资源文件：" + p_filePath);
            return stream;
        }

        /// <summary>
        /// 返回当前调用所属程序集的嵌入文件的内容，文件在项目的 Bag 目录
        /// </summary>
        /// <param name="p_fileName">文件名，不包含Bag，当文件在Bag子目录时，需要添加子目录前缀。如：Json.Abc.txt，文件Abc.txt在Bag\Json 目录</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetBagFileText(string p_fileName)
        {
            // 获取调用堆栈信息
            var st = new StackTrace();
            if (st.FrameCount < 2)
                throw new Exception("获取嵌入资源文件时，异常位置未知");

            var assembly = st.GetFrame(1).GetMethod().DeclaringType.Assembly;
            return GetEmbedText(assembly, "Bag." + p_fileName);
        }

        /// <summary>
        /// 返回当前调用所属程序集的嵌入文件的内容
        /// </summary>
        /// <param name="p_asm">程序集</param>
        /// <param name="p_filePath">文件名，包含目录，如：Bag.Json.Abc.txt，文件Abc.txt在Bag\Json 目录</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static string GetEmbedText(Assembly p_asm, string p_filePath)
        {
            if (p_asm == null)
                throw new ArgumentNullException(nameof(p_asm), "嵌入文件的程序集不能为空");
            if (string.IsNullOrEmpty(p_filePath))
                throw new ArgumentNullException(nameof(p_filePath), "嵌入文件名不能为空");
            
            try
            {
                using (var stream = p_asm.GetManifestResourceStream(p_asm.GetName().Name + "." + p_filePath))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                throw new Exception("未找到资源文件：" + p_filePath);
            }
        }
    }
}

