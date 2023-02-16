#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

namespace $ext_safeprojectname$
{
    [Api]
    public class TestApi : DomainSvc
    {
        public string Hello(string p_str)
        {
            return p_str;
        }
    }
}