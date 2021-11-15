#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Model
{
    [View("基础选项")]
    public partial class BaseOption : Win
    {
        public BaseOption()
        {
            InitializeComponent();
            LoadCategory();
        }

        async void LoadCategory()
        {
            var data = await AtCm.Query("选项-所有分类");
            _lvCate.Data = data;
            _fv["Category"].To<CList>().Data = data;
        }

        async void OnCateClick(object sender, ItemClickArgs e)
        {
            _lv.Data = await AtCm.Query<OptionObj>("选项-分类选项", new { Category = _lvCate.SelectedRow.Str(0) });
            SelectTab("选项列表");
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            var op = _lv.SelectedRow.To<OptionObj>();
            _fv.Data = await AtCm.First<OptionObj>("选项-选项", new { Category = op.Category, Name = op.Name });
            SelectTab("选项");
        }

        void OnMoveUp(object sender, Mi e)
        {
            var src = e.Data.To<OptionObj>();
            int index = _lv.Data.IndexOf(src);
            if (index > 0)
                Exchange(src, _lv.Data[index - 1].To<OptionObj>());
        }

        void OnMoveDown(object sender, Mi e)
        {
            var src = e.Data.To<OptionObj>();
            int index = _lv.Data.IndexOf(src);
            if (index < _lv.Data.Count - 1 && index >= 0)
                Exchange(src, _lv.Data[index + 1].To<OptionObj>());
        }

        async void Exchange(OptionObj p_src, OptionObj p_tgt)
        {
            var tbl = Table<OptionObj>.Create();

            var save = (OptionObj)tbl.AddRow(new { Name = p_src.Name, Category = p_src.Category });
            save.AcceptChanges();
            save.Dispidx = p_tgt.Dispidx;

            save = (OptionObj)tbl.AddRow(new { Name = p_tgt.Name, Category = p_tgt.Category });
            save.AcceptChanges();
            save.Dispidx = p_src.Dispidx;

            if (await AtCm.BatchSave(tbl))
            {
                _lv.Data = await AtCm.Query<OptionObj>("选项-分类选项", new { Category = p_src.Category });
                AtCm.PromptForUpdateModel();
            }
        }

        async void OnAdd(object sender, Mi e)
        {
            string cate = "";
            var op = _fv.Data.To<OptionObj>();
            if (op != null)
                cate = op.Category;
            else if (_lvCate.SelectedRow != null)
                cate = _lvCate.SelectedRow.Str(0);

            _fv.Data = new OptionObj(
                Name: "新选项",
                Category: cate,
                Dispidx: await AtCm.NewSeq("sq_option"));
        }

        async void OnSave(object sender, Mi e)
        {
            var op = _fv.Data.To<OptionObj>();
            if (await AtCm.Save(op))
            {
                LoadCategory();
                _lv.Data = await AtCm.Query<OptionObj>("选项-分类选项", new { Category = op.Category });
                AtCm.PromptForUpdateModel();
            }
        }

        void OnListDel(object sender, Mi e)
        {
            DelOption(e.Data.To<OptionObj>());
        }

        void OnDel(object sender, Mi e)
        {
            var op = _fv.Data.To<OptionObj>();
            if (op != null)
                DelOption(op);
        }

        async void DelOption(OptionObj p_option)
        {
            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (p_option.IsAdded)
            {
                _fv.Data = null;
                return;
            }

            if (await AtCm.Delete(p_option))
            {
                LoadCategory();
                _lv.Data = await AtCm.Query<OptionObj>("选项-分类选项", new { Category = p_option.Category });
                _fv.Data = null;
                AtCm.PromptForUpdateModel();
            }
        }
    }

    public partial class OptionObj
    {
        async Task OnSaving()
        {
            Throw.If(string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Category), "选项名称和所属分类不可为空！");
            if (Cells["Name"].IsChanged || Cells["Category"].IsChanged)
            {
                var op = await AtCm.First<OptionObj>("选项-选项", new { Category = Category, Name = Name });
                Throw.If(op != null, "选项重复！");
            }
        }
    }

    #region 自动生成
    [Tbl("cm_option")]
    public partial class OptionObj : Entity
    {
        #region 构造方法
        OptionObj() { }

        public OptionObj(
            string Name,
            string Category,
            int Dispidx = default)
        {
            AddCell<string>("Name", Name);
            AddCell<string>("Category", Category);
            AddCell<int>("Dispidx", Dispidx);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 选项名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 所属分类
        /// </summary>
        public string Category
        {
            get { return (string)this["Category"]; }
            set { this["Category"] = value; }
        }

        new public long ID { get { return -1; } }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["Dispidx"]; }
            set { this["Dispidx"] = value; }
        }
        #endregion
    }
    #endregion
}