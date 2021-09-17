        #region 搜索
        Row _query;

        public void OnSearch(Row p_query)
        {
            if (p_query != null)
            {
                _query = p_query;
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
            if (_query == null)
            {
                _lv.Data = await AtCm.Query<$entityname$Obj>("$entitytitle$-全部");
            }
            else
            {
                
            }
        }
        #endregion