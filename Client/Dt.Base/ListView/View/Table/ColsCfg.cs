#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Markup;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// AutoSaveCols 时管理列设置
    /// </summary>
    class ColsCfg
    {
        readonly Lv _owner;
        Cols _cols;
        string _key;

        public ColsCfg(Lv p_owner)
        {
            _owner = p_owner;
        }

        /// <summary>
        /// 配置变化事件
        /// </summary>
        public event Action CfgChanged;
        
        public Cols Cols => _cols;
        
        public async Task Init()
        {
            var cols = _owner.View as Cols;
            StringBuilder sb = new StringBuilder(_owner.BaseUri.AbsoluteUri);
            foreach (var col in cols.OfType<Col>())
            {
                if (string.IsNullOrEmpty(col.ID))
                    continue;

                sb.Append("#");
                sb.Append(col.ID);
                if (!string.IsNullOrEmpty(col.Title))
                {
                    sb.Append("/");
                    sb.Append(col.Title);
                }
            }
            _key = sb.ToString();

            var cookie = await ColsCfgX.GetByID(_key);
            if (cookie != null)
            {
                // 加载历史布局
                ApplyLayout(cookie.Layout);
            }

            if (_cols == null)
                LoadDefaultCols();
            AttachEvent();
        }

        public async Task<bool> ResetCfg()
        {
            bool suc = await ColsCfgX.DelByID(_key, true, false);
            if (!suc)
                return false;

            LoadDefaultCols();
            AttachEvent();
            return true;
        }
        
        void LoadDefaultCols()
        {
            _cols = new Cols();
            foreach (var col in (_owner.View as Cols).OfType<Col>())
            {
                if (!string.IsNullOrEmpty(col.ID))
                    _cols.Add(col.Clone());
            }
        }

        void AttachEvent()
        {
            _cols.LayoutChanged += SaveLayout;
        }
        
        async void SaveLayout()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, JsonOptions.UnsafeWriter))
                {
                    _cols.WriteJson(writer);
                }
                var json = Encoding.UTF8.GetString(stream.ToArray());
                var cookie = await ColsCfgX.GetByID(_key);
                if (cookie != null)
                {
                    cookie.Layout = json;
                }
                else
                {
                    cookie = new ColsCfgX(_key, json);
                }
                if (await cookie.Save(false))
                {
                    CfgChanged?.Invoke();
                }
            }
        }

        bool ApplyLayout(string p_content)
        {
            if (string.IsNullOrEmpty(p_content))
                return false;

            bool succ = true;
            try
            {
                _cols = new Cols();
                Utf8JsonReader reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(p_content));
                _cols.ReadJson(ref reader);
            }
            catch
            {
                succ = false;
                _cols = null;
            }
            return succ;
        }
    }
}
