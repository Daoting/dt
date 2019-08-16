#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
#endregion

namespace Dt.Core.Model
{
    /// <summary>
    /// 业务菜单
    /// </summary>
    [Table]
    public class OmMenu : ViewModel
    {
        string _icon;
        string _warning;

        /// <summary>
        /// 菜单ID
        /// </summary>
        [PrimaryKey, MaxLength(32)]
        public string ID { get; set; }

        /// <summary>
        /// 父菜单项ID
        /// </summary>
        [Indexed, MaxLength(32)]
        public string ParentID { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// 菜单类型是否为分组
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// 菜单对应的视图名称
        /// </summary>
        [MaxLength(128)]
        public string ViewName { get; set; }

        /// <summary>
        /// 菜单参数
        /// </summary>
        public string Params { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get
            {
                if (!string.IsNullOrEmpty(_icon))
                    return _icon;
                return (IsGroup ? "文件夹" : "文件");
            }
            set { _icon = value; }
        }

        /// <summary>
        /// 提供提示信息的服务名称，空表示无提示信息
        /// </summary>
        public string SrvName { get; set; }

        /// <summary>
        /// 菜单描述
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DispIdx { get; set; }

        /// <summary>
        /// 菜单项醒目提示的数字
        /// </summary>
        [Ignore]
        public string Warning
        {
            get { return _warning; }
        }

        /// <summary>
        /// 设置菜单项醒目提示的数字
        /// </summary>
        /// <param name="p_num"></param>
        public void SetWarningNum(int p_num)
        {
            string msg;
            if (p_num == 0)
                msg = null;
            else if (p_num < 100)
                msg = p_num.ToString();
            else
                msg = "┅";

            if (msg != _warning)
            {
                _warning = msg;
                OnPropertyChanged("Warning");
            }
        }
    }
}
