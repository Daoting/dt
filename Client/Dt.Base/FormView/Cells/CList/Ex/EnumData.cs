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

namespace Dt.Base
{
    /// <summary>
    /// 枚举类型作为下拉框数据源
    /// </summary>
    [CListEx]
    public class EnumData : CListEx
    {
        /// <summary>
        /// 格式：枚举名(包含命名空间),程序集；如：
        /// <para>Dt.Base.CtType,Dt.Base</para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override Task<INotifyList> GetData()
        {
            // 枚举数据
            Type type = Type.GetType(_params, true, true);
            var data = (INotifyList)_dlg.CreateEnumData(type);
            return Task.FromResult(data);
        }
    }
}