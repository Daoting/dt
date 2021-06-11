#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace $rootnamespace$
{
	class $safeitemrootname$
	{
		public static TextBlock xb(ViewItem p_item)
		{
			return new TextBlock { Text = p_item.Row.Str("xb") == "男" ? "\uE060" : "\uE0D9" };
		}

		///// <summary>
		///// 设置行样式
		///// </summary>
		///// <param name="p_item"></param>
		//public static void SetStyle(ViewItem p_item)
		//{
		//}
	}
}