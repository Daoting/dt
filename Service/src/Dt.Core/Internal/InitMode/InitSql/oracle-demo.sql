-- ----------------------------
-- Table structure for demo_cache_tbl1
-- ----------------------------
CREATE TABLE DEMO_CACHE_TBL1 (
  ID NUMBER(19) NOT NULL,
  PHONE VARCHAR2(255) NOT NULL,
  NAME VARCHAR2(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_cache_tbl1
-- ----------------------------
INSERT INTO DEMO_CACHE_TBL1 VALUES ('454454068519129088', 'ca4f271212bc4add946c55feed7400bb', '3917');
INSERT INTO DEMO_CACHE_TBL1 VALUES ('484620968746045440', '3f435d84c76a46e29002f467a4cd0187', '7425');
INSERT INTO DEMO_CACHE_TBL1 VALUES ('484621133057904640', '3329d521b2134b0195083828152cb5b0', '1786');
INSERT INTO DEMO_CACHE_TBL1 VALUES ('484624179913576448', 'd80e785d1d44472abe88723e4ed17ca8', '156');

-- ----------------------------
-- Table structure for demo_child_tbl1
-- ----------------------------
CREATE TABLE DEMO_CHILD_TBL1 (
  ID NUMBER(19) NOT NULL,
  PARENT_ID NUMBER(19) NOT NULL,
  ITEM_NAME VARCHAR2(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_child_tbl1
-- ----------------------------
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588385740705792', '443588385522601984', '修改370');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588388055961600', '443588385522601984', '修改370');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588388299231232', '443588385522601984', '修改370');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588583695077376', '443588583535693824', '新增0');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588583913181184', '443588583535693824', '新增1');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588584148062208', '443588583535693824', '新增2');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588895562551296', '443588895352836096', '新增0');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588895814209536', '443588895352836096', '新增1');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588896132976640', '443588895352836096', '新增2');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588932807970816', '443588932694724608', '新增0');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588933026074624', '443588932694724608', '新增1');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('443588933248372736', '443588932694724608', '新增2');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('445140374660337664', '445140374589034496', '新增0');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('445140374786166784', '445140374589034496', '新增1');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('446130095746207744', '446130095742013440', '新增0');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('446130095754596352', '446130095742013440', '新增1');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('484622270955802624', '484622270804807680', '新增0');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('484622271224238080', '484622270804807680', '新增1');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('484622408784826368', '484622408633831424', '新增0');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('484622408994541568', '484622408633831424', '新增1');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('484623850744598528', '484623850568437760', '新增0');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('484623850987868160', '484623850568437760', '新增1');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('484623946806743040', '484623946693496832', '新增0');
INSERT INTO DEMO_CHILD_TBL1 VALUES ('484623947016458240', '484623946693496832', '新增1');

-- ----------------------------
-- Table structure for demo_child_tbl2
-- ----------------------------
CREATE TABLE DEMO_CHILD_TBL2 (
  ID NUMBER(19) NOT NULL,
  GROUP_ID NUMBER(19) NOT NULL,
  ITEM_NAME VARCHAR2(255)
)
;

-- ----------------------------
-- Records of demo_child_tbl2
-- ----------------------------
INSERT INTO DEMO_CHILD_TBL2 VALUES ('443588388416671744', '443588385522601984', '修改975');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('443588583799934976', '443588583535693824', '新增0');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('443588584039010304', '443588583535693824', '新增1');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('443588584374554624', '443588583535693824', '新增2');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('443588895692574720', '443588895352836096', '新增0');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('443588895931650048', '443588895352836096', '新增1');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('443588896258805760', '443588895352836096', '新增2');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('443588932917022720', '443588932694724608', '新增0');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('443588933135126528', '443588932694724608', '新增1');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('443588933361618944', '443588932694724608', '新增2');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('445140374735835136', '445140374589034496', '新增0');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('445140374819721216', '445140374589034496', '新增1');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('446130095750402048', '446130095742013440', '新增0');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('446130095754596353', '446130095742013440', '新增1');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('484622271115186176', '484622270804807680', '新增0');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('484622271333289984', '484622270804807680', '新增1');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('484622408889683968', '484622408633831424', '新增0');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('484622409107787776', '484622408633831424', '新增1');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('484623850878816256', '484623850568437760', '新增0');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('484623851092725760', '484623850568437760', '新增1');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('484623946907406336', '484623946693496832', '新增0');
INSERT INTO DEMO_CHILD_TBL2 VALUES ('484623947121315840', '484623946693496832', '新增1');

