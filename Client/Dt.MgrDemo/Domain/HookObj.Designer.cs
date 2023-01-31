#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.Domain
{
    [Tbl("demo_hook")]
    public partial class HookObj : EntityX<HookObj>
    {
        #region 构造方法
        HookObj() { }

        public HookObj(CellList p_cells) : base(p_cells) { }

        public HookObj(
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
            AddCell("ID", ID);
            AddCell("MaxLength", MaxLength);
            AddCell("NotNull", NotNull);
            AddCell("Src", Src);
            AddCell("Tgt", Tgt);
            AddCell("IsCheck", IsCheck);
            AddCell("NoBinding", NoBinding);
            AddCell("NoHook", NoHook);
            AddCell("NoDelete", NoDelete);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 限制最大长度
        /// </summary>
        public string MaxLength
        {
            get { return (string)this["MaxLength"]; }
            set { this["MaxLength"] = value; }
        }

        /// <summary>
        /// 不为空
        /// </summary>
        public string NotNull
        {
            get { return (string)this["NotNull"]; }
            set { this["NotNull"] = value; }
        }

        /// <summary>
        /// 联动源
        /// </summary>
        public string Src
        {
            get { return (string)this["Src"]; }
            set { this["Src"] = value; }
        }

        /// <summary>
        /// 联动目标
        /// </summary>
        public string Tgt
        {
            get { return (string)this["Tgt"]; }
            set { this["Tgt"] = value; }
        }

        /// <summary>
        /// 联动源不为空可选中
        /// </summary>
        public bool IsCheck
        {
            get { return (bool)this["IsCheck"]; }
            set { this["IsCheck"] = value; }
        }

        /// <summary>
        /// 未和UI绑定
        /// </summary>
        public int NoBinding
        {
            get { return (int)this["NoBinding"]; }
            set { this["NoBinding"] = value; }
        }

        /// <summary>
        /// 无值变化Hook
        /// </summary>
        public int NoHook
        {
            get { return (int)this["NoHook"]; }
            set { this["NoHook"] = value; }
        }

        /// <summary>
        /// 禁止删除
        /// </summary>
        public bool NoDelete
        {
            get { return (bool)this["NoDelete"]; }
            set { this["NoDelete"] = value; }
        }
    }
}