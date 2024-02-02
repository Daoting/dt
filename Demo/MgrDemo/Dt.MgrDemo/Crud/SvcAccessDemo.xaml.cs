#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.MgrDemo.Crud;
using Microsoft.UI.Xaml;
using System.Reflection;
using System.Text;
using Windows.UI.Core;
#endregion

namespace Dt.MgrDemo
{
    public partial class SvcAccessDemo : Win
    {
        public SvcAccessDemo()
        {
            InitializeComponent();
        }

        #region 增删改
        async void OnInsert(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.Insert(), "Insert");
        }

        async void OnUpdate(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.Update(), "Update");
        }

        async void OnDelete(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.Delete(), "Delete");
        }

        async void OnBatchInsert(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.BatchInsert(), "BatchInsert");
        }

        async void OnBatch(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.Batch(), "Batch");
        }

        async void OnSaveTable(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.SaveTable(), "SaveTable");
        }

        async void OnBatchDel(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.BatchDel(), "BatchDel");
        }

        async void OnDirectDel(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.DirectDel(), "DirectDel");
        }

        async void OnDelByID(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.DelByID(), "DelByID");
        }

        void Msg(bool p_suc, string p_method)
        {
            if (p_suc)
                Kit.Msg(p_method + "调用成功！");
            else
                Kit.Warn(p_method + "调用失败！");
        }
        #endregion

        #region 领域事件
        async void OnInsertEvent(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.InsertEvent(), "InsertEvent");
        }

        async void OnUpdateEvent(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.UpdateEvent(), "UpdateEvent");
        }

        async void OnDelEvent(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.DelEvent(), "DelEvent");
        }
        #endregion

        #region 虚拟实体
        async void OnInsertVir(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.InsertVir(), "InsertVir");
        }

        async void OnUpdateVir(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.UpdateVir(), "UpdateVir");
        }

        async void OnDelVir(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.DelVir(), "DelVir");
        }

        async void OnSaveVir(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.SaveVir(), "SaveVir");
        }

        async void OnDirectDelVir(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.DirectDelVir(), "DirectDelVir");
        }

        async void OnDelByIDVir(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.DelByIDVir(), "DelByIDVir");
        }
        #endregion

        #region 父子实体
        async void OnInsertWithChild(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.InsertWithChild(), "InsertWithChild");
        }

        async void OnUpdateWithChild(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.UpdateWithChild(), "UpdateWithChild");
        }

        async void OnQueryWithChild(object sender, RoutedEventArgs e)
        {
            Kit.Msg(await AtSvc.QueryWithChild());
        }

        #endregion

        #region 缓存
        async void OnInsertCache(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.InsertCache(), "InsertCache");
        }

        async void OnUpdateCache(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.UpdateCache(), "UpdateCache");
        }

        async void OnDelCache(object sender, RoutedEventArgs e)
        {
            Msg(await AtSvc.DelCache(), "DelCache");
        }

        async void OnCacheByKey(object sender, RoutedEventArgs e)
        {
            Kit.Msg(await AtSvc.CacheByKey());
        }
        #endregion
    }
}