        #region 搜索
        $maincls$Search _search;

        /// <summary>
        /// 获取设置查询对象
        /// </summary>
        public Row QueryRow { get; set; }

        async void OnToSearch(object sender, Mi e)
        {
            if (_search == null)
                _search = new $maincls$Search();
            var row = await Forward<Row>(_search);
            if (row != null)
            {
                QueryRow = row;
                Query();
            }
        }

        async void Query()
        {
            if (QueryRow == null)
            {
                _lv.Data = await $agent$.Query<$maincls$Obj>("$maintitle$-全部");
            }
            else
            {
                
            }
        }
        #endregion