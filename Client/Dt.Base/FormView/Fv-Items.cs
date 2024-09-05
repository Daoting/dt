#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Windows.Foundation.Collections;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 表单控件
    /// </summary>
    public partial class Fv
    {
        /// <summary>
        /// 获取单元格集合
        /// </summary>
        public FvItems Items { get; } = new FvItems();

        /// <summary>
        /// 获取具有指定id的单元格
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public FvCell this[string p_id]
        {
            get { return Items[p_id]; }
        }

        /// <summary>
        /// 获取指定索引的单元格
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        public FvCell this[int p_index]
        {
            get { return Items[p_index] as FvCell; }
        }

        /// <summary>
        /// 获取所有含ID的格
        /// </summary>
        public IEnumerable<FvCell> IDCells
        {
            get
            {
                return from obj in Items
                       let cell = obj as FvCell
                       where cell != null && !string.IsNullOrEmpty(cell.ID)
                       select cell;
            }
        }

        #region Items管理
        void OnItemsChanged(object sender, ItemListChangedArgs e)
        {
            if (e.CollectionChange == CollectionChange.ItemRemoved || e.CollectionChange == CollectionChange.ItemChanged)
                _panel.Children.RemoveAt(e.Index);

            if (e.CollectionChange == CollectionChange.ItemInserted || e.CollectionChange == CollectionChange.ItemChanged)
            {
                AddItem(Items[e.Index], e.Index);
            }
            else if (e.CollectionChange == CollectionChange.Reset)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] is not UIElement item)
                        continue;

                    if (_panel.Children.Count > i)
                    {
                        var elem = _panel.Children[i];

                        // 内容没变
                        if (item == elem
                            || (elem is CFree f && f.Content == item))
                            continue;

                        // 变了移除旧元素
                        _panel.Children.RemoveAt(i);
                    }
                    AddItem(item, i);
                }

                // 移除多余的元素
                while (_panel.Children.Count > Items.Count)
                {
                    _panel.Children.RemoveAt(_panel.Children.Count - 1);
                }
            }
        }

        protected virtual void LoadAllItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                AddItem(Items[i], i);
            }
        }

        protected void AddItem(object p_item, int p_index)
        {
            var elem = p_item as UIElement;
            if (p_item is FvCell cell)
            {
                cell.Owner = this;
            }
            else if (!(p_item is CBar))
            {
                // 自定义内容
                CFree c = new CFree();
                c.Owner = this;
                c.ShowTitle = false;
                c.Content = elem;
                elem = c;
            }
            _panel.Children.Insert(p_index, elem);
        }
        #endregion
    }
}