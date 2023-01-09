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
    public class ResKit
    {
        /// <summary>
        /// 返回资源文件流，需要在外部关闭流
        /// </summary>
        /// <param name="p_fileName">文件名，在Res下含子目录时需要添加子目录前缀如：Excel.1040.xlsx</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Stream GetStream(string p_fileName)
        {
            Assembly assembly = typeof(ResKit).Assembly;
            Stream stream = assembly.GetManifestResourceStream($"Dt.UIDemo.Res.{p_fileName}");
            if (stream == null)
                throw new Exception("未发现资源文件：" + p_fileName );
            return stream;
        }
    }
}

