#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [View1("v_物资主单")]
    public partial class 物资主单X
    {
        public static async Task<物资主单X> NewView1(long ID, 物资入出类别X 入出类别)
        {
            var seq = await At.NewSeq("物资主单_单号");
            string no = 入出类别.单号前缀 + seq.ToString("D6");
            var x = new 物资主单X(
                ID: ID,
                部门id: Gs.缺省部门.部门id,
                入出类别id: 入出类别.ID,
                状态: 单据状态.填写,
                单号: no,
                入出系数: 入出类别.系数);

            x.Add("部门名称", Gs.缺省部门.部门名称);
            x.Add("入出类别", 入出类别.名称);
            x.Add("供应商", "");
            return x;
        }

        protected override void InitHook()
        {
            //OnSaving(() =>
            //{

            //    return Task.CompletedTask;
            //});

            //OnSaved(() =>
            //{

            //    return Task.CompletedTask;
            //});

            //OnDeleting(() =>
            //{

            //    return Task.CompletedTask;
            //});

            //OnDeleted(() =>
            //{

            //    return Task.CompletedTask;
            //});

            //OnChanging(cName, e =>
            //{

            //});
        }

        #region Sql

        #endregion
    }
}