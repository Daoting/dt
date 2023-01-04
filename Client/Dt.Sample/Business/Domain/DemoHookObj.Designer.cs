#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Sample
{
    [Tbl("cm_demo_hook")]
    public partial class DemoHookObj : Entity
    {
        #region 构造方法
        DemoHookObj() { }

        public DemoHookObj(
            long ID,
            string MaxLength = default,
            string NotNull = default,
            string Src = default,
            string Tgt = default,
            bool IsCheck = default,
            int NoBinding = default,
            int NoHook = default,
            bool NoDelete = default)
        {
            AddCell<long>("ID", ID);
            AddCell<string>("MaxLength", MaxLength);
            AddCell<string>("NotNull", NotNull);
            AddCell<string>("Src", Src);
            AddCell<string>("Tgt", Tgt);
            AddCell<bool>("IsCheck", IsCheck);
            AddCell<int>("NoBinding", NoBinding);
            AddCell<int>("NoHook", NoHook);
            AddCell<bool>("NoDelete", NoDelete);
            IsAdded = true;
        }
        #endregion

        public string MaxLength
        {
            get { return (string)this["MaxLength"]; }
            set { this["MaxLength"] = value; }
        }

        public string NotNull
        {
            get { return (string)this["NotNull"]; }
            set { this["NotNull"] = value; }
        }

        public string Src
        {
            get { return (string)this["Src"]; }
            set { this["Src"] = value; }
        }

        public string Tgt
        {
            get { return (string)this["Tgt"]; }
            set { this["Tgt"] = value; }
        }

        public bool IsCheck
        {
            get { return (bool)this["IsCheck"]; }
            set { this["IsCheck"] = value; }
        }

        public int NoBinding
        {
            get { return (int)this["NoBinding"]; }
            set { this["NoBinding"] = value; }
        }

        public int NoHook
        {
            get { return (int)this["NoHook"]; }
            set { this["NoHook"] = value; }
        }

        public bool NoDelete
        {
            get { return (bool)this["NoDelete"]; }
            set { this["NoDelete"] = value; }
        }
    }
}