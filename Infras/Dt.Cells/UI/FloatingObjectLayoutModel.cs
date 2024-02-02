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
        Dictionary<string, FloatingObjectLayout> _items;

        public FloatingObjectLayoutModel()
        {
        }

        internal FloatingObjectLayoutModel(FloatingObjectLayoutModel model)
        {
            if ((model != null) && (model.Count > 0))
            {
                _items = new Dictionary<string, FloatingObjectLayout>();
                foreach (FloatingObjectLayout layout in model)
                {
                    _items.Add(layout.Name, new FloatingObjectLayout(layout.Name, layout.X, layout.Y, layout.Width, layout.Height));
                }
            }
        }

        public void Add(FloatingObjectLayout chartLayout)
        {
            Items.Add(chartLayout.Name, chartLayout);
        }

        public FloatingObjectLayout Find(string name)
        {
            if (_items != null)
            {
                FloatingObjectLayout layout = null;
                if (_items.TryGetValue(name, out layout))
                {
                    return layout;
                }
            }
            return null;
        }

        public IEnumerator<FloatingObjectLayout> GetEnumerator()
        {
            return (IEnumerator<FloatingObjectLayout>) Items.Values.GetEnumerator();
        }

        public void Remove(string name)
        {
            Items.Remove(name);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) GetEnumerator();
        }

        public int Count
        {
            get
            {
                if (_items == null)
                {
                    return 0;
                }
                return Items.Count;
            }
        }

        Dictionary<string, FloatingObjectLayout> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new Dictionary<string, FloatingObjectLayout>();
                }
                return _items;
            }
        }
    }
}