-- ----------------------------
-- Table structure for demo_crud
-- ----------------------------
CREATE TABLE DEMO_CRUD (
  ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(255) NOT NULL,
  DISPIDX NUMBER(9) NOT NULL,
  MTIME DATE NOT NULL,
  ENABLE_INSERT_EVENT CHAR(1) DEFAULT 0 NOT NULL,
  ENABLE_NAME_CHANGED_EVENT CHAR(1) DEFAULT 0 NOT NULL,
  ENABLE_DEL_EVENT CHAR(1) DEFAULT 0 NOT NULL
)
;

ALTER TABLE DEMO_CRUD ADD CHECK (ENABLE_INSERT_EVENT in (0,1));
ALTER TABLE DEMO_CRUD ADD CHECK (ENABLE_NAME_CHANGED_EVENT in (0,1));
ALTER TABLE DEMO_CRUD ADD CHECK (ENABLE_DEL_EVENT in (0,1));

COMMENT ON COLUMN DEMO_CRUD.ID IS '标识';
COMMENT ON COLUMN DEMO_CRUD.NAME IS '名称';
COMMENT ON COLUMN DEMO_CRUD.DISPIDX IS '显示顺序';
COMMENT ON COLUMN DEMO_CRUD.MTIME IS '最后修改时间';
COMMENT ON COLUMN DEMO_CRUD.ENABLE_INSERT_EVENT IS '#bool#true时允许发布插入事件';
COMMENT ON COLUMN DEMO_CRUD.ENABLE_NAME_CHANGED_EVENT IS '#bool#true时允许发布Name变化事件';
COMMENT ON COLUMN DEMO_CRUD.ENABLE_DEL_EVENT IS '#bool#true时允许发布删除事件';
COMMENT ON TABLE DEMO_CRUD IS '#demo#基础增删改';

