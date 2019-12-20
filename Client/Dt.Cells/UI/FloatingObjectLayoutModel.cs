#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.UI
{
    internal class FloatingObjectLayoutModel : IEnumerable<FloatingObjectLayout>, IEnumerable
    {
        private Dictionary<string, FloatingObjectLayout> _items;

        public FloatingObjectLayoutModel()
        {
        }

        internal FloatingObjectLayoutModel(FloatingObjectLayoutModel model)
        {
            if ((model != null) && (model.Count > 0))
            {
                this._items = new Dictionary<string, FloatingObjectLayout>();
                foreach (FloatingObjectLayout layout in model)
                {
                    this._items.Add(layout.Name, new FloatingObjectLayout(layout.Name, layout.X, layout.Y, layout.Width, layout.Height));
                }
            }
        }

        public void Add(FloatingObjectLayout chartLayout)
        {
            this.Items.Add(chartLayout.Name, chartLayout);
        }

        public FloatingObjectLayout Find(string name)
        {
            if (this._items != null)
            {
                FloatingObjectLayout layout = null;
                if (this._items.TryGetValue(name, out layout))
                {
                    return layout;
                }
            }
            return null;
        }

        public IEnumerator<FloatingObjectLayout> GetEnumerator()
        {
            return (IEnumerator<FloatingObjectLayout>) this.Items.Values.GetEnumerator();
        }

        public void Remove(string name)
        {
            this.Items.Remove(name);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) this.GetEnumerator();
        }

        public int Count
        {
            get
            {
                if (this._items == null)
                {
                    return 0;
                }
                return this.Items.Count;
            }
        }

        private Dictionary<string, FloatingObjectLayout> Items
        {
            get
            {
                if (this._items == null)
                {
                    this._items = new Dictionary<string, FloatingObjectLayout>();
                }
                return this._items;
            }
        }
    }
}

