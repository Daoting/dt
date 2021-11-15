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
using System;
using System.IO;
using System.Reflection;
#endregion

namespace Dt.Sample
{
    public class ResKit
    {
        public static Stream GetResource(string p_fileName)
        {
            Assembly assembly = typeof(ResKit).Assembly;
            Stream stream = assembly.GetManifestResourceStream($"Dt.Sample.Resource.{p_fileName}");
            if (stream == null)
                throw new Exception("未发现资源文件：" + p_fileName );
            return stream;
        }
    }
}

