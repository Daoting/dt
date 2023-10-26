/*
Navicat 从 mysql 导出后修改：
1. VARCHAR2 VARCHAR2
2. NUMBER(4) 布尔类型修改为
   ISFOLDER CHAR(1) DEFAULT 0 NOT NULL ,
   ALTER TABLE CM_FILE_MY ADD CHECK (ISFOLDER in (0,1))

   有部分枚举类型改成 NUMBER(3)
3. NCLOB VARCHAR2(4000)
4. 双引号 删除
5. NCHAR CHAR
6. NUMBER(20) NUMBER(19)
7. NUMBER(11) NUMBER(9)
8. 存储过程内部的分号去除，避免 END 被拆分成独立语句
*/

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
COMMENT ON TABLE CM_GROUP IS '分组，与用户和角色多对多';

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
INSERT INTO CM_MENU VALUES ('1', NULL, '工作台', '1', '', '', '搬运工', '', '1', '0', TO_DATE('2019-03-07 10:45:44', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-03-07 10:45:43', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('2', '1', '用户账号', '0', '用户账号', '', '钥匙', '', '2', '0', TO_DATE('2019-11-08 11:42:28', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-11-08 11:43:53', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('3', '1', '菜单管理', '0', '菜单管理', '', '大图标', '', '3', '0', TO_DATE('2019-03-11 11:35:59', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-03-11 11:35:58', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('4', '1', '系统角色', '0', '系统角色', '', '两人', '', '4', '0', TO_DATE('2019-11-08 11:47:21', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-11-08 11:48:22', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('5', '1', '分组管理', '0', '分组管理', '', '分组', '', '5', '0', TO_DATE('2023-03-10 08:34:49', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2023-03-10 08:34:49', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('6', '1', '基础权限', '0', '基础权限', '', '审核', '', '6', '0', TO_DATE('2019-03-12 09:11:22', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-03-07 11:23:40', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('7', '1', '参数定义', '0', '参数定义', '', '调色板', '', '7', '0', TO_DATE('2019-03-12 15:35:56', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-03-12 15:37:10', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('8', '1', '基础选项', '0', '基础选项', '', '修理', '', '8', '0', TO_DATE('2019-11-08 11:49:40', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2019-11-08 11:49:46', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('9', '1', '报表设计', '0', '报表设计', '', '折线图', '', '76', '0', TO_DATE('2020-10-19 11:21:38', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2020-10-19 11:21:38', 'SYYYY-MM-DD HH24:MI:SS'));
INSERT INTO CM_MENU VALUES ('10', '1', '流程设计', '0', '流程设计', '', '双绞线', '', '79', '0', TO_DATE('2020-11-02 16:21:19', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2020-11-02 16:21:19', 'SYYYY-MM-DD HH24:MI:SS'));

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
-- Table structure for cm_permission
-- ----------------------------
CREATE TABLE CM_PERMISSION (
  ID NUMBER(19) NOT NULL,
  NAME VARCHAR2(64) NOT NULL,
  NOTE VARCHAR2(255)
)
;
COMMENT ON COLUMN CM_PERMISSION.ID IS '权限标识';
COMMENT ON COLUMN CM_PERMISSION.NAME IS '权限名称';
COMMENT ON COLUMN CM_PERMISSION.NOTE IS '权限描述';
COMMENT ON TABLE CM_PERMISSION IS '权限';

-- ----------------------------
-- Records of cm_permission
-- ----------------------------
INSERT INTO CM_PERMISSION VALUES ('1', '公共文件管理', '禁止删除');
INSERT INTO CM_PERMISSION VALUES ('2', '素材库管理', '禁止删除');

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
INSERT INTO CM_ROLE_MENU VALUES ('1', '7');
INSERT INTO CM_ROLE_MENU VALUES ('1', '8');
INSERT INTO CM_ROLE_MENU VALUES ('1', '9');
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
INSERT INTO CM_ROLE_PER VALUES ('1', '1');
INSERT INTO CM_ROLE_PER VALUES ('1', '2');

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
  NAME VARCHAR2(32),
  PHONE VARCHAR2(16),
  PWD CHAR(32) NOT NULL,
  PHOTO VARCHAR2(255),
  EXPIRED CHAR(1) DEFAULT 0 NOT NULL,
  CTIME DATE NOT NULL,
  MTIME DATE NOT NULL
)
;

ALTER TABLE CM_USER ADD CHECK (EXPIRED in (0,1));

COMMENT ON COLUMN CM_USER.ID IS '用户标识';
COMMENT ON COLUMN CM_USER.NAME IS '账号，唯一';
COMMENT ON COLUMN CM_USER.PHONE IS '手机号，唯一';
COMMENT ON COLUMN CM_USER.PWD IS '密码的md5';
COMMENT ON COLUMN CM_USER.PHOTO IS '头像';
COMMENT ON COLUMN CM_USER.EXPIRED IS '#bool#是否停用';
COMMENT ON COLUMN CM_USER.CTIME IS '创建时间';
COMMENT ON COLUMN CM_USER.MTIME IS '修改时间';
COMMENT ON TABLE CM_USER IS '系统用户';

-- ----------------------------
-- Records of cm_user
-- ----------------------------
INSERT INTO CM_USER VALUES ('1', 'admin', '13511111111', 'b59c67bf196a4758191e42f76670ceba', '', '0', TO_DATE('2019-10-24 09:06:38', 'SYYYY-MM-DD HH24:MI:SS'), TO_DATE('2023-03-16 08:35:39', 'SYYYY-MM-DD HH24:MI:SS'));

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
COMMENT ON COLUMN CM_WFI_ITEM.SENDER IS '发送者';
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
  "SIZE" NUMBER(19) NOT NULL,
  INFO VARCHAR2(512),
  UPLOADER NUMBER(19) NOT NULL,
  CTIME DATE NOT NULL,
  DOWNLOADS NUMBER(19) NOT NULL
)
;
COMMENT ON COLUMN FSM_FILE.ID IS '文件标识';
COMMENT ON COLUMN FSM_FILE.NAME IS '文件名称';
COMMENT ON COLUMN FSM_FILE.PATH IS '存放路径：卷/两级目录/id.ext';
COMMENT ON COLUMN FSM_FILE."SIZE" IS '文件长度';
COMMENT ON COLUMN FSM_FILE.INFO IS '文件描述';
COMMENT ON COLUMN FSM_FILE.UPLOADER IS '上传人id';
COMMENT ON COLUMN FSM_FILE.CTIME IS '上传时间';
COMMENT ON COLUMN FSM_FILE.DOWNLOADS IS '下载次数';

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
-- Primary Key structure for table cm_permission
-- ----------------------------
ALTER TABLE CM_PERMISSION ADD PRIMARY KEY (ID);

-- ----------------------------
-- Indexes structure for table cm_permission
-- ----------------------------
CREATE UNIQUE INDEX IDX_PERMISSION_NAME
  ON CM_PERMISSION (NAME ASC);

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
CREATE INDEX IDX_USER_NAME
  ON "DT"."CM_USER" ("NAME" ASC);

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
-- Foreign Keys structure for table cm_file_my
-- ----------------------------
ALTER TABLE "CM_FILE_MY" ADD CONSTRAINT "FK_FILE_MY_PARENTID" FOREIGN KEY ("PARENT_ID") REFERENCES "CM_FILE_MY" ("ID");
ALTER TABLE "CM_FILE_MY" ADD CONSTRAINT "FK_FILE_MY_USERID" FOREIGN KEY ("USER_ID") REFERENCES "CM_USER" ("ID");

ALTER TABLE "CM_FILE_PUB" ADD CONSTRAINT "FK_FILE_PUB_PARENTID" FOREIGN KEY ("PARENT_ID") REFERENCES "CM_FILE_PUB" ("ID");

-- ----------------------------
-- Foreign Keys structure for table cm_group_role
-- ----------------------------
ALTER TABLE "CM_GROUP_ROLE" ADD CONSTRAINT "FK_GROUPROLE_GROUPID" FOREIGN KEY ("GROUP_ID") REFERENCES "CM_GROUP" ("ID") ON DELETE CASCADE;
ALTER TABLE "CM_GROUP_ROLE" ADD CONSTRAINT "FK_GROUPROLE_ROLEID" FOREIGN KEY ("ROLE_ID") REFERENCES "CM_ROLE" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_menu
-- ----------------------------
ALTER TABLE "CM_MENU" ADD CONSTRAINT "FK_MENU_PARENTID" FOREIGN KEY ("PARENT_ID") REFERENCES "CM_MENU" ("ID");

-- ----------------------------
-- Foreign Keys structure for table cm_option
-- ----------------------------
ALTER TABLE "CM_OPTION" ADD CONSTRAINT "FK_OPTION_GROUPID" FOREIGN KEY ("GROUP_ID") REFERENCES "CM_OPTION_GROUP" ("ID");

-- ----------------------------
-- Foreign Keys structure for table cm_role_menu
-- ----------------------------
ALTER TABLE "CM_ROLE_MENU" ADD CONSTRAINT "FK_ROLEMENU_MENUID" FOREIGN KEY ("MENU_ID") REFERENCES "CM_MENU" ("ID") ON DELETE CASCADE;
ALTER TABLE "CM_ROLE_MENU" ADD CONSTRAINT "FK_ROLEMENU_ROLEID" FOREIGN KEY ("ROLE_ID") REFERENCES "CM_ROLE" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_role_per
-- ----------------------------
ALTER TABLE "CM_ROLE_PER" ADD CONSTRAINT "FK_ROLEPER_PERID" FOREIGN KEY ("PER_ID") REFERENCES "CM_PERMISSION" ("ID") ON DELETE CASCADE;
ALTER TABLE "CM_ROLE_PER" ADD CONSTRAINT "FK_ROLEPER_ROLEID" FOREIGN KEY ("ROLE_ID") REFERENCES "CM_ROLE" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_user_group
-- ----------------------------
ALTER TABLE "CM_USER_GROUP" ADD CONSTRAINT "FK_USERGROUP_GROUPID" FOREIGN KEY ("GROUP_ID") REFERENCES "CM_GROUP" ("ID") ON DELETE CASCADE;
ALTER TABLE "CM_USER_GROUP" ADD CONSTRAINT "FK_USERGROUP_USERID" FOREIGN KEY ("USER_ID") REFERENCES "CM_USER" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_user_params
-- ----------------------------
ALTER TABLE "CM_USER_PARAMS" ADD CONSTRAINT "FK_USERPARAMS_PARAMSID" FOREIGN KEY ("PARAM_ID") REFERENCES "CM_PARAMS" ("ID") ON DELETE CASCADE;
ALTER TABLE "CM_USER_PARAMS" ADD CONSTRAINT "FK_USERPARAMS_USERID" FOREIGN KEY ("USER_ID") REFERENCES "CM_USER" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_user_role
-- ----------------------------
ALTER TABLE "CM_USER_ROLE" ADD CONSTRAINT "FK_USERROLE_ROLEID" FOREIGN KEY ("ROLE_ID") REFERENCES "CM_ROLE" ("ID") ON DELETE CASCADE;
ALTER TABLE "CM_USER_ROLE" ADD CONSTRAINT "FK_USERROLE_USERID" FOREIGN KEY ("USER_ID") REFERENCES "CM_USER" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfd_atv
-- ----------------------------
ALTER TABLE "CM_WFD_ATV" ADD CONSTRAINT "FK_WFDATV_PRCID" FOREIGN KEY ("PRC_ID") REFERENCES "CM_WFD_PRC" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfd_atv_role
-- ----------------------------
ALTER TABLE "CM_WFD_ATV_ROLE" ADD CONSTRAINT "FK_WFDATVROLE_ATVID" FOREIGN KEY ("ATV_ID") REFERENCES "CM_WFD_ATV" ("ID") ON DELETE CASCADE;
ALTER TABLE "CM_WFD_ATV_ROLE" ADD CONSTRAINT "FK_WFDATVROLE_ROLEID" FOREIGN KEY ("ROLE_ID") REFERENCES "CM_ROLE" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfd_trs
-- ----------------------------
ALTER TABLE "CM_WFD_TRS" ADD CONSTRAINT "FK_WFDTRS_PRCID" FOREIGN KEY ("PRC_ID") REFERENCES "CM_WFD_PRC" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_atv
-- ----------------------------
ALTER TABLE "CM_WFI_ATV" ADD CONSTRAINT "FK_WFIATV_ATVDID" FOREIGN KEY ("ATVD_ID") REFERENCES "CM_WFD_ATV" ("ID");
ALTER TABLE "CM_WFI_ATV" ADD CONSTRAINT "FK_WFIATV_PRCIID" FOREIGN KEY ("PRCI_ID") REFERENCES "CM_WFI_PRC" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_item
-- ----------------------------
ALTER TABLE "CM_WFI_ITEM" ADD CONSTRAINT "FK_WFIITEM_ATVIID" FOREIGN KEY ("ATVI_ID") REFERENCES "CM_WFI_ATV" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_prc
-- ----------------------------
ALTER TABLE "CM_WFI_PRC" ADD CONSTRAINT "FK_WFIPRC_PRCDID" FOREIGN KEY ("PRCD_ID") REFERENCES "CM_WFD_PRC" ("ID");

-- ----------------------------
-- Foreign Keys structure for table cm_wfi_trs
-- ----------------------------
ALTER TABLE "CM_WFI_TRS" ADD CONSTRAINT "FK_WFITRS_SRCATVIID" FOREIGN KEY ("SRC_ATVI_ID") REFERENCES "CM_WFI_ATV" ("ID") ON DELETE CASCADE;
ALTER TABLE "CM_WFI_TRS" ADD CONSTRAINT "FK_WFITRS_TGTATVIID" FOREIGN KEY ("TGT_ATVI_ID") REFERENCES "CM_WFI_ATV" ("ID") ON DELETE CASCADE;
ALTER TABLE "CM_WFI_TRS" ADD CONSTRAINT "FK_WFITRS_TRSDID" FOREIGN KEY ("TRSD_ID") REFERENCES "CM_WFD_TRS" ("ID") ON DELETE CASCADE;

-- ----------------------------
-- 序列
-- ----------------------------
DROP SEQUENCE CM_MENU_DISPIDX;
DROP SEQUENCE CM_OPTION_DISPIDX;
DROP SEQUENCE CM_WFD_PRC_DISPIDX;
DROP SEQUENCE CM_WFI_ITEM_DISPIDX;
DROP SEQUENCE CM_WFI_PRC_DISPIDX;
CREATE SEQUENCE CM_MENU_DISPIDX START WITH 90;
CREATE SEQUENCE CM_OPTION_DISPIDX START WITH 1032;
CREATE SEQUENCE CM_WFD_PRC_DISPIDX START WITH 12;
CREATE SEQUENCE CM_WFI_ITEM_DISPIDX START WITH 177;
CREATE SEQUENCE CM_WFI_PRC_DISPIDX START WITH 66;