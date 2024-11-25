-- ----------------------------
-- 按照依赖顺序删除demo库对象
-- ----------------------------
DROP VIEW v_物资目录;
DROP VIEW v_人员;
DROP VIEW v_部门;
DROP VIEW v_物资主单;
DROP VIEW v_物资详单;

DROP TABLE crud_基础;
DROP TABLE crud_扩展1;
DROP TABLE crud_扩展2;
DROP TABLE crud_主表;
DROP TABLE crud_大儿;
DROP TABLE crud_小儿;
DROP TABLE crud_父表;
DROP TABLE crud_缓存表;
DROP TABLE crud_角色权限;
DROP TABLE crud_用户角色;
DROP TABLE crud_用户;
DROP TABLE crud_角色;
DROP TABLE crud_权限;
DROP TABLE crud_字段类型;

DROP TABLE 物资详单;
DROP TABLE 物资主单;
DROP TABLE 物资计划明细;
DROP TABLE 物资计划;
DROP TABLE 物资库存;
DROP TABLE 物资目录;
DROP TABLE 物资分类;
DROP TABLE 物资入出类别;
DROP TABLE 供应商;
DROP TABLE 部门人员;
DROP TABLE 部门;
DROP TABLE 人员;

DROP SEQUENCE CRUD_基础_序列;
DROP SEQUENCE 物资主单_单号;
DROP SEQUENCE 物资入出类别_ID;

-- ----------------------------
-- 按照依赖顺序删除dt初始库对象
-- ----------------------------
DROP TABLE cm_cache;
DROP TABLE cm_wfi_trs;
DROP TABLE cm_wfi_item;
DROP TABLE cm_wfi_atv;
DROP TABLE cm_wfi_prc;
DROP TABLE cm_wfd_trs;
DROP TABLE cm_wfd_atv_role;
DROP TABLE cm_wfd_atv;
DROP TABLE cm_wfd_prc;
DROP TABLE cm_user_group;
DROP TABLE cm_user_params;
DROP TABLE cm_user_role;
DROP TABLE cm_group_role;
DROP TABLE cm_role_menu;
DROP TABLE cm_role_per;
DROP TABLE cm_group;
DROP TABLE cm_menu;
DROP TABLE cm_option;
DROP TABLE cm_option_group;
DROP TABLE cm_params;
DROP TABLE cm_permission;
DROP TABLE cm_permission_func;
DROP TABLE cm_permission_module;
DROP TABLE cm_role;
DROP TABLE cm_rpt;
DROP TABLE cm_file_pub;
DROP TABLE cm_file_my;
DROP TABLE cm_user;
DROP TABLE fsm_file;

DROP SEQUENCE CM_MENU_DISPIDX;
DROP SEQUENCE CM_OPTION_DISPIDX;
DROP SEQUENCE CM_WFD_PRC_DISPIDX;
DROP SEQUENCE CM_WFI_ITEM_DISPIDX;
DROP SEQUENCE CM_WFI_PRC_DISPIDX;


-- ----------------------------
-- Table structure for CM_CACHE
-- ----------------------------
CREATE TABLE CM_CACHE (
  ID VARCHAR2(255) NOT NULL,
  VAL VARCHAR2(512) NOT NULL
)
;

COMMENT ON TABLE CM_CACHE IS '模拟redis缓存key value数据，直连数据库时用';

-- ----------------------------
-- Table structure for cm_file_my
-- ----------------------------
CREATE TABLE CM_FILE_MY (
  ID NUMBER(19) NOT NULL,
  PARENT_ID NUMBER(19),
  NAME VARCHAR2(255) NOT NULL,
  IS_FOLDER CHAR(1) DEFAULT 0 NOT NULL,
  EXT_NAME VARCHAR2(8),
  INFO VARCHAR2(512),
  CTIME DATE NOT NULL,
  USER_ID NUMBER(19) NOT NULL
)
;

ALTER TABLE CM_FILE_MY ADD CHECK (IS_FOLDER in (0,1));

COMMENT ON COLUMN CM_FILE_MY.ID IS '文件标识';
COMMENT ON COLUMN CM_FILE_MY.PARENT_ID IS '上级目录，根目录的parendid为空';
COMMENT ON COLUMN CM_FILE_MY.NAME IS '名称';
COMMENT ON COLUMN CM_FILE_MY.IS_FOLDER IS '#bool#是否为文件夹';
COMMENT ON COLUMN CM_FILE_MY.EXT_NAME IS '文件扩展名';
COMMENT ON COLUMN CM_FILE_MY.INFO IS '文件描述信息';
COMMENT ON COLUMN CM_FILE_MY.CTIME IS '创建时间';
COMMENT ON COLUMN CM_FILE_MY.USER_ID IS '所属用户';
COMMENT ON TABLE CM_FILE_MY IS '个人文件';

-- ----------------------------
-- Table structure for cm_file_pub
-- ----------------------------
CREATE TABLE CM_FILE_PUB (
  ID NUMBER(19) NOT NULL,
  PARENT_ID NUMBER(19),
  NAME VARCHAR2(255) NOT NULL,
  IS_FOLDER CHAR(1) DEFAULT 0 NOT NULL,
  EXT_NAME VARCHAR2(8),
  INFO VARCHAR2(512),
  CTIME DATE NOT NULL
)
;

ALTER TABLE CM_FILE_PUB ADD CHECK (IS_FOLDER in (0,1));

COMMENT ON COLUMN CM_FILE_PUB.ID IS '文件标识';
COMMENT ON COLUMN CM_FILE_PUB.PARENT_ID IS '上级目录，根目录的parendid为空';
COMMENT ON COLUMN CM_FILE_PUB.NAME IS '名称';
COMMENT ON COLUMN CM_FILE_PUB.IS_FOLDER IS '#bool#是否为文件夹';
COMMENT ON COLUMN CM_FILE_PUB.EXT_NAME IS '文件扩展名';
COMMENT ON COLUMN CM_FILE_PUB.INFO IS '文件描述信息';
COMMENT ON COLUMN CM_FILE_PUB.CTIME IS '创建时间';
COMMENT ON TABLE CM_FILE_PUB IS '公共文件';

