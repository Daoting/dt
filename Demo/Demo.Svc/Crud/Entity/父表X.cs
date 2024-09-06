#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Crud
{
    public partial class 父表X
    {
        public static async Task<父表X> New(
            string 父名 = default)
        {
            var x = new 父表X(
                ID: await NewID(),
                父名: 父名);
            
            x.Tbl1 = new Table<大儿X>();
            x.Tbl2 = new Table<小儿X>();
            return x;
        }

        protected override void InitHook()
        {
            
        }

        [ChildX("parent_id")]
        public Table<大儿X> Tbl1 { get; set; }


        [ChildX("group_id")]
        public Table<小儿X> Tbl2 { get; set; }
    }
}