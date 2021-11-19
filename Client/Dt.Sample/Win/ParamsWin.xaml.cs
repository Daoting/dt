#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.Sample
{
    public partial class ParamsWin : Win
    {
        public ParamsWin()
        {
            InitializeComponent();
            _tb.Text = "无参数";
            Params = 0;
        }

        public ParamsWin(int p_params)
        {
            InitializeComponent();
            _tb.Text = "参数值：" + p_params.ToString();
            Params = p_params;
        }
    }
}