#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 释放资源
    /// </summary>
    public partial class Fv : IDestroy
    {
        public void Destroy()
        {
            _isLoaded = false;
            Unloaded -= OnFvUnloaded;
            
            if (Data != null)
            {
                // 移除旧数据事件
                if (Data is Row row)
                {
                    row.Changed -= OnCellValueChanged;
                }
                else if (DataView != null)
                {
                    DataView.Changed -= OnPropertyValueChanged;
                    DataView = null;
                }
                
                Data = null;
            }

            if (Items.Count == 0)
                return;
            
            Items.ItemsChanged -= OnItemsChanged;
            while (Items.Count > 0)
            {
                if (Items[0] is FvCell cell)
                    cell.Destroy();
                Items.RemoveAt(0);
            }
        }
    }
}