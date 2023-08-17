#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-12-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 模型库的OmOption作为下拉框数据源
    /// </summary>
    [CListEx]
    public class Option : CListEx
    {
        /// <summary>
        /// 格式：枚举名(包含命名空间),程序集；如：
        /// <para>Dt.Base.CtType,Dt.Base</para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<INotifyList> GetData()
        {
            return await AtOption.Query($"select name from OmOption where Category=\"{_params}\"");
        }
    }
}