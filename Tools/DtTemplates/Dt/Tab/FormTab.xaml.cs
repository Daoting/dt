#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace $rootnamespace$
{
    public sealed partial class $safeitemname$ : Tab
    {
        public $safeitemname$()
        {
            InitializeComponent();
        }

        public async void Update(long p_id)
        {
            if (!await _fv.DiscardChanges())
                return;

            if (p_id > 0)
            {
                //Data = await MyEntityX.GetByID(p_id);
            }
            else
            {
                Create();
            }
        }

        public void Clear()
        {
            //Data = null;
        }

        void Create()
        {
            //Data = await MyEntityX.New();
        }

        void Save()
        {
            //if (await Data.Save())
            //{
            //    _win.List.Update();
            //}
        }

        void Delete()
        {
            //var d = Data;
            //if (d == null)
            //    return;

            //if (!await Kit.Confirm("确认要删除吗？"))
            //{
            //    Kit.Msg("已取消删除！");
            //    return;
            //}

            //if (d.IsAdded)
            //{
            //    Clear();
            //    return;
            //}

        }

        //MyEntityX Data
        //{
        //    get { return _fv.Data.To<MyEntityX>(); }
        //    set { _fv.Data = value; }
        //}

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }
    }
}