-- ----------------------------
-- Records of cm_file_pub
-- ----------------------------
INSERT INTO CM_FILE_PUB VALUES ('1', NULL, '公共文件', '1', NULL, '', TO_DATE('2020-10-21 15:19:20', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_FILE_PUB VALUES ('2', NULL, '素材库', '1', NULL, '', TO_DATE('2020-10-21 15:20:21', 'SYYYY-MM-DD HH24:MI:SS'));

-- ----------------------------
-- Table structure for cm_group
-- ----------------------------
CREATE TABLE CM_GROUP (
  ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(64) NOT NULL,
  NOTE VARCHAR2(255)
)
;
COMMENT ON COLUMN CM_GROUP.ID IS '组标识';
COMMENT ON COLUMN CM_GROUP.NAME IS '组名';
COMMENT ON COLUMN CM_GROUP.NOTE IS '组描述';
COMMENT ON TABLE CM_GROUP IS '用户组，与用户和角色多对多';

-- ----------------------------
-- Table structure for cm_group_role
-- ----------------------------
CREATE TABLE CM_GROUP_ROLE (
  GROUP_ID NUMBER(19) NOT NULL,
  ROLE_ID NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN CM_GROUP_ROLE.GROUP_ID IS '组标识';
COMMENT ON COLUMN CM_GROUP_ROLE.ROLE_ID IS '角色标识';
COMMENT ON TABLE CM_GROUP_ROLE IS '组一角色多对多';

-- ----------------------------
-- Table structure for cm_menu
-- ----------------------------
CREATE TABLE CM_MENU (
  ID NUMBER(19) NOT NULL,
  PARENT_ID NUMBER(19),
  NAME VARCHAR2(64) NOT NULL,
  IS_GROUP CHAR(1) DEFAULT 0 NOT NULL,
  VIEW_NAME VARCHAR2(128),
  PARAMS VARCHAR2(4000),
  ICON VARCHAR2(128),
  NOTE VARCHAR2(512),
  DISPIDX NUMBER(9) NOT NULL,
  IS_LOCKED CHAR(1) DEFAULT 0 NOT NULL,
  CTIME DATE NOT NULL,
  MTIME DATE NOT NULL
)
;

ALTER TABLE CM_MENU ADD CHECK (IS_GROUP in (0,1));
ALTER TABLE CM_MENU ADD CHECK (IS_LOCKED in (0,1));

COMMENT ON COLUMN CM_MENU.ID IS '菜单标识';
COMMENT ON COLUMN CM_MENU.PARENT_ID IS '父菜单标识';
COMMENT ON COLUMN CM_MENU.NAME IS '菜单名称';
COMMENT ON COLUMN CM_MENU.IS_GROUP IS '#bool#分组或实例。0表实例，1表分组';
COMMENT ON COLUMN CM_MENU.VIEW_NAME IS '视图名称';
COMMENT ON COLUMN CM_MENU.PARAMS IS '传递给菜单程序的参数';
COMMENT ON COLUMN CM_MENU.ICON IS '图标';
COMMENT ON COLUMN CM_MENU.NOTE IS '备注';
COMMENT ON COLUMN CM_MENU.DISPIDX IS '显示顺序';
COMMENT ON COLUMN CM_MENU.IS_LOCKED IS '#bool#定义了菜单是否被锁定。0表未锁定，1表锁定不可用';
COMMENT ON COLUMN CM_MENU.CTIME IS '创建时间';
COMMENT ON COLUMN CM_MENU.MTIME IS '最后修改时间';
COMMENT ON TABLE CM_MENU IS '业务菜单';

-- ----------------------------
-- Records of cm_menu
-- ----------------------------
INSERT INTO CM_MENU VALUES ('93146668397260800', NULL, '基础维护', '1', NULL, NULL, NULL, NULL, '1', '0', TO_DATE('2024-06-14 08:51:37', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-14 08:51:37', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('93147399237955584', '93146668397260800', '部门管理', '0', '部门管理', NULL, '多人', NULL, '115', '0', TO_DATE('2024-06-14 08:54:32', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-14 08:54:32', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('93147789455028224', '93146668397260800', '人员管理', '0', '人员管理', NULL, '个人信息', NULL, '117', '0', TO_DATE('2024-06-14 08:56:05', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-14 08:56:05', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('95003376719523840', '93146668397260800', '供应商管理', '0', '供应商管理', NULL, '全局', NULL, '119', '0', TO_DATE('2024-06-19 11:49:30', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-19 11:49:30', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('96885816660619264', NULL, '物资管理', '1', NULL, NULL, NULL, NULL, '122', '0', TO_DATE('2024-06-24 16:29:45', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-24 16:29:45', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('1', NULL, '工作台', '1', '', NULL, '搬运工', NULL, '123', '0', TO_DATE('2019-03-07 10:45:44', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-03-07 10:45:43', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('97869834403213312', NULL, '测试组', '1', NULL, NULL, NULL, NULL, '130', '0', TO_DATE('2024-06-27 09:39:50', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-27 09:39:50', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('97869954830069760', '97869834403213312', '一级菜单1', '0', NULL, NULL, '文件', NULL, '131', '0', TO_DATE('2024-06-27 09:40:18', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-27 09:40:18', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('97870059381485568', '97869834403213312', '一级菜单2', '0', NULL, NULL, '文件', NULL, '132', '0', TO_DATE('2024-06-27 09:40:43', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-27 09:40:43', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('97870113269903360', '97869834403213312', '二级组', '1', NULL, NULL, NULL, NULL, '133', '0', TO_DATE('2024-06-27 09:40:56', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-27 09:40:56', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('97870286377218048', '97870113269903360', '二级菜单1', '0', NULL, NULL, '文件', NULL, '134', '0', TO_DATE('2024-06-27 09:41:37', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-27 09:41:37', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('97870350000615424', '97870113269903360', '二级菜单2', '0', NULL, NULL, '文件', NULL, '135', '0', TO_DATE('2024-06-27 09:41:52', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-27 09:41:52', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('97871217135218688', '97870113269903360', '三级组', '1', NULL, NULL, NULL, NULL, '136', '0', TO_DATE('2024-06-27 09:45:19', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-27 09:45:19', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('97871290111913984', '97871217135218688', '三级菜单', '0', NULL, NULL, '文件', NULL, '137', '0', TO_DATE('2024-06-27 09:45:37', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-27 09:45:37', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('105150016726003712', '93146668397260800', '物资入出类别', '0', '物资入出类别', NULL, '分组', NULL, '138', '0', TO_DATE('2024-07-17 11:48:40', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-07-17 11:48:40', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('95004558183657472', '93146668397260800', '物资目录管理', '0', '物资目录管理', NULL, '树形', NULL, '121', '0', TO_DATE('2024-06-19 11:54:11', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-19 11:54:11', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('3', '1', '用户组', '0', '用户组', NULL, '分组', '管理用户组、组内用户，为用户组分配角色', '3', '0', TO_DATE('2023-03-10 08:34:49', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2023-03-10 08:34:49', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('5', '1', '基础权限', '0', '基础权限', NULL, '审核', '按照模块和功能两级目录管理权限、将权限分配给角色', '5', '0', TO_DATE('2019-03-12 09:11:22', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-03-07 11:23:40', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('6', '1', '菜单管理', '0', '菜单管理', NULL, '大图标', '菜单和菜单组管理、将菜单授权给角色', '6', '0', TO_DATE('2019-03-11 11:35:59', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-03-11 11:35:58', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('96886018188537856', '96885816660619264', '物资入出管理', '0', '物资入出', NULL, '四面体', NULL, '124', '0', TO_DATE('2024-06-24 16:30:33', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-24 16:30:33', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('96889439553613824', '96885816660619264', '物资盘存管理', '0', '物资盘存', NULL, '文件', NULL, '128', '0', TO_DATE('2024-06-24 16:44:09', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-24 16:44:09', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('96889910070636544', '96885816660619264', '物资计划管理', '0', '物资计划', NULL, '外设', NULL, '129', '0', TO_DATE('2024-06-24 16:46:01', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2024-06-24 16:46:01', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('7', '1', '报表设计', '0', '报表设计', NULL, '折线图', '报表管理及报表模板设计', '7', '0', TO_DATE('2020-10-19 11:21:38', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2020-10-19 11:21:38', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('2', '1', '用户账号', '0', '用户账号', NULL, '钥匙', '用户账号及所属用户组管理、为用户分配角色、查看用户可访问菜单和已授权限', '2', '0', TO_DATE('2019-11-08 11:42:28', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-11-08 11:43:53', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('9', '1', '参数定义', '0', '参数定义', NULL, '调色板', '参数名称、默认值的定义管理', '9', '0', TO_DATE('2019-03-12 15:35:56', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-03-12 15:37:10', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('4', '1', '系统角色', '0', '系统角色', NULL, '两人', '角色管理、为用户和用户组分配角色、设置角色可访问菜单、授予权限', '4', '0', TO_DATE('2019-11-08 11:47:21', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-11-08 11:48:22', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('10', '1', '基础选项', '0', '基础选项', NULL, '修理', '按照分组管理的选项列表，如民族、学历等静态列表', '10', '0', TO_DATE('2019-11-08 11:49:40', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-11-08 11:49:46', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('8', '1', '流程设计', '0', '流程设计', NULL, '双绞线', '流程模板设计及流程实例查询', '8', '0', TO_DATE('2020-11-02 16:21:19', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2020-11-02 16:21:19', 'SYYYY-MM-DD HH24:MI:SS'));

-- ----------------------------
-- Table structure for cm_option
-- ----------------------------
CREATE TABLE CM_OPTION (
  ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(64) NOT NULL,
  DISPIDX NUMBER(9) NOT NULL,
  GROUP_ID NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN CM_OPTION.ID IS '标识';
COMMENT ON COLUMN CM_OPTION.NAME IS '选项名称';
COMMENT ON COLUMN CM_OPTION.DISPIDX IS '显示顺序';
COMMENT ON COLUMN CM_OPTION.GROUP_ID IS '所属分组';
COMMENT ON TABLE CM_OPTION IS '基础选项';

-- ----------------------------
-- Records of cm_option
-- ----------------------------
INSERT INTO CM_OPTION VALUES ('2', '汉族', '2', '1');
INSERT INTO CM_OPTION VALUES ('3', '蒙古族', '3', '1');
INSERT INTO CM_OPTION VALUES ('4', '回族', '4', '1');
INSERT INTO CM_OPTION VALUES ('5', '藏族', '5', '1');
INSERT INTO CM_OPTION VALUES ('6', '维吾尔族', '6', '1');
INSERT INTO CM_OPTION VALUES ('7', '苗族', '7', '1');
INSERT INTO CM_OPTION VALUES ('8', '彝族', '8', '1');
INSERT INTO CM_OPTION VALUES ('9', '壮族', '9', '1');
INSERT INTO CM_OPTION VALUES ('10', '布依族', '10', '1');
INSERT INTO CM_OPTION VALUES ('11', '朝鲜族', '11', '1');
INSERT INTO CM_OPTION VALUES ('12', '满族', '12', '1');
INSERT INTO CM_OPTION VALUES ('13', '侗族', '13', '1');
INSERT INTO CM_OPTION VALUES ('14', '瑶族', '14', '1');
INSERT INTO CM_OPTION VALUES ('15', '白族', '15', '1');
INSERT INTO CM_OPTION VALUES ('16', '土家族', '16', '1');
INSERT INTO CM_OPTION VALUES ('17', '哈尼族', '17', '1');
INSERT INTO CM_OPTION VALUES ('18', '哈萨克族', '18', '1');
INSERT INTO CM_OPTION VALUES ('19', '傣族', '19', '1');
INSERT INTO CM_OPTION VALUES ('20', '黎族', '20', '1');
INSERT INTO CM_OPTION VALUES ('21', '傈僳族', '21', '1');
INSERT INTO CM_OPTION VALUES ('22', '佤族', '22', '1');
INSERT INTO CM_OPTION VALUES ('23', '畲族', '23', '1');
INSERT INTO CM_OPTION VALUES ('24', '高山族', '24', '1');
INSERT INTO CM_OPTION VALUES ('25', '拉祜族', '25', '1');
INSERT INTO CM_OPTION VALUES ('26', '水族', '26', '1');
INSERT INTO CM_OPTION VALUES ('27', '东乡族', '27', '1');
INSERT INTO CM_OPTION VALUES ('28', '纳西族', '28', '1');
INSERT INTO CM_OPTION VALUES ('29', '景颇族', '29', '1');
INSERT INTO CM_OPTION VALUES ('30', '柯尔克孜族', '30', '1');
INSERT INTO CM_OPTION VALUES ('31', '土族', '31', '1');
INSERT INTO CM_OPTION VALUES ('32', '达斡尔族', '32', '1');
INSERT INTO CM_OPTION VALUES ('33', '仫佬族', '33', '1');
INSERT INTO CM_OPTION VALUES ('34', '羌族', '34', '1');
INSERT INTO CM_OPTION VALUES ('35', '布朗族', '35', '1');
INSERT INTO CM_OPTION VALUES ('36', '撒拉族', '36', '1');
INSERT INTO CM_OPTION VALUES ('37', '毛难族', '37', '1');
INSERT INTO CM_OPTION VALUES ('38', '仡佬族', '38', '1');
INSERT INTO CM_OPTION VALUES ('39', '锡伯族', '39', '1');
INSERT INTO CM_OPTION VALUES ('40', '阿昌族', '40', '1');
INSERT INTO CM_OPTION VALUES ('41', '普米族', '41', '1');
INSERT INTO CM_OPTION VALUES ('42', '塔吉克族', '42', '1');
INSERT INTO CM_OPTION VALUES ('43', '怒族', '43', '1');
INSERT INTO CM_OPTION VALUES ('44', '乌孜别克族', '44', '1');
INSERT INTO CM_OPTION VALUES ('45', '俄罗斯族', '45', '1');
INSERT INTO CM_OPTION VALUES ('46', '鄂温克族', '46', '1');
INSERT INTO CM_OPTION VALUES ('47', '德昂族', '47', '1');
INSERT INTO CM_OPTION VALUES ('48', '保安族', '48', '1');
INSERT INTO CM_OPTION VALUES ('49', '裕固族', '49', '1');
INSERT INTO CM_OPTION VALUES ('50', '京族', '50', '1');
INSERT INTO CM_OPTION VALUES ('51', '塔塔尔族', '51', '1');
INSERT INTO CM_OPTION VALUES ('52', '独龙族', '52', '1');
INSERT INTO CM_OPTION VALUES ('53', '鄂伦春族', '53', '1');
INSERT INTO CM_OPTION VALUES ('54', '赫哲族', '54', '1');
INSERT INTO CM_OPTION VALUES ('55', '门巴族', '55', '1');
INSERT INTO CM_OPTION VALUES ('56', '珞巴族', '56', '1');
INSERT INTO CM_OPTION VALUES ('57', '基诺族', '57', '1');
INSERT INTO CM_OPTION VALUES ('58', '大学', '58', '2');
INSERT INTO CM_OPTION VALUES ('59', '高中', '59', '2');
INSERT INTO CM_OPTION VALUES ('60', '中学', '60', '2');
INSERT INTO CM_OPTION VALUES ('61', '小学', '61', '2');
INSERT INTO CM_OPTION VALUES ('62', '硕士', '62', '2');
INSERT INTO CM_OPTION VALUES ('63', '博士', '63', '2');
INSERT INTO CM_OPTION VALUES ('64', '其他', '64', '2');
INSERT INTO CM_OPTION VALUES ('342', '男', '342', '4');
INSERT INTO CM_OPTION VALUES ('343', '女', '343', '4');
INSERT INTO CM_OPTION VALUES ('344', '未知', '344', '4');
INSERT INTO CM_OPTION VALUES ('345', '不明', '345', '4');
INSERT INTO CM_OPTION VALUES ('346', 'string', '346', '5');
INSERT INTO CM_OPTION VALUES ('347', 'int', '347', '5');
INSERT INTO CM_OPTION VALUES ('348', 'double', '348', '5');
INSERT INTO CM_OPTION VALUES ('349', 'DateTime', '349', '5');
INSERT INTO CM_OPTION VALUES ('350', 'Date', '350', '5');
INSERT INTO CM_OPTION VALUES ('351', 'bool', '351', '5');

-- ----------------------------
-- Table structure for cm_option_group
-- ----------------------------
CREATE TABLE CM_OPTION_GROUP (
  ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(255) NOT NULL
)
;
COMMENT ON COLUMN CM_OPTION_GROUP.ID IS '标识';
COMMENT ON COLUMN CM_OPTION_GROUP.NAME IS '分组名称';
COMMENT ON TABLE CM_OPTION_GROUP IS '基础选项分组';

-- ----------------------------
-- Records of cm_option_group
-- ----------------------------
INSERT INTO CM_OPTION_GROUP VALUES ('1', '民族');
INSERT INTO CM_OPTION_GROUP VALUES ('2', '学历');
INSERT INTO CM_OPTION_GROUP VALUES ('3', '地区');
INSERT INTO CM_OPTION_GROUP VALUES ('4', '性别');
INSERT INTO CM_OPTION_GROUP VALUES ('5', '数据类型');

-- ----------------------------
-- Table structure for cm_params
-- ----------------------------
CREATE TABLE CM_PARAMS (
  ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(255) NOT NULL,
  VALUE VARCHAR2(255),
  NOTE VARCHAR2(255),
  CTIME DATE NOT NULL,
  MTIME DATE NOT NULL
)
;
COMMENT ON COLUMN CM_PARAMS.ID IS '用户参数标识';
COMMENT ON COLUMN CM_PARAMS.NAME IS '参数名称';
COMMENT ON COLUMN CM_PARAMS.VALUE IS '参数缺省值';
COMMENT ON COLUMN CM_PARAMS.NOTE IS '参数描述';
COMMENT ON COLUMN CM_PARAMS.CTIME IS '创建时间';
COMMENT ON COLUMN CM_PARAMS.MTIME IS '修改时间';
COMMENT ON TABLE CM_PARAMS IS '用户参数定义';

-- ----------------------------
-- Records of cm_params
-- ----------------------------
INSERT INTO CM_PARAMS VALUES ('1', '接收新任务', 'true', '', TO_DATE('2020-12-01 15:13:49', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2020-12-02 09:23:53', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_PARAMS VALUES ('2', '接收新发布通知', 'true', '', TO_DATE('2020-12-02 09:25:15', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2020-12-02 09:25:15', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_PARAMS VALUES ('3', '接收新消息', 'true', '接收通讯录消息推送', TO_DATE('2020-12-02 09:24:28', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2020-12-02 09:24:28', 'SYYYY-MM-DD HH24:MI:SS'));

-- ----------------------------
-- Table structure for cm_permission_module
-- ----------------------------
CREATE TABLE CM_PERMISSION_MODULE (
  ID NUMBER(19) NOT NULL,
  NAME NVARCHAR2(64) NOT NULL,
  NOTE NVARCHAR2(255)
)
;
COMMENT ON COLUMN CM_PERMISSION_MODULE.ID IS '模块标识';
COMMENT ON COLUMN CM_PERMISSION_MODULE.NAME IS '模块名称';
COMMENT ON COLUMN CM_PERMISSION_MODULE.NOTE IS '模块描述';
COMMENT ON TABLE CM_PERMISSION_MODULE IS '权限所属模块';

-- ----------------------------
-- Records of cm_permission_module
-- ----------------------------
INSERT INTO CM_PERMISSION_MODULE VALUES ('1', '系统预留', '系统内部使用的权限控制，禁止删除');
INSERT INTO CM_PERMISSION_MODULE VALUES ('87433840629673984', '物资管理', NULL);

-- ----------------------------
-- Table structure for cm_permission_func
-- ----------------------------
CREATE TABLE CM_PERMISSION_FUNC (
  ID NUMBER(19) NOT NULL,
  MODULE_ID NUMBER(19) NOT NULL,
  NAME NVARCHAR2(64) NOT NULL,
  NOTE NVARCHAR2(255)
)
;
COMMENT ON COLUMN CM_PERMISSION_FUNC.MODULE_ID IS '所属模块';
COMMENT ON COLUMN CM_PERMISSION_FUNC.NAME IS '功能名称';
COMMENT ON COLUMN CM_PERMISSION_FUNC.NOTE IS '功能描述';
COMMENT ON TABLE CM_PERMISSION_FUNC IS '权限所属功能';

-- ----------------------------
-- Records of cm_permission_func
-- ----------------------------
INSERT INTO CM_PERMISSION_FUNC VALUES ('1', '1', '文件管理', '管理文件的上传、删除等');
INSERT INTO CM_PERMISSION_FUNC VALUES ('87433900117487616', '87433840629673984', '入出', NULL);

-- ----------------------------
-- Table structure for cm_permission
-- ----------------------------
CREATE TABLE CM_PERMISSION (
  ID NUMBER(19) NOT NULL,
  FUNC_ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(64) NOT NULL,
  NOTE VARCHAR2(255)
)
;
COMMENT ON COLUMN CM_PERMISSION.ID IS '权限标识';
COMMENT ON COLUMN CM_PERMISSION.FUNC_ID IS '所属功能';
COMMENT ON COLUMN CM_PERMISSION.NAME IS '权限名称';
COMMENT ON COLUMN CM_PERMISSION.NOTE IS '权限描述';
COMMENT ON TABLE CM_PERMISSION IS '权限';

-- ----------------------------
-- Records of cm_permission
-- ----------------------------
INSERT INTO CM_PERMISSION VALUES ('1', '1', '公共文件增删', '公共文件的上传、删除等');
INSERT INTO CM_PERMISSION VALUES ('2', '1', '素材库增删', '素材库目录的上传、删除等');
INSERT INTO CM_PERMISSION VALUES ('87434002596917248', '87433900117487616', '冲销', NULL);

-- ----------------------------
-- Table structure for cm_role
-- ----------------------------
CREATE TABLE CM_ROLE (
  ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(32) NOT NULL,
  NOTE VARCHAR2(255)
)
;
COMMENT ON COLUMN CM_ROLE.ID IS '角色标识';
COMMENT ON COLUMN CM_ROLE.NAME IS '角色名称';
COMMENT ON COLUMN CM_ROLE.NOTE IS '角色描述';
COMMENT ON TABLE CM_ROLE IS '角色';

-- ----------------------------
-- Records of cm_role
-- ----------------------------
INSERT INTO CM_ROLE VALUES ('1', '任何人', '所有用户默认都具有该角色，不可删除');
INSERT INTO CM_ROLE VALUES ('2', '系统管理员', '系统角色，不可删除');
INSERT INTO CM_ROLE VALUES ('87363447483035648', '库管员', NULL);
INSERT INTO CM_ROLE VALUES ('87368228331089920', '库主管', NULL);

-- ----------------------------
-- Table structure for cm_role_menu
-- ----------------------------
CREATE TABLE CM_ROLE_MENU (
  ROLE_ID NUMBER(19) NOT NULL,
  MENU_ID NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN CM_ROLE_MENU.ROLE_ID IS '角色标识';
COMMENT ON COLUMN CM_ROLE_MENU.MENU_ID IS '菜单标识';
COMMENT ON TABLE CM_ROLE_MENU IS '角色一菜单多对多';

-- ----------------------------
-- Records of cm_role_menu
-- ----------------------------
INSERT INTO CM_ROLE_MENU VALUES ('2', '2');
INSERT INTO CM_ROLE_MENU VALUES ('2', '3');
INSERT INTO CM_ROLE_MENU VALUES ('2', '4');
INSERT INTO CM_ROLE_MENU VALUES ('2', '5');
INSERT INTO CM_ROLE_MENU VALUES ('2', '6');
INSERT INTO CM_ROLE_MENU VALUES ('2', '7');
INSERT INTO CM_ROLE_MENU VALUES ('2', '8');
INSERT INTO CM_ROLE_MENU VALUES ('2', '9');
INSERT INTO CM_ROLE_MENU VALUES ('2', '10');

-- ----------------------------
-- Table structure for cm_role_per
-- ----------------------------
CREATE TABLE CM_ROLE_PER (
  ROLE_ID NUMBER(19) NOT NULL,
  PER_ID NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN CM_ROLE_PER.ROLE_ID IS '角色标识';
COMMENT ON COLUMN CM_ROLE_PER.PER_ID IS '权限标识';
COMMENT ON TABLE CM_ROLE_PER IS '角色一权限多对多';

-- ----------------------------
-- Records of cm_role_per
-- ----------------------------
INSERT INTO CM_ROLE_PER VALUES ('2', '1');
INSERT INTO CM_ROLE_PER VALUES ('2', '2');

-- ----------------------------
-- Table structure for cm_rpt
-- ----------------------------
CREATE TABLE CM_RPT (
  ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(64) NOT NULL,
  DEFINE CLOB,
  NOTE VARCHAR2(255),
  CTIME DATE NOT NULL,
  MTIME DATE NOT NULL
)
;
COMMENT ON COLUMN CM_RPT.ID IS '报表标识';
COMMENT ON COLUMN CM_RPT.NAME IS '报表名称';
COMMENT ON COLUMN CM_RPT.DEFINE IS '报表模板定义';
COMMENT ON COLUMN CM_RPT.NOTE IS '报表描述';
COMMENT ON COLUMN CM_RPT.CTIME IS '创建时间';
COMMENT ON COLUMN CM_RPT.MTIME IS '修改时间';
COMMENT ON TABLE CM_RPT IS '报表模板定义';

-- ----------------------------
-- Table structure for cm_user
-- ----------------------------
CREATE TABLE CM_USER (
  ID NUMBER(19) NOT NULL,
  ACC VARCHAR2(32),
  PHONE VARCHAR2(16),
  PWD CHAR(32) NOT NULL,
  NAME VARCHAR2(32),
  PHOTO VARCHAR2(255),
  EXPIRED CHAR(1) DEFAULT 0 NOT NULL,
  CTIME DATE NOT NULL,
  MTIME DATE NOT NULL
)
;

ALTER TABLE CM_USER ADD CHECK (EXPIRED in (0,1));

COMMENT ON COLUMN CM_USER.ID IS '用户标识';
COMMENT ON COLUMN CM_USER.ACC IS '账号，唯一';
COMMENT ON COLUMN CM_USER.PHONE IS '手机号，唯一';
COMMENT ON COLUMN CM_USER.PWD IS '密码的md5';
COMMENT ON COLUMN CM_USER.NAME IS '姓名';
COMMENT ON COLUMN CM_USER.PHOTO IS '头像';
COMMENT ON COLUMN CM_USER.EXPIRED IS '#bool#是否停用';
COMMENT ON COLUMN CM_USER.CTIME IS '创建时间';
COMMENT ON COLUMN CM_USER.MTIME IS '修改时间';
COMMENT ON TABLE CM_USER IS '系统用户';

-- ----------------------------
-- Records of cm_user
-- ----------------------------
INSERT INTO CM_USER VALUES ('1', 'admin', '13511111111', 'b59c67bf196a4758191e42f76670ceba', '', '', '0', TO_DATE('2019-10-24 09:06:38', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2023-03-16 08:35:39', 'SYYYY-MM-DD HH24:MI:SS'));

-- ----------------------------
-- Table structure for cm_user_group
-- ----------------------------
CREATE TABLE CM_USER_GROUP (
  USER_ID NUMBER(19) NOT NULL,
  GROUP_ID NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN CM_USER_GROUP.USER_ID IS '用户标识';
COMMENT ON COLUMN CM_USER_GROUP.GROUP_ID IS '组标识';
COMMENT ON TABLE CM_USER_GROUP IS '用户一组多对多';

-- ----------------------------
-- Table structure for cm_user_params
-- ----------------------------
CREATE TABLE CM_USER_PARAMS (
  USER_ID NUMBER(19) NOT NULL,
  PARAM_ID NUMBER(19) NOT NULL,
  VALUE VARCHAR2(255),
  MTIME DATE NOT NULL
)
;
COMMENT ON COLUMN CM_USER_PARAMS.USER_ID IS '用户标识';
COMMENT ON COLUMN CM_USER_PARAMS.PARAM_ID IS '参数标识';
COMMENT ON COLUMN CM_USER_PARAMS.VALUE IS '参数值';
COMMENT ON COLUMN CM_USER_PARAMS.MTIME IS '修改时间';
COMMENT ON TABLE CM_USER_PARAMS IS '用户参数值';

-- ----------------------------
-- Table structure for cm_user_role
-- ----------------------------
CREATE TABLE CM_USER_ROLE (
  USER_ID NUMBER(19) NOT NULL,
  ROLE_ID NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN CM_USER_ROLE.USER_ID IS '用户标识';
COMMENT ON COLUMN CM_USER_ROLE.ROLE_ID IS '角色标识';
COMMENT ON TABLE CM_USER_ROLE IS '用户一角色多对多';

-- ----------------------------
-- Records of cm_user_role
-- ----------------------------
INSERT INTO CM_USER_ROLE VALUES ('1', '2');

-- ----------------------------
-- Table structure for cm_wfd_atv
-- ----------------------------
CREATE TABLE CM_WFD_ATV (
  ID NUMBER(19) NOT NULL,
  PRC_ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(64) NOT NULL,
  TYPE NUMBER(3) NOT NULL,
  EXEC_SCOPE NUMBER(3) NOT NULL,
  EXEC_LIMIT NUMBER(3) NOT NULL,
  EXEC_ATV_ID NUMBER(19),
  AUTO_ACCEPT CHAR(1) DEFAULT 0 NOT NULL,
  CAN_DELETE CHAR(1) DEFAULT 0 NOT NULL,
  CAN_TERMINATE CHAR(1) DEFAULT 0 NOT NULL,
  CAN_JUMP_INTO CHAR(1) DEFAULT 0 NOT NULL,
  TRANS_KIND NUMBER(3) NOT NULL,
  JOIN_KIND NUMBER(3) NOT NULL,
  CTIME DATE NOT NULL,
  MTIME DATE NOT NULL
)
;

ALTER TABLE CM_WFD_ATV ADD CHECK (AUTO_ACCEPT in (0,1));
ALTER TABLE CM_WFD_ATV ADD CHECK (CAN_DELETE in (0,1));
ALTER TABLE CM_WFD_ATV ADD CHECK (CAN_TERMINATE in (0,1));
ALTER TABLE CM_WFD_ATV ADD CHECK (CAN_JUMP_INTO in (0,1));

COMMENT ON COLUMN CM_WFD_ATV.ID IS '活动标识';
COMMENT ON COLUMN CM_WFD_ATV.PRC_ID IS '流程标识';
COMMENT ON COLUMN CM_WFD_ATV.NAME IS '活动名称，同时作为状态名称';
COMMENT ON COLUMN CM_WFD_ATV.TYPE IS '#WfdAtvType#活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动';
COMMENT ON COLUMN CM_WFD_ATV.EXEC_SCOPE IS '#WfdAtvExecScope#执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户';
COMMENT ON COLUMN CM_WFD_ATV.EXEC_LIMIT IS '#WfdAtvExecLimit#执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者';
COMMENT ON COLUMN CM_WFD_ATV.EXEC_ATV_ID IS '在执行者限制为3或4时选择的活动';
COMMENT ON COLUMN CM_WFD_ATV.AUTO_ACCEPT IS '#bool#是否自动签收，打开工作流视图时自动签收工作项';
COMMENT ON COLUMN CM_WFD_ATV.CAN_DELETE IS '#bool#能否删除流程实例和业务数据，0否 1';
COMMENT ON COLUMN CM_WFD_ATV.CAN_TERMINATE IS '#bool#能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能';
COMMENT ON COLUMN CM_WFD_ATV.CAN_JUMP_INTO IS '#bool#是否可作为跳转目标，0不可跳转 1可以';
COMMENT ON COLUMN CM_WFD_ATV.TRANS_KIND IS '#WfdAtvTransKind#当前活动的后续迁移方式 0:自由选择 1:并行 2:独占式选择';
COMMENT ON COLUMN CM_WFD_ATV.JOIN_KIND IS '#WfdAtvJoinKind#同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步';
COMMENT ON COLUMN CM_WFD_ATV.CTIME IS '创建时间';
COMMENT ON COLUMN CM_WFD_ATV.MTIME IS '修改时间';
COMMENT ON TABLE CM_WFD_ATV IS '活动模板';

-- ----------------------------
-- Table structure for cm_wfd_atv_role
-- ----------------------------
CREATE TABLE CM_WFD_ATV_ROLE (
  ATV_ID NUMBER(19) NOT NULL,
  ROLE_ID NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN CM_WFD_ATV_ROLE.ATV_ID IS '活动标识';
COMMENT ON COLUMN CM_WFD_ATV_ROLE.ROLE_ID IS '角色标识';
COMMENT ON TABLE CM_WFD_ATV_ROLE IS '活动授权';

-- ----------------------------
-- Table structure for cm_wfd_prc
-- ----------------------------
CREATE TABLE CM_WFD_PRC (
  ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(64) NOT NULL,
  DIAGRAM CLOB,
  IS_LOCKED CHAR(1) DEFAULT 0 NOT NULL,
  SINGLETON CHAR(1) DEFAULT 0 NOT NULL,
  NOTE VARCHAR2(255),
  DISPIDX NUMBER(9) NOT NULL,
  CTIME DATE NOT NULL,
  MTIME DATE NOT NULL
)
;

ALTER TABLE CM_WFD_PRC ADD CHECK (IS_LOCKED in (0,1));
ALTER TABLE CM_WFD_PRC ADD CHECK (SINGLETON in (0,1));

COMMENT ON COLUMN CM_WFD_PRC.ID IS '流程标识';
COMMENT ON COLUMN CM_WFD_PRC.NAME IS '流程名称';
COMMENT ON COLUMN CM_WFD_PRC.DIAGRAM IS '流程图';
COMMENT ON COLUMN CM_WFD_PRC.IS_LOCKED IS '#bool#锁定标志，0表未锁定；1表锁定，不能创建流程实例，已启动的流程实例继续执行';
COMMENT ON COLUMN CM_WFD_PRC.SINGLETON IS '#bool#同一时刻只允许有一个激活的流程实例，0表非单实例，1表单实例';
COMMENT ON COLUMN CM_WFD_PRC.NOTE IS '描述';
COMMENT ON COLUMN CM_WFD_PRC.DISPIDX IS '显示顺序';
COMMENT ON COLUMN CM_WFD_PRC.CTIME IS '创建时间';
COMMENT ON COLUMN CM_WFD_PRC.MTIME IS '最后修改时间';
COMMENT ON TABLE CM_WFD_PRC IS '流程模板';

-- ----------------------------
-- Table structure for cm_wfd_trs
-- ----------------------------
CREATE TABLE CM_WFD_TRS (
  ID NUMBER(19) NOT NULL,
  PRC_ID NUMBER(19) NOT NULL,
  SRC_ATV_ID NUMBER(19) NOT NULL,
  TGT_ATV_ID NUMBER(19) NOT NULL,
  IS_ROLLBACK CHAR(1) DEFAULT 0 NOT NULL,
  TRS_ID NUMBER(19)
)
;

ALTER TABLE CM_WFD_TRS ADD CHECK (IS_ROLLBACK in (0,1));

COMMENT ON COLUMN CM_WFD_TRS.ID IS '迁移标识';
COMMENT ON COLUMN CM_WFD_TRS.PRC_ID IS '流程模板标识';
COMMENT ON COLUMN CM_WFD_TRS.SRC_ATV_ID IS '起始活动模板标识';
COMMENT ON COLUMN CM_WFD_TRS.TGT_ATV_ID IS '目标活动模板标识';
COMMENT ON COLUMN CM_WFD_TRS.IS_ROLLBACK IS '#bool#是否为回退迁移';
COMMENT ON COLUMN CM_WFD_TRS.TRS_ID IS '类别为回退迁移时对应的常规迁移标识';
COMMENT ON TABLE CM_WFD_TRS IS '迁移模板';

-- ----------------------------
-- Table structure for cm_wfi_atv
-- ----------------------------
CREATE TABLE CM_WFI_ATV (
  ID NUMBER(19) NOT NULL,
  PRCI_ID NUMBER(19) NOT NULL,
  ATVD_ID NUMBER(19) NOT NULL,
  STATUS NUMBER(3) NOT NULL,
  INST_COUNT NUMBER(9) NOT NULL,
  CTIME DATE NOT NULL,
  MTIME DATE NOT NULL
)
;
COMMENT ON COLUMN CM_WFI_ATV.ID IS '活动实例标识';
COMMENT ON COLUMN CM_WFI_ATV.PRCI_ID IS '流程实例标识';
COMMENT ON COLUMN CM_WFI_ATV.ATVD_ID IS '活动模板标识';
COMMENT ON COLUMN CM_WFI_ATV.STATUS IS '#WfiAtvStatus#活动实例的状态 0活动 1结束 2终止 3同步活动';
COMMENT ON COLUMN CM_WFI_ATV.INST_COUNT IS '活动实例在流程实例被实例化的次数';
COMMENT ON COLUMN CM_WFI_ATV.CTIME IS '创建时间';
COMMENT ON COLUMN CM_WFI_ATV.MTIME IS '最后一次状态改变的时间';
COMMENT ON TABLE CM_WFI_ATV IS '活动实例';

-- ----------------------------
-- Table structure for cm_wfi_item
-- ----------------------------
CREATE TABLE CM_WFI_ITEM (
  ID NUMBER(19) NOT NULL,
  ATVI_ID NUMBER(19) NOT NULL,
  STATUS NUMBER(3) NOT NULL,
  ASSIGN_KIND NUMBER(3) NOT NULL,
  SENDER_ID NUMBER(19),
  SENDER VARCHAR2(32),
  STIME DATE NOT NULL,
  IS_ACCEPT CHAR(1) DEFAULT 0 NOT NULL,
  ACCEPT_TIME DATE,
  ROLE_ID NUMBER(19),
  USER_ID NUMBER(19),
  NOTE VARCHAR2(255),
  DISPIDX NUMBER(9) NOT NULL,
  CTIME DATE NOT NULL,
  MTIME DATE NOT NULL
)
;

ALTER TABLE CM_WFI_ITEM ADD CHECK (IS_ACCEPT in (0,1));

COMMENT ON COLUMN CM_WFI_ITEM.ID IS '工作项标识';
COMMENT ON COLUMN CM_WFI_ITEM.ATVI_ID IS '活动实例标识';
COMMENT ON COLUMN CM_WFI_ITEM.STATUS IS '#WfiItemStatus#工作项状态 0活动 1结束 2终止 3同步活动';
COMMENT ON COLUMN CM_WFI_ITEM.ASSIGN_KIND IS '#WfiItemAssignKind#指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派';
COMMENT ON COLUMN CM_WFI_ITEM.SENDER_ID IS '发送者标识';
COMMENT ON COLUMN CM_WFI_ITEM.SENDER IS '发送者姓名';
COMMENT ON COLUMN CM_WFI_ITEM.STIME IS '发送时间';
COMMENT ON COLUMN CM_WFI_ITEM.IS_ACCEPT IS '#bool#是否签收此项任务';
COMMENT ON COLUMN CM_WFI_ITEM.ACCEPT_TIME IS '签收时间';
COMMENT ON COLUMN CM_WFI_ITEM.ROLE_ID IS '执行者角色标识';
COMMENT ON COLUMN CM_WFI_ITEM.USER_ID IS '执行者用户标识';
COMMENT ON COLUMN CM_WFI_ITEM.NOTE IS '工作项备注';
COMMENT ON COLUMN CM_WFI_ITEM.DISPIDX IS '显示顺序';
COMMENT ON COLUMN CM_WFI_ITEM.CTIME IS '创建时间';
COMMENT ON COLUMN CM_WFI_ITEM.MTIME IS '最后一次状态改变的时间';
COMMENT ON TABLE CM_WFI_ITEM IS '工作项';

-- ----------------------------
-- Table structure for cm_wfi_prc
-- ----------------------------
CREATE TABLE CM_WFI_PRC (
  ID NUMBER(19) NOT NULL,
  PRCD_ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(255) NOT NULL,
  STATUS NUMBER(3) NOT NULL,
  DISPIDX NUMBER(9) NOT NULL,
  CTIME DATE NOT NULL,
  MTIME DATE NOT NULL
)
;
COMMENT ON COLUMN CM_WFI_PRC.ID IS '流程实例标识，同时为业务数据主键';
COMMENT ON COLUMN CM_WFI_PRC.PRCD_ID IS '流程模板标识';
COMMENT ON COLUMN CM_WFI_PRC.NAME IS '流转单名称';
COMMENT ON COLUMN CM_WFI_PRC.STATUS IS '#WfiPrcStatus#流程实例状态 0活动 1结束 2终止';
COMMENT ON COLUMN CM_WFI_PRC.DISPIDX IS '显示顺序';
COMMENT ON COLUMN CM_WFI_PRC.CTIME IS '创建时间';
COMMENT ON COLUMN CM_WFI_PRC.MTIME IS '最后一次状态改变的时间';
COMMENT ON TABLE CM_WFI_PRC IS '流程实例';

-- ----------------------------
-- Table structure for cm_wfi_trs
-- ----------------------------
CREATE TABLE CM_WFI_TRS (
  ID NUMBER(19) NOT NULL,
  TRSD_ID NUMBER(19) NOT NULL,
  SRC_ATVI_ID NUMBER(19) NOT NULL,
  TGT_ATVI_ID NUMBER(19) NOT NULL,
  IS_ROLLBACK CHAR(1) DEFAULT 0 NOT NULL,
  CTIME DATE NOT NULL
)
;

ALTER TABLE CM_WFI_TRS ADD CHECK (IS_ROLLBACK in (0,1));

COMMENT ON COLUMN CM_WFI_TRS.ID IS '迁移实例标识';
COMMENT ON COLUMN CM_WFI_TRS.TRSD_ID IS '迁移模板标识';
COMMENT ON COLUMN CM_WFI_TRS.SRC_ATVI_ID IS '起始活动实例标识';
COMMENT ON COLUMN CM_WFI_TRS.TGT_ATVI_ID IS '目标活动实例标识';
COMMENT ON COLUMN CM_WFI_TRS.IS_ROLLBACK IS '#bool#是否为回退迁移，1表回退';
COMMENT ON COLUMN CM_WFI_TRS.CTIME IS '迁移时间';
COMMENT ON TABLE CM_WFI_TRS IS '迁移实例';

-- ----------------------------
-- Table structure for fsm_file
-- ----------------------------
CREATE TABLE FSM_FILE (
  ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(512) NOT NULL,
  PATH VARCHAR2(512) NOT NULL,
  SIZE NUMBER(19) NOT NULL,
  INFO VARCHAR2(512),
  UPLOADER NUMBER(19) NOT NULL,
  CTIME DATE NOT NULL,
  DOWNLOADS NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN FSM_FILE.ID IS '文件标识';
COMMENT ON COLUMN FSM_FILE.NAME IS '文件名称';
COMMENT ON COLUMN FSM_FILE.PATH IS '存放路径：卷/两级目录/id.ext';
COMMENT ON COLUMN FSM_FILE.SIZE IS '文件长度';
COMMENT ON COLUMN FSM_FILE.INFO IS '文件描述';
COMMENT ON COLUMN FSM_FILE.UPLOADER IS '上传人id';
COMMENT ON COLUMN FSM_FILE.CTIME IS '上传时间';
COMMENT ON COLUMN FSM_FILE.DOWNLOADS IS '下载次数';

-- ----------------------------
-- Table structure for crud_大儿
-- ----------------------------
CREATE TABLE CRUD_大儿 (
  ID NUMBER(19) NOT NULL,
  PARENT_ID NUMBER(19) NOT NULL,
  大儿名 VARCHAR2(255)
)
;

-- ----------------------------
-- Records of crud_大儿
-- ----------------------------

-- ----------------------------
-- Table structure for crud_父表
-- ----------------------------
CREATE TABLE CRUD_父表 (
  ID NUMBER(19) NOT NULL,
  父名 VARCHAR2(255)
)
;

-- ----------------------------
-- Records of crud_父表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_缓存表
-- ----------------------------
CREATE TABLE CRUD_缓存表 (
  ID NUMBER(19) NOT NULL,
  手机号 CHAR(11),
  姓名 VARCHAR2(32)
)
;

-- ----------------------------
-- Records of crud_缓存表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_基础
-- ----------------------------
CREATE TABLE CRUD_基础 (
  ID NUMBER(19) NOT NULL,
  PARENT_ID NUMBER(19),
  序列 NUMBER(9) NOT NULL,
  名称 VARCHAR2(255),
  限长4 VARCHAR2(16),
  不重复 VARCHAR2(64),
  禁止选中 CHAR(1) NOT NULL,
  禁止保存 CHAR(1) NOT NULL,
  禁止删除 CHAR(1) NOT NULL,
  值变事件 VARCHAR2(64),
  发布插入事件 CHAR(1) NOT NULL,
  发布删除事件 CHAR(1) NOT NULL,
  创建时间 DATE NOT NULL,
  修改时间 DATE NOT NULL
)
;
COMMENT ON COLUMN CRUD_基础.ID IS '标识';
COMMENT ON COLUMN CRUD_基础.PARENT_ID IS '上级id，演示树状结构';
COMMENT ON COLUMN CRUD_基础.序列 IS '序列自动赋值';
COMMENT ON COLUMN CRUD_基础.限长4 IS '限制最大长度4';
COMMENT ON COLUMN CRUD_基础.不重复 IS '列值无重复';
COMMENT ON COLUMN CRUD_基础.禁止选中 IS '始终为false';
COMMENT ON COLUMN CRUD_基础.禁止保存 IS 'true时保存前校验不通过';
COMMENT ON COLUMN CRUD_基础.禁止删除 IS 'true时删除前校验不通过';
COMMENT ON COLUMN CRUD_基础.值变事件 IS '每次值变化时触发领域事件';
COMMENT ON COLUMN CRUD_基础.发布插入事件 IS 'true时允许发布插入事件';
COMMENT ON COLUMN CRUD_基础.发布删除事件 IS 'true时允许发布删除事件';
COMMENT ON COLUMN CRUD_基础.创建时间 IS '初次创建时间';
COMMENT ON COLUMN CRUD_基础.修改时间 IS '最后修改时间';

-- ----------------------------
-- Records of crud_基础
-- ----------------------------

-- ----------------------------
-- Table structure for crud_角色
-- ----------------------------
CREATE TABLE CRUD_角色 (
  ID NUMBER(19) NOT NULL,
  角色名称 VARCHAR2(32),
  角色描述 VARCHAR2(255)
)
;
COMMENT ON COLUMN CRUD_角色.ID IS '角色标识';
COMMENT ON COLUMN CRUD_角色.角色名称 IS '角色名称';
COMMENT ON COLUMN CRUD_角色.角色描述 IS '角色描述';
COMMENT ON TABLE CRUD_角色 IS '角色';

-- ----------------------------
-- Records of crud_角色
-- ----------------------------

-- ----------------------------
-- Table structure for crud_角色权限
-- ----------------------------
CREATE TABLE CRUD_角色权限 (
  ROLE_ID NUMBER(19) NOT NULL,
  PRV_ID NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN CRUD_角色权限.ROLE_ID IS '角色标识';
COMMENT ON COLUMN CRUD_角色权限.PRV_ID IS '权限标识';
COMMENT ON TABLE CRUD_角色权限 IS '角色关联的权限';

-- ----------------------------
-- Records of crud_角色权限
-- ----------------------------

-- ----------------------------
-- Table structure for crud_扩展1
-- ----------------------------
CREATE TABLE CRUD_扩展1 (
  ID NUMBER(19) NOT NULL,
  扩展1名称 VARCHAR2(255),
  禁止选中 CHAR(1) NOT NULL,
  禁止保存 CHAR(1) NOT NULL
)
;
COMMENT ON COLUMN CRUD_扩展1.ID IS '标识';
COMMENT ON COLUMN CRUD_扩展1.禁止选中 IS '始终为false';
COMMENT ON COLUMN CRUD_扩展1.禁止保存 IS 'true时保存前校验不通过';

-- ----------------------------
-- Records of crud_扩展1
-- ----------------------------

-- ----------------------------
-- Table structure for crud_扩展2
-- ----------------------------
CREATE TABLE CRUD_扩展2 (
  ID NUMBER(19) NOT NULL,
  扩展2名称 VARCHAR2(255),
  禁止删除 CHAR(1) NOT NULL,
  值变事件 VARCHAR2(255)
)
;
COMMENT ON COLUMN CRUD_扩展2.ID IS '标识';
COMMENT ON COLUMN CRUD_扩展2.禁止删除 IS 'true时删除前校验不通过';
COMMENT ON COLUMN CRUD_扩展2.值变事件 IS '每次值变化时触发领域事件';

-- ----------------------------
-- Records of crud_扩展2
-- ----------------------------

-- ----------------------------
-- Table structure for crud_权限
-- ----------------------------
CREATE TABLE CRUD_权限 (
  ID NUMBER(19) NOT NULL,
  权限名称 VARCHAR2(255)
)
;
COMMENT ON COLUMN CRUD_权限.ID IS '权限名称';
COMMENT ON TABLE CRUD_权限 IS '权限';

-- ----------------------------
-- Records of crud_权限
-- ----------------------------

-- ----------------------------
-- Table structure for crud_小儿
-- ----------------------------
CREATE TABLE CRUD_小儿 (
  ID NUMBER(19) NOT NULL,
  GROUP_ID NUMBER(19) NOT NULL,
  小儿名 VARCHAR2(255)
)
;

-- ----------------------------
-- Records of crud_小儿
-- ----------------------------

-- ----------------------------
-- Table structure for crud_用户
-- ----------------------------
CREATE TABLE CRUD_用户 (
  ID NUMBER(19) NOT NULL,
  手机号 CHAR(11),
  姓名 VARCHAR2(32),
  密码 CHAR(32)
)
;
COMMENT ON COLUMN CRUD_用户.ID IS '用户标识';
COMMENT ON COLUMN CRUD_用户.手机号 IS '手机号，唯一';
COMMENT ON COLUMN CRUD_用户.姓名 IS '姓名';
COMMENT ON COLUMN CRUD_用户.密码 IS '密码的md5';
COMMENT ON TABLE CRUD_用户 IS '系统用户';

-- ----------------------------
-- Records of crud_用户
-- ----------------------------

-- ----------------------------
-- Table structure for crud_用户角色
-- ----------------------------
CREATE TABLE CRUD_用户角色 (
  USER_ID NUMBER(19) NOT NULL,
  ROLE_ID NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN CRUD_用户角色.USER_ID IS '用户标识';
COMMENT ON COLUMN CRUD_用户角色.ROLE_ID IS '角色标识';
COMMENT ON TABLE CRUD_用户角色 IS '用户关联的角色';

-- ----------------------------
-- Records of crud_用户角色
-- ----------------------------

-- ----------------------------
-- Table structure for crud_主表
-- ----------------------------
CREATE TABLE CRUD_主表 (
  ID NUMBER(19) NOT NULL,
  主表名称 VARCHAR2(255),
  限长4 VARCHAR2(16),
  不重复 VARCHAR2(255)
)
;
COMMENT ON COLUMN CRUD_主表.限长4 IS '限制最大长度4';
COMMENT ON COLUMN CRUD_主表.不重复 IS '列值无重复';

-- ----------------------------
-- Records of crud_主表
-- ----------------------------

-- ----------------------------
-- Table structure for crud_字段类型
-- ----------------------------
CREATE TABLE CRUD_字段类型 (
  ID NUMBER(19) NOT NULL,
  字符串 VARCHAR2(255),
  整型 NUMBER(9) NOT NULL,
  可空整型 NUMBER(9),
  长整型 NUMBER(19),
  布尔 CHAR(1) NOT NULL,
  可空布尔 CHAR(1),
  日期时间 DATE NOT NULL,
  可空时间 DATE,
  枚举 NUMBER(6) NOT NULL,
  可空枚举 NUMBER(6),
  单精度 NUMBER(24,0) NOT NULL,
  可空单精度 NUMBER(24,0)
)
;
COMMENT ON COLUMN CRUD_字段类型.枚举 IS '#Gender#性别';
COMMENT ON COLUMN CRUD_字段类型.可空枚举 IS '#Gender#性别';

-- ----------------------------
-- Records of crud_字段类型
-- ----------------------------

-- ----------------------------
-- Table structure for 部门
-- ----------------------------
CREATE TABLE 部门 (
  ID NUMBER(19) NOT NULL,
  上级ID NUMBER(19),
  编码 VARCHAR2(16),
  名称 VARCHAR2(32),
  说明 VARCHAR2(64),
  建档时间 DATE,
  撤档时间 DATE
)
;
COMMENT ON COLUMN 部门.说明 IS '位置、环境、备注等';

-- ----------------------------
-- Records of 部门
-- ----------------------------
INSERT INTO 部门 VALUES ('93173171340210176', NULL, '01', '设备科', NULL, TO_DATE('2024-06-14 10:37:22', 'SYYYY-MM-DD HH24:MI:SS'), NULL);
INSERT INTO 部门 VALUES ('93173345370271744', NULL, '02', '物资科', NULL, TO_DATE('2024-06-14 10:37:41', 'SYYYY-MM-DD HH24:MI:SS'), NULL);
INSERT INTO 部门 VALUES ('93174118862843904', NULL, '03', '财务科', NULL, TO_DATE('2024-06-14 10:40:52', 'SYYYY-MM-DD HH24:MI:SS'), NULL);

-- ----------------------------
-- Table structure for 部门人员
-- ----------------------------
CREATE TABLE 部门人员 (
  部门ID NUMBER(19) NOT NULL,
  人员ID NUMBER(19) NOT NULL,
  缺省 CHAR(1)
)
;
COMMENT ON COLUMN 部门人员.缺省 IS '当一个人员属于多个部门时，当前是否为缺省';

-- ----------------------------
-- Records of 部门人员
-- ----------------------------
INSERT INTO 部门人员 VALUES ('93173345370271744', '93233663974862848', '1');
INSERT INTO 部门人员 VALUES ('93173345370271744', '100436029211963392', '1');

-- ----------------------------
-- Table structure for 供应商
-- ----------------------------
CREATE TABLE 供应商 (
  ID NUMBER(19) NOT NULL,
  名称 VARCHAR2(64),
  执照号 VARCHAR2(32),
  执照效期 DATE,
  税务登记号 VARCHAR2(32),
  地址 VARCHAR2(128),
  电话 VARCHAR2(16),
  开户银行 VARCHAR2(64),
  帐号 VARCHAR2(32),
  联系人 VARCHAR2(32),
  建档时间 DATE,
  撤档时间 DATE,
  备注 VARCHAR2(255)
)
;

-- ----------------------------
-- Records of 供应商
-- ----------------------------
INSERT INTO 供应商 VALUES ('95034724012290048', '物资东厂', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, TO_DATE('2024-06-19 13:54:37', 'SYYYY-MM-DD HH24:MI:SS'), NULL, NULL);
INSERT INTO 供应商 VALUES ('95034312534290432', '仁和制药', NULL, NULL, NULL, NULL, '13698562456', NULL, NULL, NULL, TO_DATE('2024-06-19 13:53:52', 'SYYYY-MM-DD HH24:MI:SS'), NULL, NULL);

-- ----------------------------
-- Table structure for 人员
-- ----------------------------
CREATE TABLE 人员 (
  ID NUMBER(19) NOT NULL,
  姓名 VARCHAR2(32),
  出生日期 DATE,
  性别 NUMBER(6),
  工作日期 DATE,
  办公室电话 VARCHAR2(32),
  电子邮件 VARCHAR2(32),
  建档时间 DATE,
  撤档时间 DATE,
  撤档原因 VARCHAR2(64),
  USER_ID NUMBER(19)
)
;
COMMENT ON COLUMN 人员.性别 IS '#Gender#';
COMMENT ON COLUMN 人员.USER_ID IS '账号ID';

-- ----------------------------
-- Records of 人员
-- ----------------------------
INSERT INTO 人员 VALUES ('93233663974862848', '王库管', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '87374677803298816');
INSERT INTO 人员 VALUES ('93233694710722560', '张主管', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '87375101197316096');
INSERT INTO 人员 VALUES ('100436029211963392', '测试员', NULL, NULL, NULL, NULL, NULL, TO_DATE('2024-07-04 11:37:09', 'SYYYY-MM-DD HH24:MI:SS'), NULL, NULL, '1');

-- ----------------------------
-- Table structure for 物资分类
-- ----------------------------
CREATE TABLE 物资分类 (
  ID NUMBER(19) NOT NULL,
  名称 VARCHAR2(64) NOT NULL
)
;

-- ----------------------------
-- Records of 物资分类
-- ----------------------------
INSERT INTO 物资分类 VALUES ('95413444640272384', '电工材料');
INSERT INTO 物资分类 VALUES ('95419313314623488', '劳保材料');
INSERT INTO 物资分类 VALUES ('95419350320967680', '水暖材料');
INSERT INTO 物资分类 VALUES ('95419395929829376', '维修材料');
INSERT INTO 物资分类 VALUES ('95419431795322880', '办公材料');
INSERT INTO 物资分类 VALUES ('95419477521625088', '低值易耗');
INSERT INTO 物资分类 VALUES ('95419514808987648', '易耗材料');
INSERT INTO 物资分类 VALUES ('95419598749593600', '其他材料');

-- ----------------------------
-- Table structure for 物资计划
-- ----------------------------
CREATE TABLE 物资计划 (
  ID NUMBER(19) NOT NULL,
  部门ID NUMBER(19),
  NO VARCHAR2(8),
  计划类型 NUMBER(6),
  编制方法 NUMBER(6),
  摘要 VARCHAR2(64),
  编制人 VARCHAR2(32),
  编制日期 DATE,
  审核人 VARCHAR2(32),
  审核日期 DATE
)
;
COMMENT ON COLUMN 物资计划.计划类型 IS '#计划类型#月;季;年';
COMMENT ON COLUMN 物资计划.编制方法 IS '#计划编制方法#1-往年同期线性参照法,2-临近期间平均参照法,3-物资储备定额参照法,4-由部门申购计划产生';

-- ----------------------------
-- Records of 物资计划
-- ----------------------------

-- ----------------------------
-- Table structure for 物资计划明细
-- ----------------------------
CREATE TABLE 物资计划明细 (
  计划ID NUMBER(19) NOT NULL,
  物资ID NUMBER(19) NOT NULL,
  前期数量 NUMBER(24,0),
  上期数量 NUMBER(24,0),
  库存数量 NUMBER(24,0),
  计划数量 NUMBER(24,0),
  审批数量 NUMBER(24,0),
  单价 NUMBER(24,0),
  金额 NUMBER(24,0),
  显示顺序 NUMBER(9)
)
;
COMMENT ON COLUMN 物资计划明细.前期数量 IS '前年、上上月、前季度数量';
COMMENT ON COLUMN 物资计划明细.上期数量 IS '去年、上个月、上季度数量';

-- ----------------------------
-- Records of 物资计划明细
-- ----------------------------

-- ----------------------------
-- Table structure for 物资库存
-- ----------------------------
CREATE TABLE 物资库存 (
  ID NUMBER(19) NOT NULL,
  部门ID NUMBER(19),
  物资ID NUMBER(19),
  批次 VARCHAR2(16),
  可用数量 NUMBER(24,0),
  可用金额 NUMBER(24,0),
  实际数量 NUMBER(24,0),
  实际金额 NUMBER(24,0)
)
;
COMMENT ON COLUMN 物资库存.批次 IS '相同物资ID不同批次的物资独立计算库存，部门ID+物资ID+批次构成唯一索引';
COMMENT ON COLUMN 物资库存.可用数量 IS '当填写申领单还未审批时只影响可用数量，确保后填写申领单时数量有效';

-- ----------------------------
-- Records of 物资库存
-- ----------------------------

-- ----------------------------
-- Table structure for 物资目录
-- ----------------------------
CREATE TABLE 物资目录 (
  ID NUMBER(19) NOT NULL,
  分类ID NUMBER(19),
  名称 VARCHAR2(64),
  规格 VARCHAR2(64),
  产地 VARCHAR2(64),
  成本价 NUMBER(24,0),
  核算方式 NUMBER(6),
  摊销月数 NUMBER(9),
  建档时间 DATE,
  撤档时间 DATE
)
;
COMMENT ON COLUMN 物资目录.规格 IS '计量单位，如 盒、10个/包、20个/箱、支';
COMMENT ON COLUMN 物资目录.产地 IS '名称,规格,产地构成唯一索引';
COMMENT ON COLUMN 物资目录.成本价 IS '预估价格，库存计算金额用';
COMMENT ON COLUMN 物资目录.核算方式 IS '#物资核算方式#一次性、分期摊销(折旧)';
COMMENT ON COLUMN 物资目录.摊销月数 IS '当核算方式为分期摊销时的总月数';

-- ----------------------------
-- Records of 物资目录
-- ----------------------------
INSERT INTO 物资目录 VALUES ('104839509410344960', '95413444640272384', '电线', '米', '上海第一电线厂', NULL, NULL, NULL, TO_DATE('2024-07-16 15:15:05', 'SYYYY-MM-DD HH24:MI:SS'), NULL);
INSERT INTO 物资目录 VALUES ('95434428013375488', '95413444640272384', '测电笔', '只', '江苏苏州电工工具厂', NULL, NULL, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for 物资入出类别
-- ----------------------------
CREATE TABLE 物资入出类别 (
  ID NUMBER(19) NOT NULL,
  名称 VARCHAR2(32) NOT NULL,
  系数 NUMBER(6) NOT NULL,
  单号前缀 CHAR(2) NOT NULL
)
;
COMMENT ON COLUMN 物资入出类别.系数 IS '1-入库；-1-出库';

-- ----------------------------
-- Records of 物资入出类别
-- ----------------------------
INSERT INTO 物资入出类别 VALUES ('1', '外购入库', '1', 'WG');
INSERT INTO 物资入出类别 VALUES ('2', '自产入库', '1', 'ZC');
INSERT INTO 物资入出类别 VALUES ('3', '返还入库', '1', 'FH');
INSERT INTO 物资入出类别 VALUES ('4', '盘盈', '1', 'PY');
INSERT INTO 物资入出类别 VALUES ('5', '申领出库', '-1', 'SL');
INSERT INTO 物资入出类别 VALUES ('6', '物资报废', '-1', 'BF');
INSERT INTO 物资入出类别 VALUES ('7', '盘亏', '-1', 'PK');

-- ----------------------------
-- Table structure for 物资详单
-- ----------------------------
CREATE TABLE 物资详单 (
  ID NUMBER(19) NOT NULL,
  单据ID NUMBER(19) NOT NULL,
  物资ID NUMBER(19),
  序号 NUMBER(6),
  批次 VARCHAR2(16),
  数量 NUMBER(24,0),
  单价 NUMBER(24,0),
  金额 NUMBER(24,0),
  随货单号 VARCHAR2(128),
  发票号 VARCHAR2(128),
  发票日期 DATE,
  发票金额 NUMBER(24,0),
  盘点时间 DATE,
  盘点金额 NUMBER(24,0)
)
;
COMMENT ON COLUMN 物资详单.序号 IS '在一张单据内部从1连续编号，入出类别+冲销状态+单号+序号共同构成唯一索引';
COMMENT ON COLUMN 物资详单.数量 IS '按散装单位填写';
COMMENT ON COLUMN 物资详单.单价 IS '售价';
COMMENT ON COLUMN 物资详单.金额 IS '实际数量与单价的乘积。';
COMMENT ON COLUMN 物资详单.随货单号 IS '外购入库时填写';
COMMENT ON COLUMN 物资详单.发票号 IS '外购入库时填写';
COMMENT ON COLUMN 物资详单.发票日期 IS '外购入库时填写';
COMMENT ON COLUMN 物资详单.发票金额 IS '外购入库时填写';
COMMENT ON COLUMN 物资详单.盘点时间 IS '盘点有效';
COMMENT ON COLUMN 物资详单.盘点金额 IS '盘点有效';

-- ----------------------------
-- Records of 物资详单
-- ----------------------------

-- ----------------------------
-- Table structure for 物资主单
-- ----------------------------
CREATE TABLE 物资主单 (
  ID NUMBER(19) NOT NULL,
  部门ID NUMBER(19) NOT NULL,
  入出类别ID NUMBER(19) NOT NULL,
  状态 NUMBER(6) NOT NULL,
  单号 VARCHAR2(8) NOT NULL,
  摘要 VARCHAR2(64),
  填制人 VARCHAR2(32),
  填制日期 DATE,
  审核人 VARCHAR2(32),
  审核日期 DATE,
  入出系数 NUMBER(6),
  供应商ID NUMBER(19),
  发料人 VARCHAR2(32),
  发料日期 DATE,
  金额 NUMBER(24,0),
  发票金额 NUMBER(24,0)
)
;
COMMENT ON COLUMN 物资主单.状态 IS '#单据状态#0-填写;1-待审核;2-已审核;3-被冲销;4-冲销';
COMMENT ON COLUMN 物资主单.单号 IS '相同单号可以不同的冲销状态，命名：前缀+连续序号';
COMMENT ON COLUMN 物资主单.填制人 IS '如果是申领单，表示申领人';
COMMENT ON COLUMN 物资主单.入出系数 IS '1:物资入,-1:物资出;0-盘点记录单';
COMMENT ON COLUMN 物资主单.供应商ID IS '外购入库时填写';
COMMENT ON COLUMN 物资主单.发料人 IS '申请单时用效,主要反应该张单据什么人发的料';
COMMENT ON COLUMN 物资主单.发料日期 IS '申请单时用效';
COMMENT ON COLUMN 物资主单.金额 IS '单据内所有详单的金额和';

-- ----------------------------
-- Records of 物资主单
-- ----------------------------

-- ----------------------------
-- View structure for v_部门
-- ----------------------------
CREATE VIEW v_部门 AS SELECT a.id,
    a.上级id,
    a.编码,
    a.名称,
    a.说明,
    a.建档时间,
    a.撤档时间,
    b.名称 AS 上级部门
   FROM 部门 a
     LEFT JOIN 部门 b ON a.上级id = b.id;

-- ----------------------------
-- View structure for v_人员
-- ----------------------------
CREATE VIEW v_人员 AS SELECT a.id,
    a.姓名,
    a.出生日期,
    a.性别,
    a.工作日期,
    a.办公室电话,
    a.电子邮件,
    a.建档时间,
    a.撤档时间,
    a.撤档原因,
    a.user_id,
    COALESCE(b.name, b.acc, b.phone) AS 账号
   FROM 人员 a
     LEFT JOIN cm_user b ON a.user_id = b.id;

-- ----------------------------
-- View structure for v_物资目录
-- ----------------------------
CREATE VIEW v_物资目录 AS SELECT a.id,
    a.分类id,
    a.名称,
    a.规格,
    a.产地,
    a.成本价,
    a.核算方式,
    a.摊销月数,
    a.建档时间,
    a.撤档时间,
    b.名称 AS 物资分类
   FROM 物资目录 a
     LEFT JOIN 物资分类 b ON a.分类id = b.id;

-- ----------------------------
-- View structure for v_物资详单
-- ----------------------------
CREATE VIEW v_物资详单 AS SELECT a.id,
    a.单据id,
    a.物资id,
    a.序号,
    a.批次,
    a.数量,
    a.单价,
    a.金额,
    a.随货单号,
    a.发票号,
    a.发票日期,
    a.发票金额,
    a.盘点时间,
    a.盘点金额,
    b.名称 AS 物资名称,
    b.规格,
    b.产地
   FROM 物资详单 a
     LEFT JOIN 物资目录 b ON a.物资id = b.id;

-- ----------------------------
-- View structure for v_物资主单
-- ----------------------------
CREATE VIEW v_物资主单 AS SELECT a.id,
    a.部门id,
    a.入出类别id,
    a.状态,
    a.单号,
    a.摘要,
    a.填制人,
    a.填制日期,
    a.审核人,
    a.审核日期,
    a.入出系数,
    a.供应商id,
    a.发料人,
    a.发料日期,
    a.金额,
    a.发票金额,
    b.名称 AS 部门名称,
    c.名称 AS 供应商,
    d.名称 AS 入出类别
   FROM 物资主单 a
     LEFT JOIN 部门 b ON a.部门id = b.id
     LEFT JOIN 供应商 c ON a.供应商id = c.id
     LEFT JOIN 物资入出类别 d ON a.入出类别id = d.id;

-- ----------------------------
-- Primary Key structure for table CM_CACHE
-- ----------------------------
ALTER TABLE CM_CACHE ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table cm_file_my
-- ----------------------------
ALTER TABLE CM_FILE_MY ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_file_my
-- ----------------------------
CREATE INDEX IDX_MYFILE_PARENTID
  ON CM_FILE_MY (PARENT_ID ASC);
CREATE INDEX IDX_MYFILE_USERID
  ON CM_FILE_MY (USER_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_file_pub
-- ----------------------------
ALTER TABLE CM_FILE_PUB ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_file_pub
-- ----------------------------
CREATE INDEX IDX_PUBFILE_PARENTID
  ON CM_FILE_PUB (PARENT_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_group
-- ----------------------------
ALTER TABLE CM_GROUP ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_group
-- ----------------------------
CREATE UNIQUE INDEX IDX_GROUP_NAME
  ON CM_GROUP (NAME ASC);

-- ----------------------------
-- Primary Key structure for table cm_group_role
-- ----------------------------
ALTER TABLE CM_GROUP_ROLE ADD PRIMARY KEY (GROUP_ID, ROLE_ID);

-- ----------------------------
-- Indexes structure for table cm_group_role
-- ----------------------------
CREATE INDEX IDX_GROUPROLE_ROLEID
  ON CM_GROUP_ROLE (ROLE_ID ASC);
CREATE INDEX IDX_GROUPROLE_GROUPID
  ON CM_GROUP_ROLE (GROUP_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_menu
-- ----------------------------
ALTER TABLE CM_MENU ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_menu
-- ----------------------------
CREATE INDEX IDX_MENU_PARENTID
  ON CM_MENU (PARENT_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_option
-- ----------------------------
ALTER TABLE CM_OPTION ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_option
-- ----------------------------
CREATE INDEX IDX_OPTION_GROUPID
  ON CM_OPTION (GROUP_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_option_group
-- ----------------------------
ALTER TABLE CM_OPTION_GROUP ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table cm_params
-- ----------------------------
ALTER TABLE CM_PARAMS ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_params
-- ----------------------------
CREATE UNIQUE INDEX IDX_PARAMS_NAME
  ON CM_PARAMS (NAME ASC);

-- ----------------------------
-- Primary Key structure for table cm_permission_module
-- ----------------------------
ALTER TABLE CM_PERMISSION_MODULE ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table cm_permission_func
-- ----------------------------
ALTER TABLE CM_PERMISSION_FUNC ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_permission_func
-- ----------------------------
CREATE INDEX FK_PERMISSION_FUNC
  ON CM_PERMISSION_FUNC (MODULE_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_permission
-- ----------------------------
ALTER TABLE CM_PERMISSION ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_permission
-- ----------------------------
CREATE INDEX FK_PERMISSION
  ON CM_PERMISSION (FUNC_ID ASC);

-- ----------------------------
-- Uniques structure for table cm_permission_func
-- ----------------------------
ALTER TABLE CM_PERMISSION_FUNC ADD CONSTRAINT UK_PERMISSION_FUNC UNIQUE (MODULE_ID, NAME) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;

-- ----------------------------
-- Uniques structure for table cm_permission_func
-- ----------------------------
ALTER TABLE CM_PERMISSION ADD CONSTRAINT UK_PERMISSION UNIQUE (FUNC_ID, NAME) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;

-- ----------------------------
-- Primary Key structure for table cm_role
-- ----------------------------
ALTER TABLE CM_ROLE ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_role
-- ----------------------------
CREATE UNIQUE INDEX IDX_ROLE_NAME
  ON CM_ROLE (NAME ASC);

-- ----------------------------
-- Primary Key structure for table cm_role_menu
-- ----------------------------
ALTER TABLE CM_ROLE_MENU ADD PRIMARY KEY (ROLE_ID, MENU_ID);

-- ----------------------------
-- Indexes structure for table cm_role_menu
-- ----------------------------
CREATE INDEX IDX_ROLEMENU_MENUID
  ON CM_ROLE_MENU (MENU_ID ASC);
CREATE INDEX IDX_ROLEMENU_ROLEID
  ON CM_ROLE_MENU (ROLE_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_role_per
-- ----------------------------
ALTER TABLE CM_ROLE_PER ADD PRIMARY KEY (ROLE_ID, PER_ID);

-- ----------------------------
-- Indexes structure for table cm_role_per
-- ----------------------------
CREATE INDEX IDX_ROLEPER_PERID
  ON CM_ROLE_PER (PER_ID ASC);
CREATE INDEX IDX_ROLEPER_ROLEID
  ON CM_ROLE_PER (ROLE_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_rpt
-- ----------------------------
ALTER TABLE CM_RPT ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_rpt
-- ----------------------------
CREATE UNIQUE INDEX IDX_RPT_NAME
  ON CM_RPT (NAME ASC);

-- ----------------------------
-- Primary Key structure for table cm_user
-- ----------------------------
ALTER TABLE CM_USER ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_user
-- ----------------------------
CREATE INDEX IDX_USER_ACC
  ON CM_USER (ACC ASC);

CREATE INDEX IDX_USER_PHONE
  ON CM_USER (PHONE ASC);

-- ----------------------------
-- Primary Key structure for table cm_user_group
-- ----------------------------
ALTER TABLE CM_USER_GROUP ADD PRIMARY KEY (USER_ID, GROUP_ID);

-- ----------------------------
-- Indexes structure for table cm_user_group
-- ----------------------------
CREATE INDEX IDX_USERGROUP_GROUPID
  ON CM_USER_GROUP (GROUP_ID ASC);
CREATE INDEX IDX_USERGROUP_USERID
  ON CM_USER_GROUP (USER_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_user_params
-- ----------------------------
ALTER TABLE CM_USER_PARAMS ADD PRIMARY KEY (USER_ID, PARAM_ID);

-- ----------------------------
-- Indexes structure for table cm_user_params
-- ----------------------------
CREATE INDEX IDX_USERPARAMS_USERID
  ON CM_USER_PARAMS (USER_ID ASC);
CREATE INDEX IDX_USERPARAMS_PARAMSID
  ON CM_USER_PARAMS (PARAM_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_user_role
-- ----------------------------
ALTER TABLE CM_USER_ROLE ADD PRIMARY KEY (USER_ID, ROLE_ID);

-- ----------------------------
-- Indexes structure for table cm_user_role
-- ----------------------------
CREATE INDEX IDX_USERROLE_USERID
  ON CM_USER_ROLE (USER_ID ASC);
CREATE INDEX IDX_USERROLE_ROLEID
  ON CM_USER_ROLE (ROLE_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_wfd_atv
-- ----------------------------
ALTER TABLE CM_WFD_ATV ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_wfd_atv
-- ----------------------------
CREATE INDEX IDX_WFDATV_PRCID
  ON CM_WFD_ATV (PRC_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_wfd_atv_role
-- ----------------------------
ALTER TABLE CM_WFD_ATV_ROLE ADD PRIMARY KEY (ATV_ID, ROLE_ID);

-- ----------------------------
-- Indexes structure for table cm_wfd_atv_role
-- ----------------------------
CREATE INDEX IDX_WFDATVROLE_ROLEID
  ON CM_WFD_ATV_ROLE (ROLE_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_wfd_prc
-- ----------------------------
ALTER TABLE CM_WFD_PRC ADD PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table cm_wfd_trs
-- ----------------------------
ALTER TABLE CM_WFD_TRS ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_wfd_trs
-- ----------------------------
CREATE INDEX IDX_WFDTRS_PRCID
  ON CM_WFD_TRS (PRC_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_wfi_atv
-- ----------------------------
ALTER TABLE CM_WFI_ATV ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_wfi_atv
-- ----------------------------
CREATE INDEX IDX_WFIATV_PRCIID
  ON CM_WFI_ATV (PRCI_ID ASC);
CREATE INDEX IDX_WFIATV_ATVDID
  ON CM_WFI_ATV (ATVD_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_wfi_item
-- ----------------------------
ALTER TABLE CM_WFI_ITEM ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_wfi_item
-- ----------------------------
CREATE INDEX IDX_WFIITEM_ATVIID
  ON CM_WFI_ITEM (ATVI_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_wfi_prc
-- ----------------------------
ALTER TABLE CM_WFI_PRC ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_wfi_prc
-- ----------------------------
CREATE INDEX IDX_WFIPRC_PRCDID
  ON CM_WFI_PRC (PRCD_ID ASC);

-- ----------------------------
-- Primary Key structure for table cm_wfi_trs
-- ----------------------------
ALTER TABLE CM_WFI_TRS ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_wfi_trs
-- ----------------------------
CREATE INDEX IDX_WFITRS_TRSDID
  ON CM_WFI_TRS (TRSD_ID ASC);
CREATE INDEX IDX_WFITRS_SRCATVIID
  ON CM_WFI_TRS (SRC_ATVI_ID ASC);
CREATE INDEX IDX_WFITRS_TGTATVIID
  ON CM_WFI_TRS (TGT_ATVI_ID ASC);

-- ----------------------------
-- Primary Key structure for table fsm_file
-- ----------------------------
ALTER TABLE FSM_FILE ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table fsm_file
-- ----------------------------
CREATE UNIQUE INDEX IDX_FSM_FILE_PATH
  ON FSM_FILE (PATH ASC);

-- ----------------------------
-- Primary Key structure for table crud_大儿
-- ----------------------------
ALTER TABLE CRUD_大儿 ADD CONSTRAINT CRUD_大儿_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table crud_大儿
-- ----------------------------
CREATE INDEX IDX_大儿_PARENDID
  ON CRUD_大儿 (PARENT_ID ASC);

-- ----------------------------
-- Primary Key structure for table crud_父表
-- ----------------------------
ALTER TABLE CRUD_父表 ADD CONSTRAINT CRUD_父表_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table crud_缓存表
-- ----------------------------
ALTER TABLE CRUD_缓存表 ADD CONSTRAINT CRUD_缓存表_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table crud_基础
-- ----------------------------
ALTER TABLE CRUD_基础 ADD CONSTRAINT CRUD_基础_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table crud_角色
-- ----------------------------
ALTER TABLE CRUD_角色 ADD CONSTRAINT CRUD_角色_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table crud_角色权限
-- ----------------------------
ALTER TABLE CRUD_角色权限 ADD CONSTRAINT CRUD_角色权限_PKEY PRIMARY KEY (ROLE_ID, PRV_ID);

-- ----------------------------
-- Indexes structure for table crud_角色权限
-- ----------------------------
CREATE INDEX IDX_CRUD_角色权限_PRVID
  ON CRUD_角色权限 (PRV_ID ASC);
CREATE INDEX IDX_CRUD_角色权限_ROLEID
  ON CRUD_角色权限 (ROLE_ID ASC);

-- ----------------------------
-- Primary Key structure for table crud_扩展1
-- ----------------------------
ALTER TABLE CRUD_扩展1 ADD CONSTRAINT CRUD_扩展1_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table crud_扩展2
-- ----------------------------
ALTER TABLE CRUD_扩展2 ADD CONSTRAINT CRUD_扩展2_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table crud_权限
-- ----------------------------
ALTER TABLE CRUD_权限 ADD CONSTRAINT CRUD_权限_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table crud_小儿
-- ----------------------------
ALTER TABLE CRUD_小儿 ADD CONSTRAINT CRUD_小儿_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table crud_小儿
-- ----------------------------
CREATE INDEX IDX_小儿_PARENTID
  ON CRUD_小儿 (GROUP_ID ASC);

-- ----------------------------
-- Primary Key structure for table crud_用户
-- ----------------------------
ALTER TABLE CRUD_用户 ADD CONSTRAINT CRUD_用户_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table crud_用户角色
-- ----------------------------
ALTER TABLE CRUD_用户角色 ADD CONSTRAINT CRUD_用户角色_PKEY PRIMARY KEY (USER_ID, ROLE_ID);

-- ----------------------------
-- Indexes structure for table crud_用户角色
-- ----------------------------
CREATE INDEX IDX_CRUD_用户角色_ROLEID
  ON CRUD_用户角色 (ROLE_ID ASC);
CREATE INDEX IDX_CRUD_用户角色_USERID
  ON CRUD_用户角色 (USER_ID ASC);

-- ----------------------------
-- Primary Key structure for table crud_主表
-- ----------------------------
ALTER TABLE CRUD_主表 ADD CONSTRAINT CRUD_主表_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table crud_字段类型
-- ----------------------------
ALTER TABLE CRUD_字段类型 ADD CONSTRAINT CRUD_字段类型_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table fsm_file
-- ----------------------------
ALTER TABLE FSM_FILE ADD CONSTRAINT FSM_FILE_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table fsm_file
-- ----------------------------
CREATE UNIQUE INDEX IDX_FSM_FILE_PATH
  ON FSM_FILE (PATH ASC);

-- ----------------------------
-- Primary Key structure for table 部门
-- ----------------------------
ALTER TABLE 部门 ADD CONSTRAINT 部门_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table 部门人员
-- ----------------------------
ALTER TABLE 部门人员 ADD CONSTRAINT 部门人员_PKEY PRIMARY KEY (部门ID, 人员ID);

-- ----------------------------
-- Primary Key structure for table 供应商
-- ----------------------------
ALTER TABLE 供应商 ADD CONSTRAINT PK_供应商 PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table 人员
-- ----------------------------
ALTER TABLE 人员 ADD CONSTRAINT 人员_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table 物资分类
-- ----------------------------
ALTER TABLE 物资分类 ADD CONSTRAINT PK_物资分类 PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table 物资计划
-- ----------------------------
ALTER TABLE 物资计划 ADD CONSTRAINT PK_物资计划 PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table 物资计划明细
-- ----------------------------
ALTER TABLE 物资计划明细 ADD CONSTRAINT PK_物资计划明细 PRIMARY KEY (计划ID, 物资ID);

-- ----------------------------
-- Primary Key structure for table 物资库存
-- ----------------------------
ALTER TABLE 物资库存 ADD CONSTRAINT PK_物资库存 PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table 物资库存
-- ----------------------------
CREATE INDEX IX_物资库存_物资ID
  ON 物资库存 (物资ID ASC);
CREATE INDEX IX_物资库存_部门ID
  ON 物资库存 (部门ID ASC);

-- ----------------------------
-- Primary Key structure for table 物资目录
-- ----------------------------
ALTER TABLE 物资目录 ADD CONSTRAINT PK_物资目录 PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table 物资入出类别
-- ----------------------------
ALTER TABLE 物资入出类别 ADD CONSTRAINT 物资入出类别_PKEY PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table 物资详单
-- ----------------------------
ALTER TABLE 物资详单 ADD CONSTRAINT PK_物资详单 PRIMARY KEY (ID);

-- ----------------------------
-- Primary Key structure for table 物资主单
-- ----------------------------
ALTER TABLE 物资主单 ADD CONSTRAINT PK_物资主单 PRIMARY KEY (ID);

-- ----------------------------
-- Foreign Keys structure for table 部门
-- ----------------------------
ALTER TABLE 部门 ADD CONSTRAINT FK_部门_上级ID FOREIGN KEY (上级ID) REFERENCES 部门 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
-- Foreign Keys structure for table 部门人员
-- ----------------------------
ALTER TABLE 部门人员 ADD CONSTRAINT FK_部门人员_人员 FOREIGN KEY (人员ID) REFERENCES 人员 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;
ALTER TABLE 部门人员 ADD CONSTRAINT FK_部门人员_部门 FOREIGN KEY (部门ID) REFERENCES 部门 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
-- Foreign Keys structure for table 物资计划
-- ----------------------------
ALTER TABLE 物资计划 ADD CONSTRAINT FK_物资计划_部门 FOREIGN KEY (部门ID) REFERENCES 部门 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
-- Foreign Keys structure for table 物资计划明细
-- ----------------------------
ALTER TABLE 物资计划明细 ADD CONSTRAINT FK_物资计划明细_物资 FOREIGN KEY (物资ID) REFERENCES 物资目录 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;
ALTER TABLE 物资计划明细 ADD CONSTRAINT FK_物资计划明细_计划 FOREIGN KEY (计划ID) REFERENCES 物资计划 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
-- Foreign Keys structure for table 物资库存
-- ----------------------------

-- ----------------------------
-- Foreign Keys structure for table 物资目录
-- ----------------------------
ALTER TABLE 物资目录 ADD CONSTRAINT FK_物资目录_分类 FOREIGN KEY (分类ID) REFERENCES 物资分类 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
-- Foreign Keys structure for table 物资详单
-- ----------------------------
ALTER TABLE 物资详单 ADD CONSTRAINT FK_物资详单_单据 FOREIGN KEY (单据ID) REFERENCES 物资主单 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;
ALTER TABLE 物资详单 ADD CONSTRAINT FK_物资详单_物资 FOREIGN KEY (物资ID) REFERENCES 物资目录 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
-- Foreign Keys structure for table 物资主单
-- ----------------------------
ALTER TABLE 物资主单 ADD CONSTRAINT FK_物资主单_供应商 FOREIGN KEY (供应商ID) REFERENCES 供应商 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;
ALTER TABLE 物资主单 ADD CONSTRAINT FK_物资主单_入出类别 FOREIGN KEY (入出类别ID) REFERENCES 物资入出类别 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;
ALTER TABLE 物资主单 ADD CONSTRAINT FK_物资主单_部门 FOREIGN KEY (部门ID) REFERENCES 部门 (ID) NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
-- 序列
-- ----------------------------
CREATE SEQUENCE CM_MENU_DISPIDX START WITH 138;
CREATE SEQUENCE CM_OPTION_DISPIDX START WITH 1050;
CREATE SEQUENCE CM_WFD_PRC_DISPIDX START WITH 15;
CREATE SEQUENCE CM_WFI_ITEM_DISPIDX START WITH 258;
CREATE SEQUENCE CM_WFI_PRC_DISPIDX START WITH 81;

CREATE SEQUENCE CRUD_基础_序列 START WITH 81;
CREATE SEQUENCE 物资主单_单号 START WITH 11;
CREATE SEQUENCE 物资入出类别_ID START WITH 12;
