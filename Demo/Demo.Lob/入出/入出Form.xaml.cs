#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 物资主单X;

    [WfForm("物资入出")]
    public sealed partial class 入出Form : WfForm
    {
        物资入出类别X _lb;

        public 入出Form()
        {
            InitializeComponent();
            ShowVeil = true;
        }

        protected override async Task OnAdd()
        {
            var dlg = new 入出类别Dlg();
            if (await dlg.ShowDlg() && dlg.入出类别 != null)
            {
                _lb = dlg.入出类别;
                Title = _lb.名称 + "单";
                _fv.Data = await A.NewView1(_info.PrcInst.ID, _lb);
            }
            else
            {
                Close();
            }
        }

        protected override async Task OnAddChild(Fv p_fv)
        {
            p_fv.Data = await 物资详单X.NewView1(单据id: _fv.Row.ID);
        }

        protected override async Task OnGet()
        {
            var x = await A.View1.GetByID(_args.ID);
            _lb = await 物资入出类别X.GetByID(x.入出类别id);
            Title = _lb.名称 + "单";
            _fv.Data = x;
            _lv详单.Data = await 物资详单X.View1.Query($"where 单据id={_args.ID}");
        }

        protected override async Task OnLoad()
        {
            Menu = await CreateMenu();
            
            if (_lb.名称 != "外购入库")
            {
                _fv.Hide("供应商", "发票金额");
                var cols = _lv详单.View as Cols;
                string[] hs = new string[] { "随货单号", "发票号", "发票日期", "发票金额" };
                cols.Hide(hs);
                _fv详单.Hide(hs);
            }

            if (_lb.名称 != "盘盈" && _lb.名称 != "盘亏")
            {
                var cols = _lv详单.View as Cols;
                string[] hs = new string[] { "盘点时间", "盘点金额" };
                cols.RemoveAt(cols.Count - 1);
                cols.Hide(hs);
                _fv详单.Hide(hs);
            }

            if (_info.State == "填写")
            {
                IsReadOnly = _info.IsReadOnly;
                _fv.Hide("审核人", "审核日期");
            }
            else
            {
                IsReadOnly = true;
            }

        }

        protected override Task<bool> OnSave(IEntityWriter w)
        {
            var main = _fv.Data as 物资主单X;
            var detail = _lv详单.Data as Table<物资详单X>;

            if (_lb.名称 == "外购入库")
            {
                if (!main.供应商id.HasValue)
                {
                    Throw.Msg("供应商不可为空！", main.Cells["供应商"]);
                }
            }

            foreach (var d in detail)
            {
                if (!d.物资id.HasValue)
                    ThrowMsg(d, "未选择物资种类！", "物资名称");

                if (d.批次 == "")
                    ThrowMsg(d, "批次不可为空！", "批次");

                if (!d.数量.HasValue)
                    ThrowMsg(d, "数量不可为空！", "数量");

                if (!d.单价.HasValue)
                    ThrowMsg(d, "单价不可为空！", "单价");

                if (!d.金额.HasValue)
                    d.金额 = d.数量 * d.单价;
            }
            return Task.FromResult(true);
        }

        void ThrowMsg(物资详单X p_x, string p_msg, string p_col)
        {
            _fi详单.ShowFv(p_x);
            Throw.Msg(p_msg, p_x.Cells[p_col]);
        }
        
        protected override async Task<bool> OnSend(IEntityWriter w)
        {
            if (_info.IsLastAtv)
            {
                if (!await Kit.Confirm("[审核] 通过后会同步物资库存，只有[冲销]可进行反操作！\r\n确认继续执行吗？"))
                    return false;
            }

            var detail = _lv详单.Data as Table<物资详单X>;
            if (detail == null || detail.Count == 0)
            {
                Kit.Warn("物资详单不可为空！");
                return false;
            }
            
            var x = _fv.Data as A;
            switch (_info.State)
            {
                case "填写":
                    x.填制人 = Kit.UserName;
                    x.填制日期 = Kit.Now;
                    break;

                case "审核":
                    x.审核人 = Kit.UserName;
                    x.审核日期 = Kit.Now;
                    await SyncStock(w);
                    break;

            }
            await w.Save(x);
            return true;
        }

        async Task SyncStock(IEntityWriter w)
        {
            var main = _fv.Data as 物资主单X;
            var detail = _lv详单.Data as Table<物资详单X>;
            
            foreach (var d in detail)
            {
                var x = await 物资库存X.First($"where 部门id={main.部门id} and 物资id={d.物资id} and 批次='{d.批次}'");
                if (x == null)
                {
                    x = await 物资库存X.New(main.部门id, d.物资id, d.批次);
                }
                
                x.可用数量 = (x.可用数量.HasValue ? x.可用数量.Value : 0) + d.数量.Value * main.入出系数.Value;
                x.可用金额 = x.可用数量 * d.单价.Value;
                await w.Save(x);
            }
        }
        
        protected override string PrcName => _lb?.名称 + "单";
    }
}