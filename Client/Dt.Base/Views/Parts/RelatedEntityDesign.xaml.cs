#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
#endregion

namespace Dt.Base.Views
{
    public sealed partial class RelatedEntityDesign : Dlg, IViewParamsEditor
    {
        RelatedEntityCfg _cfg;

        public RelatedEntityDesign()
        {
            InitializeComponent();
            IsPinned = true;
            ShowVeil = true;
        }

        [UnconditionalSuppressMessage("AOT", "IL3050")]
        public async Task<string> ShowDlg(string p_params)
        {
            _cfg = string.IsNullOrEmpty(p_params) ? new RelatedEntityCfg() : JsonSerializer.Deserialize<RelatedEntityCfg>(p_params);
            _fv.Data = _cfg;
            if (await ShowAsync())
            {
                return GetResult();
            }
            return null;
        }

        string GetResult()
        {
            return _cfg.Serialize();
        }
        
        async void EditListXaml(object sender, RoutedEventArgs e)
        {
            var info = new LvDesignInfo { Xaml = _cfg.ListXaml, };
            if (_cfg.Table != null)
            {
                var cols = new List<EntityCol>();
                foreach (var col in _cfg.Table.Columns)
                {
                    cols.Add(new EntityCol(col.Name, col.Type));
                }
                info.Cols = cols;
            }

            var xaml = await LvDesign.ShowDlg(info);
            if (!string.IsNullOrEmpty(xaml))
                _fv["ListXaml"].Val = xaml;
        }

        void OnLoadEntityCls(CList p_list, AsyncArgs p_args)
        {
            p_list.Data = EntityDesign.GetAllEntityCls();
        }

        void OnClsChanged(FvCell arg1, object arg2)
        {
            // 清空关联字段
            var list = (CList)_fv["MainFk"];
            list.Value = null;
            list.Data = null;

            list = (CList)_fv["RelatedFk"];
            list.Value = null;
            list.Data = null;

            _fv.Data = null;
            _fv.Data = _cfg;
        }

        void OnLoadKeyCols(CList arg1, AsyncArgs arg2)
        {
            if (_cfg.MiddleTable != null)
            {
                var cols = new Nl<string>();
                foreach (var col in _cfg.MiddleTable.PrimaryKey)
                {
                    cols.Add(col.Name);
                }
                arg1.Data = cols;
            }
        }
    }
}