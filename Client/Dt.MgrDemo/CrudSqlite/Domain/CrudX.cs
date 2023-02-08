#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.CrudSqlite
{
    public partial class CrudX
    {
        public static async Task<CrudX> New(string Name)
        {
            return new CrudX(
                ID: await NewID(),
                Name: Name,
                Dispidx: 0,
                Mtime: Kit.Now);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                if (IsAdded && EnableInsertEvent)
                {
                    AddEvent(new SqliteInsertCrudEvent { ID = ID });
                }
                else if (!IsAdded
                    && EnableNameChangedEvent
                    && _cells["Name"].IsChanged)
                {
                    AddEvent(new SqliteNameChangedEvent
                    {
                        // 保存后信息丢失
                        OriginalVal = _cells["Name"].GetOriginalVal<string>(),
                        NewVal = Name,
                    });
                }
                return Task.CompletedTask;
            });

            OnDeleting(() =>
            {
                if (EnableDelEvent)
                {
                    AddEvent(new SqliteDelCrudEvent { Tgt = this });
                }
                return Task.CompletedTask;
            });

            OnChanging<string>(nameof(Name), v =>
            {
                //Kit.Msg("Name新值：" + v);
            });
        }
    }
}