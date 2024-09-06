#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-19 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    public partial class 物资计划X
    {
        public static async Task<物资计划X> New(
            long? 部门id = default,
            string No = default,
            计划类型? 计划类型 = default,
            计划编制方法? 编制方法 = default,
            string 摘要 = default,
            string 编制人 = default,
            DateTime? 编制日期 = default,
            string 审核人 = default,
            DateTime? 审核日期 = default)
        {
            return new 物资计划X(
                ID: await NewID(),
                部门id: 部门id,
                No: No,
                计划类型: 计划类型,
                编制方法: 编制方法,
                摘要: 摘要,
                编制人: 编制人,
                编制日期: 编制日期,
                审核人: 审核人,
                审核日期: 审核日期);
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