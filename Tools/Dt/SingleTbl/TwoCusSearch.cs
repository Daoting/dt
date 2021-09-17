        #region 搜索
        Row _query;
        $entityname$Search _search;

        async void OnToSearch(object sender, Mi e)
        {
            if (_search == null)
                _search = new $entityname$Search();
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
                _lv.Data = await AtCm.Query<$entityname$Obj>("$entitytitle$-全部");
            }
            else
            {
                
            }
        }
        #endregion