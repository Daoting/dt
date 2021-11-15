#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.MenuView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 菜单
    /// </summary>
    [ContentProperty(Name = nameof(Items))]
    public partial class Menu : DtControl
    {
        #region 成员变量
        MenuPanel _panel;
        Mi _selectedMi;
        #endregion

        #region 构造方法
        public Menu()
        {
            DefaultStyleKey = typeof(Menu);
            Items = new MiList();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 菜单项点击事件
        /// </summary>
        public event EventHandler<Mi> ItemClick;

        /// <summary>
        /// 打开上下文菜单前事件，可取消显示、禁止显示、初始化菜单项、替换整个菜单等处理
        /// </summary>
        public event EventHandler<AsyncCancelEventArgs> Opening;

        /// <summary>
        /// 上下文菜单打开后事件
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// 上下文菜单关闭后事件
        /// </summary>
        public event EventHandler Closed;
        #endregion

        #region 属性
        /// <summary>
        /// 获取具有指定名称的菜单项
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public Mi this[string p_id]
        {
            get
            {
                if (string.IsNullOrEmpty(p_id))
                    return null;

                // 未使用键值方式，因ID可能改变，菜单项数一般不多！
                foreach (Mi mi in AllItems)
                {
                    if (string.Equals(p_id, mi.ID, StringComparison.OrdinalIgnoreCase))
                        return mi;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取指定索引处的一级菜单项
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        public Mi this[int p_index]
        {
            get
            {
                if (p_index >= 0 && p_index < Items.Count)
                    return Items[p_index];
                return null;
            }
        }

        /// <summary>
        /// 获取子菜单集合
        /// </summary>
        public MiList Items { get; }

        /// <summary>
        /// 递归获取所有子级菜单项
        /// </summary>
        public IEnumerable<Mi> AllItems
        {
            get
            {
                foreach (Mi mi in Items)
                {
                    yield return mi;
                    if (mi.Items.Count > 0)
                    {
                        foreach (Mi child in mi.AllItems)
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 当前选择项
        /// </summary>
        internal Mi SelectedMi
        {
            get { return _selectedMi; }
            set
            {
                if (_selectedMi != value)
                {
                    if (_selectedMi != null)
                        _selectedMi.IsSelected = false;
                    _selectedMi = value;
                    if (_selectedMi != null)
                        _selectedMi.IsSelected = true;
                }
            }
        }
        #endregion

        #region 显示/隐藏菜单项
        /// <summary>
        /// 隐藏名称列表中的菜单项，只处理一级！
        /// </summary>
        /// <param name="p_names"></param>
        public void Hide(params string[] p_names)
        {
            foreach (string name in p_names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                var item = (from mi in Items
                            where string.Equals(name, mi.ID, StringComparison.OrdinalIgnoreCase)
                            select mi).FirstOrDefault();
                if (item != null && item.Visibility == Visibility.Visible)
                    item.Visibility = Visibility.Collapsed;
            }
            _panel?.UpdateArrange();
        }

        /// <summary>
        /// 除显示名称列表中的菜单项外，其它都隐藏，列表空时隐藏所有，只处理一级！
        /// </summary>
        /// <param name="p_names">无值时隐藏所有</param>
        public void HideExcept(params string[] p_names)
        {
            foreach (var mi in Items)
            {
                if (p_names.Contains(mi.ID))
                    mi.Visibility = Visibility.Visible;
                else if (mi.Visibility == Visibility.Visible)
                    mi.Visibility = Visibility.Collapsed;
            }
            _panel?.UpdateArrange();
        }

        /// <summary>
        /// 显示名称列表中的菜单项，只处理一级！
        /// </summary>
        /// <param name="p_names"></param>
        public void Show(params string[] p_names)
        {
            foreach (string name in p_names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                var item = (from mi in Items
                            where string.Equals(name, mi.ID, StringComparison.OrdinalIgnoreCase)
                            select mi).FirstOrDefault();
                if (item != null && item.Visibility == Visibility.Collapsed)
                    item.Visibility = Visibility.Visible;
            }
            _panel?.UpdateArrange();
        }

        /// <summary>
        /// 除隐藏名称列表中的菜单项外，其它都显示，列表空时显示所有，只处理一级！
        /// </summary>
        /// <param name="p_names">无值时显示所有</param>
        public void ShowExcept(params string[] p_names)
        {
            foreach (var mi in Items)
            {
                if (p_names.Contains(mi.ID))
                    mi.Visibility = Visibility.Collapsed;
                else if (mi.Visibility == Visibility.Collapsed)
                    mi.Visibility = Visibility.Visible;
            }
            _panel?.UpdateArrange();
        }
        #endregion

        #region 菜单项可用/不可用
        /// <summary>
        /// 设置名称列表中的菜单项为可用，只处理一级！
        /// </summary>
        /// <param name="p_names"></param>
        public void Enable(params string[] p_names)
        {
            foreach (string name in p_names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                var item = (from mi in Items
                            where string.Equals(name, mi.ID, StringComparison.OrdinalIgnoreCase)
                            select mi).FirstOrDefault();
                if (item != null && !item.IsEnabled)
                    item.IsEnabled = true;
            }
        }

        /// <summary>
        /// 除名称列表中的菜单项外，其它都可用，列表空时所有可用，只处理一级！
        /// </summary>
        public void EnableExcept(params string[] p_names)
        {
            foreach (var mi in Items)
            {
                if (p_names.Contains(mi.ID))
                    mi.IsEnabled = false;
                else
                    mi.IsEnabled = true;
            }
        }

        /// <summary>
        /// 设置名称列表中的菜单项为不可用，只处理一级！
        /// </summary>
        /// <param name="p_names"></param>
        public void Disable(params string[] p_names)
        {
            foreach (string name in p_names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                var item = (from mi in Items
                            where string.Equals(name, mi.ID, StringComparison.OrdinalIgnoreCase)
                            select mi).FirstOrDefault();
                if (item != null && item.IsEnabled)
                    item.IsEnabled = false;
            }
        }

        /// <summary>
        /// 除名称列表中的菜单项外，其它都不可用，列表空时所有不可用，只处理一级！
        /// </summary>
        public void DisableExcept(params string[] p_names)
        {
            foreach (var mi in Items)
            {
                if (p_names.Contains(mi.ID))
                    mi.IsEnabled = true;
                else
                    mi.IsEnabled = false;
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 关闭菜单
        /// </summary>
        public void Close()
        {
            SelectedMi = null;
            // 上下文菜单
            if (_dlg != null && _dlg.IsOpened)
                _dlg.Close();
        }
        #endregion

        #region Mi.IsSelected变化
        bool _isBubbling;

        /// <summary>
        /// 选择某个菜单项时的处理
        /// </summary>
        /// <param name="p_mi"></param>
        internal void OnItemIsSelected(Mi p_mi)
        {
            if (_isBubbling)
                return;

            // 冒泡处理
            _isBubbling = true;
            Mi child = p_mi;
            Mi par = p_mi.ParentMi;
            while (par != null)
            {
                par.OnChildIsSelected(child);
                child = par;
                par = par.ParentMi;
            }

            // 新选择的为一级菜单项
            if (SelectedMi != p_mi && p_mi.ParentMi == null)
            {
                // 关闭上一菜单项的子项窗口
                if (SelectedMi != null)
                    SelectedMi.CloseSubMenu();
                SelectedMi = p_mi;
            }
            _isBubbling = false;
        }
        #endregion

        #region 内部方法
        protected override void OnLoadTemplate()
        {
            _panel = (MenuPanel)GetTemplateChild("Panel");
            _panel.SetOwner(this);
        }

        internal void OnItemClick(Mi p_item)
        {
            ItemClick?.Invoke(this, p_item);
        }
        #endregion
    }

    /// <summary>
    /// 子项列表，直接用泛型在xaml设计时异常
    /// </summary>
    public class MiList : ItemList<Mi>
    { }
}
