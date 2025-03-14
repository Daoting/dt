﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.Json;
#endregion

namespace Dt.Base.Views
{
    [View("通用单表视图")]
    public partial class SingleTblWin : Win
    {
        public SingleTblWin(string p_jsonCfg)
        {
            InitializeComponent();

            Cfg = JsonSerializer.Deserialize<SingleTblCfg>(p_jsonCfg);
            LoadCfg();
        }

        public SingleTblWin(SingleTblCfg p_cfg)
        {
            InitializeComponent();

            Cfg = p_cfg;
            LoadCfg();
        }

        public SingleTblCfg Cfg { get; }
        
        public SingleTblQuery Query => _query;
        
        public SingleTblList List => _list;
        
        public SingleTblForm Form { get; } = new SingleTblForm();

        async void LoadCfg()
        {
            await Cfg.Init();
            _query.LoadCfg(this);
            _list.LoadCfg(this);
            Form.LoadCfg(this);
        }
    }
}