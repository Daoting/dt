#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class RptDesignHome : Win
    {
        RptDesignInfo _info;

        public RptDesignHome(RptDesignInfo p_info)
        {
            InitializeComponent();

            _info = p_info;

            Nl<GroupData<Nav>> ds = new Nl<GroupData<Nav>>();
            var group = new GroupData<Nav>
            {
                new Nav("模板编辑", typeof(RptDesignWin), Icons.修改) { Desc = "编辑报表模板", Params = _info },
                new Nav("参数定义", typeof(ParamsWin), Icons.信件) { Desc = "报表输入参数可以为内部宏参数或外部输入参数", Params = _info },
                new Nav("Db数据源", typeof(DbDataWin), Icons.数据库) { Desc = "定义查询数据的SQL语句", Params = _info },
                new Nav("脚本数据源", typeof(ScriptDataWin), Icons.U盘) { Desc = "程序中动态提供的数据", Params = _info },
                new Nav("页面设置", typeof(PageSettingWin), Icons.文件) { Desc = "设置报表页面大小", Params = _info },
                new Nav("报表预览设置", typeof(ViewSettingWin), Icons.大图标) { Desc = "定制报表预览窗口的内容", Params = _info },
            };
            group.Title = "设计";
            ds.Add(group);

            group = new GroupData<Nav>
            {
                new Nav("保存模板", null, Icons.保存) { Callback = OnSave, Desc = "保存报表模板Xml内容" },
                new Nav("查看模板Xml", null, Icons.导出) { Callback = OnExport, Desc = "浏览当前报表模板的Xml内容" },
                new Nav("导入", null, Icons.导入) { Callback = OnImport, Desc = "从外部导入报表模板" },
                new Nav("报表预览", null, Icons.门卫) { Callback = OnPreview, Desc = "浏览运行时报表" },
            };
            group.Title = "操作";
            ds.Add(group);

            _nav.Data = ds;
            _nav.Select(0);
        }

        void OnExport(Win p_win, Nav p_nav)
        {
            var tb = new TextBox { AcceptsReturn = true, Style = Res.FvTextBox };
            ScrollViewer.SetHorizontalScrollBarVisibility(tb, ScrollBarVisibility.Auto);
            ScrollViewer.SetVerticalScrollBarVisibility(tb, ScrollBarVisibility.Auto);
            tb.Text = Rpt.SerializeTemplate(_info.Root);
            LoadMain(tb);
        }

        async void OnImport(Win p_win, Nav p_nav)
        {
            if (_info.IsDirty && !await Kit.Confirm("当前模板已修改，导入新模板会丢失修改内容，继续导入吗？"))
                return;

            var picker = Kit.GetFileOpenPicker();
            picker.FileTypeFilter.Add(".xml");
            StorageFile sf = await picker.PickSingleFileAsync();
            if (sf != null)
            {
                try
                {
                    using (var stream = await sf.OpenStreamForReadAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        await _info.ImportTemplate(reader.ReadToEnd());
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("导入报表模板时异常：{0}", ex.Message));
                }
            }
        }

        void OnPreview(Win p_win, Nav p_nav)
        {
            // 比较窗口类型和初始参数，关闭旧窗口
            var info = new RptInfo { Name = _info.Name, Root = _info.Root };
            Win win;
            if (!Kit.IsPhoneUI
                && (win = Desktop.Inst.ActiveWin(typeof(RptViewWin), info)) != null)
            {
                win.Close();
            }

            Rpt.Show(info);
        }

        void OnSave(Win p_win, Nav p_nav)
        {
            if (_info.IsDirty)
                _info.SaveTemplate();
            else
                Kit.Msg("报表模板未修改，无需保存！");
        }

        protected override async Task<bool> OnClosing()
        {
            if (_info.IsDirty)
            {
                return await Kit.Confirm("当前模板已修改，窗口关闭会丢失修改内容，确认要关闭吗？");
            }
            return true;
        }
    }
}
