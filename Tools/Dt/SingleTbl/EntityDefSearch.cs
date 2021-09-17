        #region 搜索
        string _query;

        public void OnSearch(string p_txt)
        {
            if (!string.IsNullOrEmpty(p_txt))
            {
                _query = p_txt;
                Title = "$entitytitle$列表 - " + p_txt;
                Query();
            }

            NaviTo(this);
        }

        void OnToSearch(object sender, Mi e)
        {
            NaviTo(_win.Search);
        }

        async void Query()
        {
            if (string.IsNullOrEmpty(_query) || _query == "#全部")
            {
                _lv.Data = await AtCm.Query<$entityname$Obj>("$entitytitle$-全部");
            }
            else
            {
                _lv.Data = await AtCm.Query<$entityname$Obj>("$entitytitle$-模糊查询", new { ID = $"%{_query}%" });
            }
        }
        #endregion