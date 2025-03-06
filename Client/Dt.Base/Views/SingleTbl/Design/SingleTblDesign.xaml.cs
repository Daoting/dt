#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using System.Text.Json;
#endregion

namespace Dt.Base.Views
{
    [ViewParamsEditor("通用单表视图")]
    public sealed partial class SingleTblDesign : Dlg, IViewParamsEditor
    {
        static Table _entityCls;
        SingleTblCfg _cfg;
        
        public SingleTblDesign()
        {
            InitializeComponent();
        }

        public async Task<string> ShowDlg(string p_params)
        {
            if (string.IsNullOrEmpty(p_params))
            {
                _cfg = new SingleTblCfg();
            }
            else
            {
                _cfg = JsonSerializer.Deserialize<SingleTblCfg>(p_params);
            }
            
            _fvMain.Data = _cfg;
            _fvList.Data = _cfg.ListCfg;
            _fvForm.Data = _cfg.FormCfg;
            
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

        void EditQueryFvXaml(object sender, RoutedEventArgs e)
        {

        }

        void EditListXaml(object sender, RoutedEventArgs e)
        {

        }

        void EditFormXaml(object sender, RoutedEventArgs e)
        {

        }

        void OnLoadEntityCls(CList p_list, AsyncArgs p_args)
        {
            if (_entityCls == null)
            {
                string prefix = "Tbl-";
                _entityCls = new Table { { "cls" }, { "tbl" }, { "group" } };
                foreach (var item in Kit.AllAliasTypes)
                {
                    if (!item.Key.StartsWith(prefix))
                        continue;

                    int index = item.Value.AssemblyQualifiedName.IndexOf(", Version=");
                    _entityCls.AddRow(new
                    {
                        cls = item.Value.AssemblyQualifiedName.Substring(0, index),
                        tbl = item.Key.Substring(prefix.Length),
                        group = "远程库",
                    });
                }

                foreach (var item in Kit.AllSqliteDbs)
                {
                    foreach (var tbl in item.Value.Tables)
                    {
                        int index = tbl.AssemblyQualifiedName.IndexOf(", Version=");
                        _entityCls.AddRow(new
                        {
                            cls = tbl.AssemblyQualifiedName.Substring(0, index),
                            tbl = tbl.Name.TrimEnd('X') + "，库" + item.Key,
                            group = "本地库",
                        });
                    }
                }
            }
            
            p_list.Data = _entityCls;
        }

        async void OnClsChanged(FvCell arg1, object arg2)
        {
            if (!string.IsNullOrEmpty(_cfg.QueryFvXaml)
                || _cfg.ListCfg.IsChanged
                || _cfg.FormCfg.IsChanged)
            {
                if (await Kit.Confirm("实体类型已修改，是否清空所有配置？"))
                {
                    _cfg.QueryFvXaml = null;
                    _cfg.ListCfg = new SingleTblListCfg();
                    _cfg.FormCfg = new SingleTblFormCfg();

                    _fvMain.Data = null;
                    _fvMain.Data = _cfg;
                    _fvList.Data = _cfg.ListCfg;
                    _fvForm.Data = _cfg.FormCfg;
                }
            }
        }
    }
}