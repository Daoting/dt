#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-26 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 实体表单配置
    /// </summary>
    public class EntityFormCfg
    {
        string _title;

        /// <summary>
        /// 表单的XAML
        /// </summary>
        public string Xaml { get; set; }

        /// <summary>
        /// Tab标题
        /// </summary>
        public string Title
        {
            get
            {
                if (!string.IsNullOrEmpty(_title))
                    return _title;
                return GetDefaultTitle();
            }
            set
            {
                var def = GetDefaultTitle();
                if (value == def)
                {
                    _title = null;
                }
                else if (_title != value)
                {
                    _title = value;
                }
            }
        }

        /// <summary>
        /// 是否显示添加菜单
        /// </summary>
        public bool ShowAddMi { get; set; } = true;

        /// <summary>
        /// 是否显示保存菜单
        /// </summary>
        public bool ShowSaveMi { get; set; } = true;

        /// <summary>
        /// 是否显示删除菜单
        /// </summary>
        public bool ShowDelMi { get; set; } = true;

        /// <summary>
        /// 配置是否有修改
        /// </summary>
        public bool IsChanged => !string.IsNullOrEmpty(Xaml) || !ShowAddMi || !ShowDelMi || !ShowSaveMi || !string.IsNullOrEmpty(_title);

        /// <summary>
        /// 是否自定义标题
        /// </summary>
        public bool IsCustomTitle => !string.IsNullOrEmpty(_title) && _title != GetDefaultTitle();

        /// <summary>
        /// 所属实体配置
        /// </summary>
        public EntityCfg Owner { get; set; }

        string GetDefaultTitle()
        {
            if (Owner != null && !string.IsNullOrEmpty(Owner.Cls))
                return Type.GetType(Owner.Cls).Name.TrimEnd('X') + "表单";
            return "表单";
        }
    }
}