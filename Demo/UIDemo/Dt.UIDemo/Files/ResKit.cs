#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
#endregion

namespace Dt.UIDemo
{
    /// <summary>
    /// 嵌入资源文件工具类
    /// </summary>
    public static class ResKit
    {
        const string _path = "Dt.UIDemo.Files.Embed.";

        /// <summary>
        /// 返回资源文件流，需要在外部关闭流，文件在 Files\Embed 目录
        /// </summary>
        /// <param name="p_fileName">文件名，在 Files\Embed 下含子目录时需要添加子目录前缀如：Excel.1040.xlsx</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Stream GetStream(string p_fileName)
        {
            Assembly assembly = typeof(ResKit).Assembly;
            Stream stream = assembly.GetManifestResourceStream(_path + p_fileName);
            if (stream == null)
                throw new Exception("未找到资源文件：" + p_fileName);
            return stream;
        }

        /// <summary>
        /// 返回资源文件内容，文件在 Files\Embed 目录
        /// </summary>
        /// <param name="p_fileName">文件名，在 Files\Embed 下含子目录时需要添加子目录前缀如：Excel.1040.xlsx</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetText(string p_fileName)
        {
            try
            {
                Assembly assembly = typeof(ResKit).Assembly;
                using (var stream = assembly.GetManifestResourceStream(_path + p_fileName))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                throw new Exception("未找到资源文件：" + p_fileName);
            }
        }
    }
}

