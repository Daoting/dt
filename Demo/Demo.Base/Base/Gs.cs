#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-23 15:19:25 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Demo.Base
{
    /// <summary>
    /// 全局静态类(global static)
    /// </summary>
    public static partial class Gs
    {
        public static async void OnLogin(AsyncArgs e)
        {
            using (e.Wait())
            {
                人员 = await 人员X.First("where user_id=" + Kit.UserID);
                if (人员 != null)
                {
                    Kit.UserName = 人员.姓名;
                    所属部门 = await 部门人员X.Query($"select b.部门id,a.名称 as 部门名称,b.缺省 from 部门 a, 部门人员 b where a.ID = b.部门id and b.人员id={人员.ID}");
                }
            }
        }

        public static 人员X 人员 { get; private set; }

        public static Table<部门人员X> 所属部门 { get; private set; }

        public static 部门人员X 缺省部门 =>
            (from x in 所属部门.Items 
             where x.缺省 == true
             select x).FirstOrDefault();
    }
}