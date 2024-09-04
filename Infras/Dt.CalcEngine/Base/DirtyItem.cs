#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.CalcEngine
{
    internal class DirtyItem
    {
        private Dictionary<CalcIdentity, DirtyItem> _predencyItems;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal DirtyItem NextItem;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal double Position;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal DirtyItem PreviousItem;

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        internal bool DirtyFlag;

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        public CalcCalculationManager Manager { get; private set; }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        public int PredencyItemCount;

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        internal int CircleReferenceCount;

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        public HashSet<CalcNode> DependencyCellTemp;

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="node"></param>
        public DirtyItem(CalcCalculationManager manager, CalcNode node)
        {
            this.Manager = manager;
            this.Node = node;
        }

        public DirtyItem(CalcService service, ICalcSource source, CalcLocalIdentity id, CalcNode node)
        {
            this.Service = service;
            this.Source = source;
            this.Id = id;
            this.Node = node;
        }

        public bool HasPredencyItem
        {
            get
            {
                return ((this._predencyItems != null) && (this._predencyItems.Count > 0));
            }
        }

        public CalcLocalIdentity Id { get; private set; }

        public bool Moved { get; set; }

        public CalcNode Node { get; private set; }

        public Dictionary<CalcIdentity, DirtyItem> PredencyItems
        {
            get
            {
                if (this._predencyItems == null)
                {
                    this._predencyItems = new Dictionary<CalcIdentity, DirtyItem>();
                }
                return this._predencyItems;
            }
        }

        public CalcService Service { get; private set; }

        public ICalcSource Source { get; private set; }
    }
}

