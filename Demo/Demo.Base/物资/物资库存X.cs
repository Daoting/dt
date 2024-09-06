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
    public partial class 物资库存X
    {
        public static async Task<物资库存X> New(
            long? 部门id = default,
            long? 物资id = default,
            string 批次 = default,
            float? 可用数量 = default,
            float? 可用金额 = default,
            float? 实际数量 = default,
            float? 实际金额 = default)
        {
            return new 物资库存X(
                ID: await NewID(),
                部门id: 部门id,
                物资id: 物资id,
                批次: 批次,
                可用数量: 可用数量,
                可用金额: 可用金额,
                实际数量: 实际数量,
                实际金额: 实际金额);
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