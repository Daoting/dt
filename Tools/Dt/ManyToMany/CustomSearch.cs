        #region 搜索
        Row _query;
        $maincls$Search _search;

        async void OnToSearch(object sender, Mi e)
        {
            if (_search == null)
                _search = new $maincls$Search();
            var row = await Forward<Row>(_search);
            if (row != null)
            {
                _query = row;
                Query();
            }
        }

        async void Query()
        {
            if (_query == null)
            {
                _lv.Data = await $agent$.Query<$maincls$Obj>("$maintitle$-全部");
            }
            else
            {
                
            }
        }
        #endregion