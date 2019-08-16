#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-12-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Table扩展方法
    /// </summary>
    public static class DataTableExt
    {
        /// <summary>
        /// 根据Xml文件内容创建Table对象
        /// </summary>
        /// <param name="p_file"></param>
        /// <returns></returns>
        public static async Task<Table> CreateFromFile(string p_file)
        {
            if (string.IsNullOrEmpty(p_file))
                return null;

            StorageFile sf = await StorageFile.GetFileFromApplicationUriAsync(new Uri(p_file));
            return Table.CreateFromXmlFile(sf.Path);
        }
    }
}
