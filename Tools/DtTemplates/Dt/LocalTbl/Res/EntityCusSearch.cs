        #region 搜索
        /// <summary>
        /// 获取设置查询对象
        /// </summary>
        public Row QueryRow { get; set; }

        public void OnSearch(Row p_query)
        {
            if (p_query != null)
            {
                QueryRow = p_query;
                Query();
            }

            NaviTo(this);
        }

        void OnToSearch(object sender, Mi e)
        {
            NaviTo(_win.Search);
        }

        void Query()
        {
            if (QueryRow == null)
            {
                _lv.Data = AtLocal.Query<$entityname$>("select * from '$table$'");
            }
            else
            {
                
            }
        }
        #endregion