#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
#endregion

namespace Dt.Mgr.Module
{
    [ViewParamsEditor(LobViews.通用报表)]
    public partial class RptViewParamsDlg : Dlg, IViewParamsEditor
    {
        string _params;

        public RptViewParamsDlg()
        {
            InitializeComponent();
            IsPinned = true;
        }

        public async Task<string> ShowDlg(string p_params)
        {
            _params = p_params;

            RptTbl = new Table { { "id", typeof(long) }, { "uri" } };
            ParamsTbl = new Table { { "rptid", typeof(long) }, { "name" }, { "val" } };
            ParseParams();

            RptForm = new(this);
            ParamsForm = new(this);
            
            if (!Kit.IsPhoneUI)
            {
                Width = 600;
                Height = 600;
            }

            if (await ShowAsync())
            {
                return GetResult();
            }
            return null;
        }

        public RptViewRptInoForm RptForm { get; private set; }

        public RptViewParamsForm ParamsForm { get; private set; }

        public Table RptTbl { get; private set; }

        public Table ParamsTbl { get; private set; }

        public Row SelectedRpt => _lvRpt.SelectedRow;

        public void DeleteRpt(Row p_row)
        {
            if (p_row == null || p_row.ID < 0)
                return;

            int i = 0;
            while (i < ParamsTbl.Count)
            {
                if (ParamsTbl[i].ID == p_row.ID)
                {
                    ParamsTbl.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            RptTbl.Remove(p_row);
        }

        string GetResult()
        {
            using (var stream = new MemoryStream())
            {
                using (var w = new Utf8JsonWriter(stream, JsonOptions.IndentedWriter))
                {
                    w.WriteStartObject();
                    foreach (var rpt in RptTbl)
                    {
                        var uri = rpt.Str("uri");
                        if (uri == "")
                            continue;

                        w.WritePropertyName(uri);
                        w.WriteStartObject();
                        foreach (var par in ParamsTbl)
                        {
                            if (rpt.ID == par.Long("rptid") && par.Str("name") != "")
                                w.WriteString(par.Str("name"), par.Str("val"));
                        }
                        w.WriteEndObject();
                    }
                    w.WriteEndObject();
                }
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        async void OnAddRpt(object sender, RoutedEventArgs e)
        {
            await RptForm.Query(-1);
        }

        async void OnRptSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_lvRpt.SelectedItem is Row row)
            {
                await RptForm.Query(row.ID, false);
            }
            else
            {
                await RptForm.Query(null);
            }
            _lvParams.Refresh();
        }

        async void OnRptDoubleClick(object obj)
        {
            await RptForm.Query(_lvRpt.SelectedRow.ID);
        }

        async void OnEditRpt(Mi e)
        {
            await RptForm.Query(e.Row.ID);
        }

        async void OnDelRpt(Mi e)
        {
            if (await Kit.Confirm("确认要删除选择的数据吗？"))
                DeleteRpt(e.Row);
        }

        void OnOk()
        {
            Close(true);
        }

        /*
         视图参数格式：
        
         {
             "报表模板Uri1":
             {
                 "参数名1": 值,
                 "参数名2": 值
             },
             "报表模板Uri2":
             {
                 "参数名1": 值,
                 "参数名2": 值
             }
         }
        
         */
        async void ParseParams()
        {
            if (!string.IsNullOrEmpty(_params))
            {
                JsonObject obj = null;
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(_params)))
                {
                    try
                    {
                        obj = await JsonSerializer.DeserializeAsync<JsonObject>(ms, JsonOptions.UnsafeSerializer);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "菜单报表参数json格式错误！");
                    }
                }

                int index = 1;
                foreach (var kv in obj)
                {
                    if (kv.Value is not JsonObject child)
                        continue;

                    var rpt = RptTbl.AddRow();
                    rpt["id"] = index;
                    rpt["uri"] = kv.Key;
                    foreach (var ckv in child)
                    {
                        var par = ParamsTbl.AddRow();
                        par["rptid"] = index;
                        par["name"] = ckv.Key;
                        par["val"] = ckv.Value.ToString();
                    }
                    index++;
                }
            }

            _lvRpt.Data = RptTbl;
            _lvParams.Data = ParamsTbl;
            _lvParams.Filter = OnFilter;
        }

        bool OnFilter(object obj)
        {
            if (_lvRpt.SelectedItem is Row parRow
                && obj is Row curRow)
            {
                return curRow.Long("rptid") == parRow.Long("id");
            }
            return false;
        }

        async void OnAddParams(object sender, RoutedEventArgs e)
        {
            if (_lvRpt.SelectedItem == null)
            {
                if (RptTbl.Count > 0)
                {
                    _lvRpt.SelectedIndex = 0;
                }
                else
                {
                    return;
                }
            }
            await ParamsForm.Query(-1);
        }

        async void OnParamsDoubleClick(object obj)
        {
            await ParamsForm.Query(_lvParams.SelectedRow.ID);
        }

        async void OnEditParams(Mi e)
        {
            await ParamsForm.Query(e.Row.ID);
        }

        async void OnDelParams(Mi e)
        {
            if (await Kit.Confirm("确认要删除选择的数据吗？"))
                _lvParams.Table.Remove(e.Row);
        }

        async void OnParamsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_lvParams.SelectedItem is Row r)
            {
                await ParamsForm.Query(r.ID, false);
            }
            else
            {
                await ParamsForm.Query(null);
            }
        }
    }
}