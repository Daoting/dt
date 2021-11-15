        #region 搜索
        /// <summary>
        /// 获取设置查询串
        /// </summary>
        public string QueryStr { get; set; }

        async void OnToSearch(object sender, Mi e)
        {
            var txt = await Forward<string>(_lzSm.Value);
            if (!string.IsNullOrEmpty(txt))
            {
                QueryStr = txt;
                Title = "$maintitle$列表 - " + txt;
                Query();
            }
        }

        Lazy<SearchMv> _lzSm = new Lazy<SearchMv>(() => new SearchMv
        {
            Placeholder = "$maintitle$名称",
            Fixed = { "全部", },
        });

        async void Query()
        {
            if (string.IsNullOrEmpty(QueryStr) || QueryStr == "#全部")
            {
                _lv.Data = await $agent$.Query<$maincls$Obj>("$maintitle$-全部");
            }
            else
            {
                _lv.Data = await $agent$.Query<$maincls$Obj>("$maintitle$-模糊查询", new { ID = $"%{QueryStr}%" });
            }
        }
        #endregion