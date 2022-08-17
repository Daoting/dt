        #region 搜索
        string _query;

        async void OnToSearch(object sender, Mi e)
        {
            var txt = await Forward<string>(_lzSm.Value);
            if (!string.IsNullOrEmpty(txt))
            {
                _query = txt;
                Title = "$entitytitle$列表 - " + txt;
                Query();
            }
        }

        Lazy<SearchMv> _lzSm = new Lazy<SearchMv>(() => new SearchMv
        {
            Placeholder = "$entitytitle$名称",
            Fixed = { "全部", },
        });


        void Query()
        {
            if (string.IsNullOrEmpty(_query) || _query == "#全部")
            {
                _lv.Data = AtLocal.Query<$entityname$>("select * from '$table$'");
            }
            else
            {
                _lv.Data = AtLocal.Query<$entityname$>("$blursql$", new { input = $"%{_query}%" });
            }
        }
        #endregion