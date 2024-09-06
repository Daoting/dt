#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [View1("v_物资详单")]
    public partial class 物资详单X
    {
        public static async Task<物资详单X> NewView1(
            long 单据id = default,
            long? 物资id = default,
            short? 序号 = default,
            string 批次 = default,
            float? 数量 = default,
            float? 单价 = default,
            float? 金额 = default,
            string 随货单号 = default,
            string 发票号 = default,
            DateTime? 发票日期 = default,
            float? 发票金额 = default,
            DateTime? 盘点时间 = default,
            float? 盘点金额 = default)
        {
            var x = new 物资详单X(
                ID: await NewID(),
                单据id: 单据id,
                物资id: 物资id,
                序号: 序号,
                批次: 批次,
                数量: 数量,
                单价: 单价,
                金额: 金额,
                随货单号: 随货单号,
                发票号: 发票号,
                发票日期: 发票日期,
                发票金额: 发票金额,
                盘点时间: 盘点时间,
                盘点金额: 盘点金额);

            x.Add("物资名称", "");
            x.Add("规格", "");
            x.Add("产地", "");
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