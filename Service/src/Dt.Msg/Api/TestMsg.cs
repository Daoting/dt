#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021/6/25 9:07:41 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#endregion

namespace Dt.Msg.Api
{
    /// <summary>
    /// 
    /// </summary>
    [Api(GroupName = "功能测试")]
    public class TestMsg : BaseApi
    {
        public int CloseAllOnline()
        {
            // 单副本
            int cnt = Online.All.Count;
            foreach (var item in Online.All)
            {
                item.Value.Close();
            }
            return cnt;
        }
    }
}