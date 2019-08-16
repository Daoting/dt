#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-06-15 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列表项按钮
    /// </summary>
    public partial class BtnItem : Button
    {
        #region 静态内容
        /// <summary>
        /// 按钮图标
        /// </summary>
        public readonly static DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(Icons),
            typeof(BtnItem),
            new PropertyMetadata(default(Icons)));

        /// <summary>
        /// 按钮标题
        /// </summary>
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(BtnItem),
            null);

        /// <summary>
        /// 按钮描述信息
        /// </summary>
        public readonly static DependencyProperty DescProperty = DependencyProperty.Register(
            "Desc",
            typeof(string),
            typeof(BtnItem),
            null);

        /// <summary>
        /// 附加的类型名称
        /// </summary>
        public readonly static DependencyProperty ClsProperty = DependencyProperty.Register(
            "Cls",
            typeof(string),
            typeof(BtnItem),
            null);

        /// <summary>
        /// 根据附加类型名称创建的类型实例
        /// </summary>
        public readonly static DependencyProperty ClsObjProperty = DependencyProperty.Register(
            "ClsObj",
            typeof(object),
            typeof(BtnItem),
            null);
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public BtnItem()
        {
            DefaultStyleKey = typeof(BtnItem);
        }

        /// <summary>
        /// 获取设置按钮图标
        /// </summary>
        public Icons Icon
        {
            get { return (Icons)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 获取设置按钮标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置按钮描述信息
        /// </summary>
        public string Desc
        {
            get { return (string)GetValue(DescProperty); }
            set { SetValue(DescProperty, value); }
        }

        /// <summary>
        /// 获取设置附加的类型名称，包括命名空间，不同程序集引用时需要提供程序集名称
        /// </summary>
        public string Cls
        {
            get { return (string)GetValue(ClsProperty); }
            set { SetValue(ClsProperty, value); }
        }

        /// <summary>
        /// 获取根据类型名称Cls创建的类型实例
        /// </summary>
        /// <param name="p_newObj">是否每次调用都实例化新对象</param>
        /// <returns></returns>
        public object GetClsObj(bool p_newObj = false)
        {
            string name = Cls;
            if (string.IsNullOrEmpty(name))
                return null;

            object obj = null;
            if (!p_newObj)
            {
                obj = GetValue(ClsObjProperty);
                if (obj != null)
                    return obj;
            }

            // 不可替换成GetClsType()！
            if (!name.Contains(","))
            {
                // 未提供程序集名称时，按调用类型所在的程序集
                var mth = new StackTrace().GetFrame(1).GetMethod();
                string str = mth.ReflectedType.AssemblyQualifiedName;
                name = name + str.Substring(str.IndexOf(','));
            }
            Type tp = Type.GetType(name, false);

            if (tp != null)
            {
                obj = Activator.CreateInstance(tp);
                if (!p_newObj)
                    SetValue(ClsObjProperty, obj);
            }
            return obj;
        }

        /// <summary>
        /// 根据设置的Cls获取类型
        /// </summary>
        public Type GetClsType()
        {
            string name = Cls;
            if (string.IsNullOrEmpty(name))
                return null;

            if (!name.Contains(","))
            {
                // 未提供程序集名称时，按调用类型所在的程序集
                var mth = new StackTrace().GetFrame(1).GetMethod();
                string str = mth.ReflectedType.AssemblyQualifiedName;
                name = name + str.Substring(str.IndexOf(','));
            }
            return Type.GetType(name, false);
        }
    }
}
