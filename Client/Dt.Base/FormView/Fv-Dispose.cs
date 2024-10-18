#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Input;
using Windows.System;
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