-- ----------------------------
-- Records of demo_crud
-- ----------------------------
INSERT INTO DEMO_CRUD VALUES ('446127712370708480', '批增更944', '50', TO_DATE('2023-02-13 09:52:21', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('446127712387485696', '批量605', '51', TO_DATE('2023-02-13 09:52:21', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('446127744155144192', '批增更887', '52', TO_DATE('2023-02-13 09:52:28', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('446127778095452160', '批增更删501', '53', TO_DATE('2023-02-13 09:52:36', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('446127928557719552', '新增事件9083', '54', TO_DATE('2023-02-13 09:53:12', 'SYYYY-MM-DD HH24:MI:SS'), '1', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('447641397090078720', '领域服务', '61', TO_DATE('2023-02-17 14:07:07', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('447641397589200896', '服务更', '62', TO_DATE('2023-02-17 14:07:08', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484620702760062976', '单个9897', '63', TO_DATE('2023-05-30 15:09:40', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484620769650823168', '批量430', '64', TO_DATE('2023-05-30 15:09:56', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484620769889898496', '批量813', '65', TO_DATE('2023-05-30 15:09:56', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484620770128973824', '批量572', '66', TO_DATE('2023-05-30 15:09:56', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484620773429891072', '批增更218', '67', TO_DATE('2023-05-30 15:09:57', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484623044423208960', '单个5122', '68', TO_DATE('2023-05-30 15:18:58', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484623148454531072', '批量40', '69', TO_DATE('2023-05-30 15:19:23', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484623148689412096', '批量680', '70', TO_DATE('2023-05-30 15:19:23', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484623148932681728', '批量531', '71', TO_DATE('2023-05-30 15:19:23', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484623187683856384', '批增更615', '72', TO_DATE('2023-05-30 15:19:33', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484623231044571136', '批增更删992', '73', TO_DATE('2023-05-30 15:19:43', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484624288650907648', '领域服务', '74', TO_DATE('2023-05-30 15:23:55', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484624288994840576', '服务更', '75', TO_DATE('2023-05-30 15:23:55', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484956889089593344', '单个8461', '76', TO_DATE('2023-05-31 13:25:35', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484957035659546624', '单个8271', '77', TO_DATE('2023-05-31 13:26:09', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484957333266386944', '批量652', '78', TO_DATE('2023-05-31 13:27:20', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484957333782286336', '批量521', '79', TO_DATE('2023-05-31 13:27:21', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484957334516289536', '批量955', '80', TO_DATE('2023-05-31 13:27:21', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('484988812650369024', '批增更778', '81', TO_DATE('2023-05-31 15:32:23', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('486788489460862976', '单个4284', '82', TO_DATE('2023-06-05 14:43:45', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('487086064026013696', '单个1221', '83', TO_DATE('2023-06-06 10:26:08', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');
INSERT INTO DEMO_CRUD VALUES ('487086286626115584', '单个685', '84', TO_DATE('2023-06-06 10:27:01', 'SYYYY-MM-DD HH24:MI:SS'), '0', '0', '0');

-- ----------------------------
-- Table structure for demo_par_tbl
-- ----------------------------
CREATE TABLE DEMO_PAR_TBL (
  ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_par_tbl
-- ----------------------------
INSERT INTO DEMO_PAR_TBL VALUES ('443588385522601984', '91471c9846a44fe8a7fc4b76e9f702ea');
INSERT INTO DEMO_PAR_TBL VALUES ('443588583535693824', '新增');
INSERT INTO DEMO_PAR_TBL VALUES ('443588895352836096', '新增');
INSERT INTO DEMO_PAR_TBL VALUES ('443588932694724608', '新增');
INSERT INTO DEMO_PAR_TBL VALUES ('445140374589034496', '新增');
INSERT INTO DEMO_PAR_TBL VALUES ('446130095742013440', '新增');
INSERT INTO DEMO_PAR_TBL VALUES ('484622270804807680', '新增');
INSERT INTO DEMO_PAR_TBL VALUES ('484622408633831424', '新增');
INSERT INTO DEMO_PAR_TBL VALUES ('484623850568437760', '新增');
INSERT INTO DEMO_PAR_TBL VALUES ('484623946693496832', '新增');

-- ----------------------------
-- Table structure for demo_virtbl1
-- ----------------------------
CREATE TABLE DEMO_VIRTBL1 (
  ID NUMBER(19) NOT NULL,
  NAME1 VARCHAR2(255) NOT NULL
)
;
COMMENT ON COLUMN DEMO_VIRTBL1.NAME1 IS '名称1';

-- ----------------------------
-- Records of demo_virtbl1
-- ----------------------------
INSERT INTO DEMO_VIRTBL1 VALUES ('484613811564728320', '新1');
INSERT INTO DEMO_VIRTBL1 VALUES ('484613939734269952', '新1');
INSERT INTO DEMO_VIRTBL1 VALUES ('484614242416218112', '批增1');
INSERT INTO DEMO_VIRTBL1 VALUES ('484621407772233728', '新1');
INSERT INTO DEMO_VIRTBL1 VALUES ('484623466739290112', '新1');

-- ----------------------------
-- Table structure for demo_virtbl2
-- ----------------------------
CREATE TABLE DEMO_VIRTBL2 (
  ID NUMBER(19) NOT NULL,
  NAME2 VARCHAR2(255) NOT NULL
)
;
COMMENT ON COLUMN DEMO_VIRTBL2.NAME2 IS '名称2';

-- ----------------------------
-- Records of demo_virtbl2
-- ----------------------------
INSERT INTO DEMO_VIRTBL2 VALUES ('484613811564728320', '新2');
INSERT INTO DEMO_VIRTBL2 VALUES ('484613939734269952', '新2');
INSERT INTO DEMO_VIRTBL2 VALUES ('484614242416218112', '批增2');
INSERT INTO DEMO_VIRTBL2 VALUES ('484621407772233728', '新2');
INSERT INTO DEMO_VIRTBL2 VALUES ('484623466739290112', '新2');

-- ----------------------------
-- Table structure for demo_virtbl3
-- ----------------------------
CREATE TABLE DEMO_VIRTBL3 (
  ID NUMBER(19) NOT NULL,
  NAME3 VARCHAR2(255) NOT NULL
)
;
COMMENT ON COLUMN DEMO_VIRTBL3.NAME3 IS '名称3';

-- ----------------------------
-- Records of demo_virtbl3
-- ----------------------------
INSERT INTO DEMO_VIRTBL3 VALUES ('484613811564728320', '新3');
INSERT INTO DEMO_VIRTBL3 VALUES ('484613939734269952', '新3');
INSERT INTO DEMO_VIRTBL3 VALUES ('484614242416218112', '批增3');
INSERT INTO DEMO_VIRTBL3 VALUES ('484621407772233728', '新3');
INSERT INTO DEMO_VIRTBL3 VALUES ('484623466739290112', '新3');

-- ----------------------------
-- Table structure for demo_大儿
-- ----------------------------
CREATE TABLE DEMO_大儿 (
  ID NUMBER(19) NOT NULL,
  PARENT_ID NUMBER(19) NOT NULL,
  大儿名 VARCHAR2(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_大儿
-- ----------------------------
INSERT INTO DEMO_大儿 VALUES ('453807589999792128', '448686488403595264', '啊北侧');
INSERT INTO DEMO_大儿 VALUES ('453810847795400704', '453810798449414144', 'bd');
INSERT INTO DEMO_大儿 VALUES ('453811346175184896', '453810798449414144', 'asdf');
INSERT INTO DEMO_大儿 VALUES ('453811364621733888', '453810798449414144', 'bde');

-- ----------------------------
-- Table structure for demo_父表
-- ----------------------------
CREATE TABLE DEMO_父表 (
  ID NUMBER(19) NOT NULL,
  父名 VARCHAR2(255) NOT NULL
)
;

-- ----------------------------
-- Records of demo_父表
-- ----------------------------
INSERT INTO DEMO_父表 VALUES ('448686488403595264', '123');
INSERT INTO DEMO_父表 VALUES ('449120963746877440', '单位');
INSERT INTO DEMO_父表 VALUES ('453810798449414144', 'aaaa');

-- ----------------------------
-- Table structure for demo_基础
-- ----------------------------
CREATE TABLE DEMO_基础 (
  ID NUMBER(19) NOT NULL,
  序列 NUMBER(9) NOT NULL,
  限长4 VARCHAR2(16) ,
  不重复 VARCHAR2(64) ,
  禁止选中 CHAR(1) DEFAULT 0 NOT NULL,
  禁止保存 CHAR(1) DEFAULT 0 NOT NULL,
  禁止删除 CHAR(1) DEFAULT 0 NOT NULL,
  值变事件 VARCHAR2(64),
  创建时间 DATE NOT NULL,
  修改时间 DATE NOT NULL
)
;

ALTER TABLE DEMO_基础 ADD CHECK (禁止选中 in (0,1));
ALTER TABLE DEMO_基础 ADD CHECK (禁止保存 in (0,1));
ALTER TABLE DEMO_基础 ADD CHECK (禁止删除 in (0,1));

COMMENT ON COLUMN DEMO_基础.ID IS '标识';
COMMENT ON COLUMN DEMO_基础.序列 IS '序列自动赋值';
COMMENT ON COLUMN DEMO_基础.限长4 IS '限制最大长度4';
COMMENT ON COLUMN DEMO_基础.不重复 IS '列值无重复';
COMMENT ON COLUMN DEMO_基础.禁止选中 IS '#bool#始终为false';
COMMENT ON COLUMN DEMO_基础.禁止保存 IS '#bool#true时保存前校验不通过';
COMMENT ON COLUMN DEMO_基础.禁止删除 IS '#bool#true时删除前校验不通过';
COMMENT ON COLUMN DEMO_基础.值变事件 IS '每次值变化时触发领域事件';
COMMENT ON COLUMN DEMO_基础.创建时间 IS '初次创建时间';
COMMENT ON COLUMN DEMO_基础.修改时间 IS '最后修改时间';

-- ----------------------------
-- Records of demo_基础
-- ----------------------------
INSERT INTO DEMO_基础 VALUES ('1', '1', 'adb', 'ddd', '1', '1', '1', 'a', TO_DATE('2023-01-17 10:08:10', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2023-01-17 10:08:14', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO DEMO_基础 VALUES ('447570516976357376', '6', '11', 'dd', '0', '0', '1', 'snv111', TO_DATE('2023-02-17 09:25:27', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2023-02-17 09:25:27', 'SYYYY-MM-DD HH24:MI:SS'));

-- ----------------------------
-- Table structure for demo_角色
-- ----------------------------
CREATE TABLE DEMO_角色 (
  ID NUMBER(19) NOT NULL,
  角色名称 VARCHAR2(32) NOT NULL,
  角色描述 VARCHAR2(255)
)
;
COMMENT ON COLUMN DEMO_角色.ID IS '角色标识';
COMMENT ON COLUMN DEMO_角色.角色名称 IS '角色名称';
COMMENT ON COLUMN DEMO_角色.角色描述 IS '角色描述';
COMMENT ON TABLE DEMO_角色 IS '角色';

-- ----------------------------
-- Records of demo_角色
-- ----------------------------
INSERT INTO DEMO_角色 VALUES ('449487215124303872', 'xxx', 'df');
INSERT INTO DEMO_角色 VALUES ('449812931669938176', '管理员', '');
INSERT INTO DEMO_角色 VALUES ('449812975420723200', '维护1', '');
INSERT INTO DEMO_角色 VALUES ('449813053959065600', '维护2', '');

-- ----------------------------
-- Table structure for demo_角色权限
-- ----------------------------
CREATE TABLE DEMO_角色权限 (
  ROLE_ID NUMBER(19) NOT NULL,
  PRV_ID NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN DEMO_角色权限.ROLE_ID IS '角色标识';
COMMENT ON COLUMN DEMO_角色权限.PRV_ID IS '权限标识';
COMMENT ON TABLE DEMO_角色权限 IS '角色关联的权限';

-- ----------------------------
-- Records of demo_角色权限
-- ----------------------------
INSERT INTO DEMO_角色权限 VALUES ('449487215124303872', '449812884102336512');

-- ----------------------------
-- Table structure for demo_扩展1
-- ----------------------------
CREATE TABLE DEMO_扩展1 (
  ID NUMBER(19) NOT NULL,
  扩展1名称 VARCHAR2(255),
  禁止选中 CHAR(1) DEFAULT 0 NOT NULL,
  禁止保存 CHAR(1) DEFAULT 0 NOT NULL
)
;

ALTER TABLE DEMO_扩展1 ADD CHECK (禁止选中 in (0,1));
ALTER TABLE DEMO_扩展1 ADD CHECK (禁止保存 in (0,1));

COMMENT ON COLUMN DEMO_扩展1.ID IS '标识';
COMMENT ON COLUMN DEMO_扩展1.禁止选中 IS '#bool#始终为false';
COMMENT ON COLUMN DEMO_扩展1.禁止保存 IS '#bool#true时保存前校验不通过';

-- ----------------------------
-- Records of demo_扩展1
-- ----------------------------
INSERT INTO DEMO_扩展1 VALUES ('447555037331214336', 'a', '0', '0');
INSERT INTO DEMO_扩展1 VALUES ('447577275388416000', '221', '0', '0');
INSERT INTO DEMO_扩展1 VALUES ('447577372700463104', '', '0', '0');

-- ----------------------------
-- Table structure for demo_扩展2
-- ----------------------------
CREATE TABLE DEMO_扩展2 (
  ID NUMBER(19) NOT NULL,
  扩展2名称 VARCHAR2(255),
  禁止删除 CHAR(1) DEFAULT 0 NOT NULL,
  值变事件 VARCHAR2(255)
)
;

ALTER TABLE DEMO_扩展2 ADD CHECK (禁止删除 in (0,1));

COMMENT ON COLUMN DEMO_扩展2.ID IS '标识';
COMMENT ON COLUMN DEMO_扩展2.禁止删除 IS '#bool#true时删除前校验不通过';
COMMENT ON COLUMN DEMO_扩展2.值变事件 IS '每次值变化时触发领域事件';

-- ----------------------------
-- Records of demo_扩展2
-- ----------------------------
INSERT INTO DEMO_扩展2 VALUES ('447555037331214336', 'a', '0', '');
INSERT INTO DEMO_扩展2 VALUES ('447577275388416000', '', '0', '221');
INSERT INTO DEMO_扩展2 VALUES ('447577372700463104', '', '0', '');

-- ----------------------------
-- Table structure for demo_权限
-- ----------------------------
CREATE TABLE DEMO_权限 (
  ID NUMBER(19) NOT NULL,
  权限名称 VARCHAR2(255) NOT NULL
)
;
COMMENT ON COLUMN DEMO_权限.ID IS '权限名称';
COMMENT ON TABLE DEMO_权限 IS '权限';

-- ----------------------------
-- Records of demo_权限
-- ----------------------------
INSERT INTO DEMO_权限 VALUES ('449812852120768512', '删除');
INSERT INTO DEMO_权限 VALUES ('449812884102336512', '修改');

-- ----------------------------
-- Table structure for demo_收文
-- ----------------------------
CREATE TABLE DEMO_收文 (
  ID NUMBER(19) NOT NULL,
  来文单位 VARCHAR2(255),
  来文时间 DATE NOT NULL,
  密级 NUMBER(3) NOT NULL,
  文件标题 VARCHAR2(255),
  文件附件 VARCHAR2(512),
  市场部经理意见 VARCHAR2(255),
  综合部经理意见 VARCHAR2(255),
  收文完成时间 DATE
)
;
COMMENT ON COLUMN DEMO_收文.密级 IS '#密级#';

-- ----------------------------
-- Records of demo_收文
-- ----------------------------
INSERT INTO DEMO_收文 VALUES ('162025231350624256', '123', TO_DATE('2020-12-21', 'SYYYY-MM-DD HH24:MI:SS'), '0', 'a', '', '', '', TO_DATE('0001-01-01', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO DEMO_收文 VALUES ('162401333600448512', 'abc', TO_DATE('2020-12-22', 'SYYYY-MM-DD HH24:MI:SS'), '0', '关于新冠疫情的批示', '', '', '', TO_DATE('0001-01-01', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO DEMO_收文 VALUES ('457384396879581184', '', TO_DATE('2023-03-16', 'SYYYY-MM-DD HH24:MI:SS'), '0', '阿斯蒂芬', '', '', '', TO_DATE('0001-01-01', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO DEMO_收文 VALUES ('457388173615452160', '', TO_DATE('2023-03-16', 'SYYYY-MM-DD HH24:MI:SS'), '0', '疫情在', '', '', '', TO_DATE('0001-01-01', 'SYYYY-MM-DD HH24:MI:SS'));

-- ----------------------------
-- Table structure for demo_小儿
-- ----------------------------
CREATE TABLE DEMO_小儿 (
  ID NUMBER(19) NOT NULL,
  GROUP_ID NUMBER(19) NOT NULL,
  小儿名 VARCHAR2(255)
)
;

-- ----------------------------
-- Records of demo_小儿
-- ----------------------------
INSERT INTO DEMO_小儿 VALUES ('449113382156521472', '448686488403595264', 'wwww');
INSERT INTO DEMO_小儿 VALUES ('453810909078376448', '453810798449414144', '34');
INSERT INTO DEMO_小儿 VALUES ('453811464773324800', '453810798449414144', 'adgas');

-- ----------------------------
-- Table structure for demo_用户
-- ----------------------------
CREATE TABLE DEMO_用户 (
  ID NUMBER(19) NOT NULL,
  手机号 CHAR(11) NOT NULL,
  姓名 VARCHAR2(32),
  密码 CHAR(32)
)
;
COMMENT ON COLUMN DEMO_用户.ID IS '用户标识';
COMMENT ON COLUMN DEMO_用户.手机号 IS '手机号，唯一';
COMMENT ON COLUMN DEMO_用户.姓名 IS '姓名';
COMMENT ON COLUMN DEMO_用户.密码 IS '密码的md5';
COMMENT ON TABLE DEMO_用户 IS '系统用户';

-- ----------------------------
-- Records of demo_用户
-- ----------------------------
INSERT INTO DEMO_用户 VALUES ('449772627373871104', '13223333', '阿斯顿', '');
INSERT INTO DEMO_用户 VALUES ('453805638385946624', '111', '', '');
INSERT INTO DEMO_用户 VALUES ('453805654500462592', '222', '', '');

-- ----------------------------
-- Table structure for demo_用户角色
-- ----------------------------
CREATE TABLE DEMO_用户角色 (
  USER_ID NUMBER(19) NOT NULL,
  ROLE_ID NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN DEMO_用户角色.USER_ID IS '用户标识';
COMMENT ON COLUMN DEMO_用户角色.ROLE_ID IS '角色标识';
COMMENT ON TABLE DEMO_用户角色 IS '用户关联的角色';

-- ----------------------------
-- Records of demo_用户角色
-- ----------------------------
INSERT INTO DEMO_用户角色 VALUES ('449772627373871104', '449487215124303872');
INSERT INTO DEMO_用户角色 VALUES ('449772627373871104', '449812931669938176');

-- ----------------------------
-- Table structure for demo_主表
-- ----------------------------
CREATE TABLE DEMO_主表 (
  ID NUMBER(19) NOT NULL,
  主表名称 VARCHAR2(255),
  限长4 VARCHAR2(16),
  不重复 VARCHAR2(255)
)
;
COMMENT ON COLUMN DEMO_主表.限长4 IS '限制最大长度4';
COMMENT ON COLUMN DEMO_主表.不重复 IS '列值无重复';

-- ----------------------------
-- Records of demo_主表
-- ----------------------------
INSERT INTO DEMO_主表 VALUES ('447555037331214336', 'a', '', '');
INSERT INTO DEMO_主表 VALUES ('447577275388416000', '1', '222222', '121');
INSERT INTO DEMO_主表 VALUES ('447577372700463104', '', '', '1');

-- ----------------------------
-- Primary Key structure for table demo_cache_tbl1
-- ----------------------------
ALTER TABLE DEMO_CACHE_TBL1 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_child_tbl1
-- ----------------------------
ALTER TABLE DEMO_CHILD_TBL1 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_child_tbl2
-- ----------------------------
ALTER TABLE DEMO_CHILD_TBL2 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_crud
-- ----------------------------
ALTER TABLE DEMO_CRUD ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_par_tbl
-- ----------------------------
ALTER TABLE DEMO_PAR_TBL ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_virtbl1
-- ----------------------------
ALTER TABLE DEMO_VIRTBL1 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_virtbl2
-- ----------------------------
ALTER TABLE DEMO_VIRTBL2 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_virtbl3
-- ----------------------------
ALTER TABLE DEMO_VIRTBL3 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_大儿
-- ----------------------------
ALTER TABLE DEMO_大儿 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table demo_大儿
-- ----------------------------
CREATE INDEX IDX_大儿_PARENDID
  ON DEMO_大儿 (PARENT_ID ASC);

-- ----------------------------
-- Primary Key structure for table demo_父表
-- ----------------------------
ALTER TABLE DEMO_父表 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_基础
-- ----------------------------
ALTER TABLE DEMO_基础 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_角色
-- ----------------------------
ALTER TABLE DEMO_角色 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_角色权限
-- ----------------------------
ALTER TABLE DEMO_角色权限 ADD PRIMARY KEY (ROLE_ID, PRV_ID);

-- ----------------------------
-- Indexes structure for table demo_角色权限
-- ----------------------------
CREATE INDEX IDX_角色权限_PRVID
  ON DEMO_角色权限 (PRV_ID ASC);

-- ----------------------------
-- Primary Key structure for table demo_扩展1
-- ----------------------------
ALTER TABLE DEMO_扩展1 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_扩展2
-- ----------------------------
ALTER TABLE DEMO_扩展2 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_权限
-- ----------------------------
ALTER TABLE DEMO_权限 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_收文
-- ----------------------------
ALTER TABLE DEMO_收文 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_小儿
-- ----------------------------
ALTER TABLE DEMO_小儿 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table demo_小儿
-- ----------------------------
CREATE INDEX IDX_小儿_GROUPID
  ON DEMO_小儿 (GROUP_ID ASC);

-- ----------------------------
-- Primary Key structure for table demo_用户
-- ----------------------------
ALTER TABLE DEMO_用户 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table demo_用户角色
-- ----------------------------
ALTER TABLE DEMO_用户角色 ADD PRIMARY KEY (USER_ID, ROLE_ID);

-- ----------------------------
-- Indexes structure for table demo_用户角色
-- ----------------------------
CREATE INDEX IDX_用户角色_USERID
  ON DEMO_用户角色 (USER_ID ASC);
CREATE INDEX IDX_用户角色_ROLEID
  ON DEMO_用户角色 (ROLE_ID ASC);

-- ----------------------------
-- Primary Key structure for table demo_主表
-- ----------------------------
ALTER TABLE DEMO_主表 ADD PRIMARY KEY (ID);

-- ----------------------------
-- Foreign Keys structure for table demo_大儿
-- ----------------------------
ALTER TABLE "DEMO_大儿" ADD CONSTRAINT "DEMO_大儿_PARENDID1" FOREIGN KEY ("PARENT_ID") REFERENCES "DEMO_父表" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table demo_角色权限
-- ----------------------------
ALTER TABLE "DEMO_角色权限" ADD CONSTRAINT "DEMO_角色权限_IBFK_1" FOREIGN KEY ("PRV_ID") REFERENCES "DEMO_权限" ("ID") ON DELETE CASCADE;
ALTER TABLE "DEMO_角色权限" ADD CONSTRAINT "DEMO_角色权限_IBFK_2" FOREIGN KEY ("ROLE_ID") REFERENCES "DEMO_角色" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table demo_小儿
-- ----------------------------
ALTER TABLE "DEMO_小儿" ADD CONSTRAINT "DEMO_小儿_PARENTID1" FOREIGN KEY ("GROUP_ID") REFERENCES "DEMO_父表" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table demo_用户角色
-- ----------------------------
ALTER TABLE "DEMO_用户角色" ADD CONSTRAINT "DEMO_用户角色_IBFK_1" FOREIGN KEY ("ROLE_ID") REFERENCES "DEMO_角色" ("ID") ON DELETE CASCADE;
ALTER TABLE "DEMO_用户角色" ADD CONSTRAINT "DEMO_用户角色_IBFK_2" FOREIGN KEY ("USER_ID") REFERENCES "DEMO_用户" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- 序列
-- ----------------------------
DROP SEQUENCE DEMO_CRUD_DISPIDX;
DROP SEQUENCE DEMO_基础_序列;
CREATE SEQUENCE DEMO_CRUD_DISPIDX START WITH 86;
CREATE SEQUENCE DEMO_基础_序列 START WITH 12;

-- ----------------------------
-- View structure for DEMO_CHILD_VIEW
-- ----------------------------
DROP VIEW "DEMO_CHILD_VIEW";
CREATE VIEW "DEMO_CHILD_VIEW" AS SELECT
	c.ID,c.PARENT_ID,c.ITEM_NAME, 
	p.NAME
FROM
	DEMO_CHILD_TBL1 c JOIN
	DEMO_PAR_TBL p on c.PARENT_ID = p.ID;

-- ----------------------------
-- Function structure for DEMO_用户可访问的菜单
-- ----------------------------
DROP PROCEDURE "DEMO_用户可访问的菜单";
CREATE OR REPLACE PROCEDURE "DEMO_用户可访问的菜单" (p_cur out sys_refcursor, p_userid in number)
AS
BEGIN

	open p_cur for
	select id,name
  from (select distinct (b.id), b.name, dispidx
          from cm_role_menu a
          left join cm_menu b
            on a.menu_id = b.id
         where exists
         (select role_id
                  from cm_user_role c
                 where a.role_id = c.role_id
                   and user_id = p_userid
					union
					select role_id
					        from cm_group_role d
									where a.role_id = d.role_id
									  and exists (select group_id from cm_user_group e where d.group_id=e.group_id and e.user_id=p_userid)
					) or a.role_id=1
			 ) t
 order by dispidx
 
END;