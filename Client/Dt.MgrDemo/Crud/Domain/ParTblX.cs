#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.Crud
{
    public partial class ParTblX
    {
        public static async Task<ParTblX> New(
            string Name = default)
        {
            var x = new ParTblX(
                ID: await NewID(),
                Name: Name);

            x.Tbl1 = new Table<ChildTbl1X>();
            x.Tbl2 = new Table<ChildTbl2X>();
            return x;
        }

        protected override void InitHook()
        {
        }

        [ChildX("ParentID")]
        public Table<ChildTbl1X> Tbl1 { get; set; }


        [ChildX("GroupID")]
        public Table<ChildTbl2X> Tbl2 { get; set; }
    }
}