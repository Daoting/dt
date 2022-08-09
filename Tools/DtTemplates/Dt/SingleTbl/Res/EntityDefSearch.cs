        #region 搜索
        /// <summary>
        /// 获取设置查询串
        /// </summary>
        public string QueryStr { get; set; }

        public void OnSearch(string p_txt)
        {
            if (!string.IsNullOrEmpty(p_txt))
            {
                QueryStr = p_txt;
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
            if (string.IsNullOrEmpty(QueryStr) || QueryStr == "#全部")
            {
                _lv.Data = await $agent$.Query<$entityname$Obj>("$entitytitle$-全部");
            }
            else
            {
                _lv.Data = await $agent$.Query<$entityname$Obj>("$entitytitle$-模糊查询", new { ID = $"%{QueryStr}%" });
            }
        }
        #endregion