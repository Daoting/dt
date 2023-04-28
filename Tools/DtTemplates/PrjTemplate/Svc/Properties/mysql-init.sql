-- MySQL dump 10.13  Distrib 5.6.13, for Win32 (x86)
--
-- Host: 10.10.1.2    Database: initdb
-- ------------------------------------------------------
-- Server version	5.7.21-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `cm_file_my`
--

DROP TABLE IF EXISTS `cm_file_my`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_file_my` (
  `ID` bigint(20) NOT NULL COMMENT '文件标识',
  `ParentID` bigint(20) DEFAULT NULL COMMENT '上级目录，根目录的parendid为空',
  `Name` varchar(255) NOT NULL COMMENT '名称',
  `IsFolder` tinyint(1) NOT NULL COMMENT '是否为文件夹',
  `ExtName` varchar(8) DEFAULT NULL COMMENT '文件扩展名',
  `Info` varchar(512) NOT NULL COMMENT '文件描述信息',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `UserID` bigint(20) NOT NULL COMMENT '所属用户',
  PRIMARY KEY (`ID`),
  KEY `fk_myfile_parentid` (`ParentID`),
  KEY `fk_user_userid` (`UserID`),
  CONSTRAINT `fk_myfile_parentid` FOREIGN KEY (`ParentID`) REFERENCES `cm_file_my` (`ID`),
  CONSTRAINT `fk_user_userid` FOREIGN KEY (`UserID`) REFERENCES `cm_user` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='个人文件';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_file_my`
--

LOCK TABLES `cm_file_my` WRITE;
/*!40000 ALTER TABLE `cm_file_my` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_file_my` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_file_pub`
--

DROP TABLE IF EXISTS `cm_file_pub`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_file_pub` (
  `ID` bigint(20) NOT NULL COMMENT '文件标识',
  `ParentID` bigint(20) DEFAULT NULL COMMENT '上级目录，根目录的parendid为空',
  `Name` varchar(255) NOT NULL COMMENT '名称',
  `IsFolder` tinyint(1) NOT NULL COMMENT '是否为文件夹',
  `ExtName` varchar(8) DEFAULT NULL COMMENT '文件扩展名',
  `Info` varchar(512) NOT NULL COMMENT '文件描述信息',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`ID`),
  KEY `fk_pubfile_parentid` (`ParentID`),
  CONSTRAINT `fk_pubfile_parentid` FOREIGN KEY (`ParentID`) REFERENCES `cm_file_pub` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='公共文件';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_file_pub`
--

LOCK TABLES `cm_file_pub` WRITE;
/*!40000 ALTER TABLE `cm_file_pub` DISABLE KEYS */;
INSERT INTO `cm_file_pub` VALUES (1,NULL,'公共文件',1,NULL,'','2020-10-21 15:19:20'),(2,NULL,'素材库',1,NULL,'','2020-10-21 15:20:21');
/*!40000 ALTER TABLE `cm_file_pub` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_group`
--

DROP TABLE IF EXISTS `cm_group`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_group` (
  `ID` bigint(20) NOT NULL COMMENT '组标识',
  `Name` varchar(64) NOT NULL COMMENT '组名',
  `Note` varchar(255) NOT NULL COMMENT '组描述',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `idx_group_name` (`Name`) COMMENT '不重复'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='分组，与用户和角色多对多';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_group`
--

LOCK TABLES `cm_group` WRITE;
/*!40000 ALTER TABLE `cm_group` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_group` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_group_role`
--

DROP TABLE IF EXISTS `cm_group_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_group_role` (
  `GroupID` bigint(20) NOT NULL COMMENT '组标识',
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`GroupID`,`RoleID`),
  KEY `fk_grouprole_roleid` (`RoleID`),
  KEY `fk_grouprole_groupid` (`GroupID`),
  CONSTRAINT `fk_grouprole_groupid` FOREIGN KEY (`GroupID`) REFERENCES `cm_group` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_grouprole_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='组一角色多对多';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_group_role`
--

LOCK TABLES `cm_group_role` WRITE;
/*!40000 ALTER TABLE `cm_group_role` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_group_role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_menu`
--

DROP TABLE IF EXISTS `cm_menu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_menu` (
  `ID` bigint(20) NOT NULL COMMENT '菜单标识',
  `ParentID` bigint(20) DEFAULT NULL COMMENT '父菜单标识',
  `Name` varchar(64) NOT NULL COMMENT '菜单名称',
  `IsGroup` tinyint(1) NOT NULL COMMENT '分组或实例。0表实例，1表分组',
  `ViewName` varchar(128) NOT NULL COMMENT '视图名称',
  `Params` varchar(4000) NOT NULL COMMENT '传递给菜单程序的参数',
  `Icon` varchar(128) NOT NULL COMMENT '图标',
  `Note` varchar(512) NOT NULL COMMENT '备注',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `IsLocked` tinyint(1) NOT NULL DEFAULT '0' COMMENT '定义了菜单是否被锁定。0表未锁定，1表锁定不可用',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后修改时间',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `idx_menu_dispidx` (`Dispidx`) USING BTREE COMMENT '确保唯一',
  KEY `fk_menu_parentid` (`ParentID`),
  CONSTRAINT `fk_menu_parentid` FOREIGN KEY (`ParentID`) REFERENCES `cm_menu` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='业务菜单';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_menu`
--

LOCK TABLES `cm_menu` WRITE;
/*!40000 ALTER TABLE `cm_menu` DISABLE KEYS */;
INSERT INTO `cm_menu` VALUES (1,NULL,'工作台',1,'','','搬运工','',1,0,'2019-03-07 10:45:44','2019-03-07 10:45:43'),(2,1,'用户账号',0,'用户账号','','钥匙','',2,0,'2019-11-08 11:42:28','2019-11-08 11:43:53'),(3,1,'菜单管理',0,'菜单管理','','大图标','',3,0,'2019-03-11 11:35:59','2019-03-11 11:35:58'),(4,1,'系统角色',0,'系统角色','','两人','',4,0,'2019-11-08 11:47:21','2019-11-08 11:48:22'),(5,1,'分组管理',0,'分组管理','','分组','',5,0,'2023-03-10 08:34:49','2023-03-10 08:34:49'),(6,1,'基础权限',0,'基础权限','','审核','',6,0,'2019-03-12 09:11:22','2019-03-07 11:23:40'),(7,1,'参数定义',0,'参数定义','','调色板','',7,0,'2019-03-12 15:35:56','2019-03-12 15:37:10'),(8,1,'基础选项',0,'基础选项','','修理','',8,0,'2019-11-08 11:49:40','2019-11-08 11:49:46'),(9,1,'报表设计',0,'报表设计','','折线图','',76,0,'2020-10-19 11:21:38','2020-10-19 11:21:38'),(10,1,'流程设计',0,'流程设计','','双绞线','',79,0,'2020-11-02 16:21:19','2020-11-02 16:21:19'),(11,1,'发布管理',0,'发布管理','','书籍','',88,0,'2020-10-27 16:09:27','2020-10-27 16:09:27');
/*!40000 ALTER TABLE `cm_menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_option`
--

DROP TABLE IF EXISTS `cm_option`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_option` (
  `ID` bigint(20) NOT NULL COMMENT '标识',
  `Name` varchar(64) NOT NULL COMMENT '选项名称',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `GroupID` bigint(20) NOT NULL COMMENT '所属分组',
  PRIMARY KEY (`ID`) USING BTREE,
  KEY `fk_option_groupid` (`GroupID`),
  CONSTRAINT `fk_option_groupid` FOREIGN KEY (`GroupID`) REFERENCES `cm_option_group` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='基础选项';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_option`
--

LOCK TABLES `cm_option` WRITE;
/*!40000 ALTER TABLE `cm_option` DISABLE KEYS */;
INSERT INTO `cm_option` VALUES (2,'汉族',2,1),(3,'蒙古族',3,1),(4,'回族',4,1),(5,'藏族',5,1),(6,'维吾尔族',6,1),(7,'苗族',7,1),(8,'彝族',8,1),(9,'壮族',9,1),(10,'布依族',10,1),(11,'朝鲜族',11,1),(12,'满族',12,1),(13,'侗族',13,1),(14,'瑶族',14,1),(15,'白族',15,1),(16,'土家族',16,1),(17,'哈尼族',17,1),(18,'哈萨克族',18,1),(19,'傣族',19,1),(20,'黎族',20,1),(21,'傈僳族',21,1),(22,'佤族',22,1),(23,'畲族',23,1),(24,'高山族',24,1),(25,'拉祜族',25,1),(26,'水族',26,1),(27,'东乡族',27,1),(28,'纳西族',28,1),(29,'景颇族',29,1),(30,'柯尔克孜族',30,1),(31,'土族',31,1),(32,'达斡尔族',32,1),(33,'仫佬族',33,1),(34,'羌族',34,1),(35,'布朗族',35,1),(36,'撒拉族',36,1),(37,'毛难族',37,1),(38,'仡佬族',38,1),(39,'锡伯族',39,1),(40,'阿昌族',40,1),(41,'普米族',41,1),(42,'塔吉克族',42,1),(43,'怒族',43,1),(44,'乌孜别克族',44,1),(45,'俄罗斯族',45,1),(46,'鄂温克族',46,1),(47,'德昂族',47,1),(48,'保安族',48,1),(49,'裕固族',49,1),(50,'京族',50,1),(51,'塔塔尔族',51,1),(52,'独龙族',52,1),(53,'鄂伦春族',53,1),(54,'赫哲族',54,1),(55,'门巴族',55,1),(56,'珞巴族',56,1),(57,'基诺族',57,1),(58,'大学',58,2),(59,'高中',59,2),(60,'中学',60,2),(61,'小学',61,2),(62,'硕士',62,2),(63,'博士',63,2),(64,'其他',64,2),(342,'男',342,4),(343,'女',343,4),(344,'未知',344,4),(345,'不明',345,4),(346,'string',346,5),(347,'int',347,5),(348,'double',348,5),(349,'DateTime',349,5),(350,'Date',350,5),(351,'bool',351,5);
/*!40000 ALTER TABLE `cm_option` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_option_group`
--

DROP TABLE IF EXISTS `cm_option_group`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_option_group` (
  `ID` bigint(20) NOT NULL COMMENT '标识',
  `Name` varchar(255) NOT NULL COMMENT '分组名称',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='基础选项分组';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_option_group`
--

LOCK TABLES `cm_option_group` WRITE;
/*!40000 ALTER TABLE `cm_option_group` DISABLE KEYS */;
INSERT INTO `cm_option_group` VALUES (1,'民族'),(2,'学历'),(4,'性别'),(5,'数据类型');
/*!40000 ALTER TABLE `cm_option_group` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_params`
--

DROP TABLE IF EXISTS `cm_params`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_params` (
  `ID` bigint(20) NOT NULL COMMENT '用户参数标识',
  `Name` varchar(255) NOT NULL COMMENT '参数名称',
  `Value` varchar(255) NOT NULL COMMENT '参数缺省值',
  `Note` varchar(255) NOT NULL COMMENT '参数描述',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `Name` (`Name`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='用户参数定义';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_params`
--

LOCK TABLES `cm_params` WRITE;
/*!40000 ALTER TABLE `cm_params` DISABLE KEYS */;
INSERT INTO `cm_params` VALUES (1,'接收新任务','true','','2020-12-01 15:13:49','2020-12-02 09:23:53'),(2,'接收新发布通知','true','','2020-12-02 09:25:15','2020-12-02 09:25:15'),(3,'接收新消息','true','接收通讯录消息推送','2020-12-02 09:24:28','2020-12-02 09:24:28');
/*!40000 ALTER TABLE `cm_params` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_permission`
--

DROP TABLE IF EXISTS `cm_permission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_permission` (
  `ID` bigint(20) NOT NULL COMMENT '权限标识',
  `Name` varchar(64) NOT NULL COMMENT '权限名称',
  `Note` varchar(255) DEFAULT NULL COMMENT '权限描述',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `idx_permission_name` (`Name`) COMMENT '不重复'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='权限';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_permission`
--

LOCK TABLES `cm_permission` WRITE;
/*!40000 ALTER TABLE `cm_permission` DISABLE KEYS */;
INSERT INTO `cm_permission` VALUES (1,'公共文件管理','禁止删除'),(2,'素材库管理','禁止删除');
/*!40000 ALTER TABLE `cm_permission` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_album`
--

DROP TABLE IF EXISTS `cm_pub_album`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_album` (
  `ID` bigint(20) NOT NULL COMMENT '专辑标识',
  `Name` varchar(255) NOT NULL COMMENT '名称',
  `Creator` varchar(32) NOT NULL COMMENT '创建人',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='文章专辑';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pub_album`
--

LOCK TABLES `cm_pub_album` WRITE;
/*!40000 ALTER TABLE `cm_pub_album` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_pub_album` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_comment`
--

DROP TABLE IF EXISTS `cm_pub_comment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_comment` (
  `ID` bigint(20) NOT NULL COMMENT '评论标识',
  `PostID` bigint(20) NOT NULL COMMENT '文章标识',
  `Content` varchar(4000) NOT NULL COMMENT '评论内容',
  `UserID` bigint(20) NOT NULL COMMENT '评论人标识',
  `UserName` varchar(64) NOT NULL COMMENT '评论人',
  `Ctime` datetime NOT NULL COMMENT '评论时间',
  `IsSpam` tinyint(1) NOT NULL COMMENT '是否为垃圾评论',
  `ParentID` bigint(20) DEFAULT NULL COMMENT '上级评论标识',
  `Support` int(11) NOT NULL COMMENT '对该评论的支持数',
  `Oppose` int(11) NOT NULL COMMENT '对该评论的反对数',
  PRIMARY KEY (`ID`),
  KEY `fk_comment_postid` (`PostID`),
  KEY `fk_comment_parentid` (`ParentID`),
  CONSTRAINT `fk_comment_parentid` FOREIGN KEY (`ParentID`) REFERENCES `cm_pub_comment` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_comment_postid` FOREIGN KEY (`PostID`) REFERENCES `cm_pub_post` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='评论信息';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pub_comment`
--

LOCK TABLES `cm_pub_comment` WRITE;
/*!40000 ALTER TABLE `cm_pub_comment` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_pub_comment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_keyword`
--

DROP TABLE IF EXISTS `cm_pub_keyword`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_keyword` (
  `ID` varchar(32) NOT NULL COMMENT '关键字',
  `Creator` varchar(32) NOT NULL COMMENT '创建人',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='关键字';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pub_keyword`
--

LOCK TABLES `cm_pub_keyword` WRITE;
/*!40000 ALTER TABLE `cm_pub_keyword` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_pub_keyword` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_post`
--

DROP TABLE IF EXISTS `cm_pub_post`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_post` (
  `ID` bigint(20) NOT NULL COMMENT '文章标识',
  `Title` varchar(255) NOT NULL COMMENT '标题',
  `Cover` varchar(1024) NOT NULL COMMENT '封面',
  `Summary` varchar(512) NOT NULL COMMENT '摘要',
  `Content` text NOT NULL COMMENT '内容',
  `TempType` tinyint(4) unsigned NOT NULL COMMENT '#PostTempType#在列表中显示时的模板类型',
  `IsPublish` tinyint(1) NOT NULL COMMENT '是否发布',
  `AllowCoverClick` tinyint(1) NOT NULL COMMENT '封面可点击',
  `AllowComment` tinyint(1) NOT NULL COMMENT '是否允许评论',
  `AddAlbumLink` tinyint(1) NOT NULL COMMENT '文章末尾是否添加同专辑链接',
  `AddKeywordLink` tinyint(1) NOT NULL COMMENT '文章末尾是否添加同关键字链接',
  `Url` varchar(128) NOT NULL COMMENT '文章地址',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `CreatorID` bigint(20) NOT NULL COMMENT '创建人ID',
  `Creator` varchar(32) NOT NULL COMMENT '创建人',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `LastEditorID` bigint(20) DEFAULT NULL COMMENT '最后编辑人ID',
  `LastEditor` varchar(32) DEFAULT NULL COMMENT '最后编辑人',
  `Mtime` datetime DEFAULT NULL COMMENT '最后修改时间',
  `ReadCount` int(11) NOT NULL COMMENT '阅读次数',
  `CommentCount` int(11) NOT NULL COMMENT '评论总数',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC COMMENT='文章/帖子';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pub_post`
--

LOCK TABLES `cm_pub_post` WRITE;
/*!40000 ALTER TABLE `cm_pub_post` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_pub_post` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_post_album`
--

DROP TABLE IF EXISTS `cm_pub_post_album`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_post_album` (
  `PostID` bigint(20) NOT NULL COMMENT '文章标识',
  `AlbumID` bigint(20) NOT NULL COMMENT '专辑标识',
  PRIMARY KEY (`PostID`,`AlbumID`) USING BTREE,
  KEY `fk_postalbum_albumid` (`AlbumID`),
  KEY `fk_postalbum_postid` (`PostID`) USING BTREE,
  CONSTRAINT `fk_postalbum_albumid` FOREIGN KEY (`AlbumID`) REFERENCES `cm_pub_album` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_postalbum_postid` FOREIGN KEY (`PostID`) REFERENCES `cm_pub_post` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pub_post_album`
--

LOCK TABLES `cm_pub_post_album` WRITE;
/*!40000 ALTER TABLE `cm_pub_post_album` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_pub_post_album` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_pub_post_keyword`
--

DROP TABLE IF EXISTS `cm_pub_post_keyword`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_pub_post_keyword` (
  `PostID` bigint(20) NOT NULL COMMENT '文章标识',
  `Keyword` varchar(32) NOT NULL COMMENT '关键字',
  PRIMARY KEY (`PostID`,`Keyword`) USING BTREE,
  KEY `fk_postkw_postid` (`PostID`) USING BTREE,
  KEY `fk_postkw_keyword` (`Keyword`),
  CONSTRAINT `fk_postkw_keyword` FOREIGN KEY (`Keyword`) REFERENCES `cm_pub_keyword` (`ID`),
  CONSTRAINT `fk_postkw_postid` FOREIGN KEY (`PostID`) REFERENCES `cm_pub_post` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_pub_post_keyword`
--

LOCK TABLES `cm_pub_post_keyword` WRITE;
/*!40000 ALTER TABLE `cm_pub_post_keyword` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_pub_post_keyword` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_role`
--

DROP TABLE IF EXISTS `cm_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_role` (
  `ID` bigint(20) NOT NULL COMMENT '角色标识',
  `Name` varchar(32) NOT NULL COMMENT '角色名称',
  `Note` varchar(255) DEFAULT NULL COMMENT '角色描述',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `idx_role_name` (`Name`) COMMENT '不重复'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='角色';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_role`
--

LOCK TABLES `cm_role` WRITE;
/*!40000 ALTER TABLE `cm_role` DISABLE KEYS */;
INSERT INTO `cm_role` VALUES (1,'任何人','所有用户默认都具有该角色，不可删除'),(2,'系统管理员','系统角色，不可删除');
/*!40000 ALTER TABLE `cm_role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_role_menu`
--

DROP TABLE IF EXISTS `cm_role_menu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_role_menu` (
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  `MenuID` bigint(20) NOT NULL COMMENT '菜单标识',
  PRIMARY KEY (`RoleID`,`MenuID`) USING BTREE,
  KEY `fk_rolemenu_menuid` (`MenuID`),
  KEY `fk_rolemenu_roleid` (`RoleID`),
  CONSTRAINT `fk_rolemenu_menuid` FOREIGN KEY (`MenuID`) REFERENCES `cm_menu` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_rolemenu_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='角色一菜单多对多';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_role_menu`
--

LOCK TABLES `cm_role_menu` WRITE;
/*!40000 ALTER TABLE `cm_role_menu` DISABLE KEYS */;
INSERT INTO `cm_role_menu` VALUES (2,2),(2,3),(2,4),(2,5),(2,6),(1,7),(1,8),(1,9),(2,10),(2,11);
/*!40000 ALTER TABLE `cm_role_menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_role_per`
--

DROP TABLE IF EXISTS `cm_role_per`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_role_per` (
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  `PerID` bigint(20) NOT NULL COMMENT '权限标识',
  PRIMARY KEY (`RoleID`,`PerID`),
  KEY `fk_roleper_perid` (`PerID`),
  KEY `fk_roleper_roleid` (`RoleID`),
  CONSTRAINT `fk_roleper_perid` FOREIGN KEY (`PerID`) REFERENCES `cm_permission` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_roleper_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='角色一权限多对多';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_role_per`
--

LOCK TABLES `cm_role_per` WRITE;
/*!40000 ALTER TABLE `cm_role_per` DISABLE KEYS */;
INSERT INTO `cm_role_per` VALUES (1,1),(1,2);
/*!40000 ALTER TABLE `cm_role_per` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_rpt`
--

DROP TABLE IF EXISTS `cm_rpt`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_rpt` (
  `ID` bigint(20) NOT NULL COMMENT '报表标识',
  `Name` varchar(64) NOT NULL COMMENT '报表名称',
  `Define` varchar(21000) NOT NULL COMMENT '报表模板定义',
  `Note` varchar(255) NOT NULL COMMENT '报表描述',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `idx_rpt_name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='报表模板定义';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_rpt`
--

LOCK TABLES `cm_rpt` WRITE;
/*!40000 ALTER TABLE `cm_rpt` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_rpt` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_sql`
--

DROP TABLE IF EXISTS `cm_sql`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_sql` (
  `id` varchar(128) NOT NULL COMMENT 'sql键值',
  `sql` varchar(20000) NOT NULL COMMENT 'sql内容',
  `note` varchar(255) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC COMMENT='服务的sql语句';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_sql`
--

LOCK TABLES `cm_sql` WRITE;
/*!40000 ALTER TABLE `cm_sql` DISABLE KEYS */;
INSERT INTO `cm_sql` VALUES ('个人文件-子级文件夹','select * from cm_file_my where isfolder=1 and parentid=@parentid',NULL),('个人文件-子项个数','select count(*) from cm_file_my where parentid=@parentid',NULL),('个人文件-所有子级','select * from cm_file_my where parentid=@parentid',NULL),('个人文件-扩展名过滤子级','select\r\n	* \r\nfrom\r\n	cm_file_my \r\nwhere\r\n	parentid = @parentid \r\n	and ( isfolder = 1 or locate( extname, @extname ) )',NULL),('个人文件-扩展名过滤根目录','select\r\n	* \r\nfrom\r\n	cm_file_my \r\nwhere\r\n	parentid is null \r\n	and userid = @userid \r\n	and ( isfolder = 1 or locate( extname, @extname ) )',NULL),('个人文件-搜索文件','select * from cm_file_my where isfolder=0 and userid=@userid and name like @name limit 20',NULL),('个人文件-根文件夹','select * from cm_file_my where isfolder=1 and parentid is null and userid=@userid',NULL),('个人文件-根目录','select * from cm_file_my where parentid is null and userid=@userid',NULL),('分组-关联用户','SELECT\r\n	id,\r\n	NAME,\r\n	phone \r\nFROM\r\n	cm_user a \r\nWHERE\r\n	EXISTS ( SELECT UserID FROM cm_user_group b WHERE a.ID = b.UserID AND GroupID = @ReleatedID )',NULL),('分组-关联角色','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_role a \r\nWHERE\r\n	EXISTS ( SELECT RoleID FROM cm_group_role b WHERE a.ID = b.RoleID AND GroupID = @ReleatedID )',NULL),('分组-分组列表的用户','SELECT DISTINCT(userid) FROM cm_user_group where FIND_IN_SET(groupid, @groupid)',NULL),('分组-未关联的用户','SELECT\r\n	id,\r\n	NAME,\r\n	phone \r\nFROM\r\n	cm_user a \r\nWHERE\r\n	NOT EXISTS ( SELECT UserID FROM cm_user_group b WHERE a.ID = b.UserID AND GroupID = @ReleatedID )',NULL),('分组-未关联的角色','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_role a \r\nWHERE\r\n	NOT EXISTS ( SELECT RoleID FROM cm_group_role b WHERE a.ID = b.RoleID AND GroupID = @ReleatedID )\r\n	 and a.ID!=1',NULL),('参数-用户参数值ByID','SELECT VALUE FROM cm_user_params WHERE userid = @userid and paramid = @paramid\r\nUNION\r\nSELECT VALUE FROM cm_params a  WHERE id = @paramid',NULL),('参数-用户参数值ByName','SELECT a.VALUE FROM cm_user_params a, cm_params b WHERE a.paramid=b.id and a.userid = @userid and b.name = @name\r\nUNION\r\nSELECT VALUE FROM cm_params a  WHERE name = @name',NULL),('参数-用户参数列表','SELECT paramid,VALUE FROM cm_user_params WHERE userid = @userid\r\n	UNION\nSELECT id,VALUE FROM cm_params a  WHERE\n	NOT EXISTS ( SELECT paramid FROM cm_user_params b WHERE a.id = b.paramid AND userid = @userid )\n',NULL),('参数-重复名称','select count(*) from cm_params where name=@name',NULL),('发布-专辑引用数','select count(*) from cm_pub_post_album where albumid=@albumid',NULL),('发布-关键字引用数','select count(*) from cm_pub_post_keyword where keyword=@keyword',NULL),('发布-模糊查询专辑','select * from cm_pub_album where name like @name',NULL),('发布-模糊查询关键字','select * from cm_pub_keyword where id like @id',NULL),('报表-最近修改','SELECT\r\n	id,name,note,ctime,mtime\r\nFROM\r\n	cm_rpt\r\nWHERE\r\n	to_days(now()) - to_days(mtime) <= 2',NULL),('报表-模板','select define from cm_rpt where id=@id',NULL),('报表-模糊查询',' SELECT\r\n	id,name,note,ctime,mtime\r\nFROM\r\n	cm_rpt\r\nWHERE\r\n	NAME LIKE @input',NULL),('报表-重复名称','select count(*) from cm_rpt where name=@name',NULL),('文件-子级文件夹','select * from cm_file_pub where isfolder=1 and parentid=@parentid',NULL),('文件-子项个数','select count(*) from cm_file_pub where parentid=@parentid',NULL),('文件-所有子级','select * from cm_file_pub where parentid=@parentid',NULL),('文件-扩展名过滤子级','select\r\n	* \r\nfrom\r\n	cm_file_pub \r\nwhere\r\n	parentid = @parentid \r\n	and ( isfolder = 1 or locate( extname, @extname ) )',NULL),('文件-搜索所有文件','select\r\n	info \r\nfrom\r\n	cm_file_pub \r\nwhere\r\n	isfolder = 0 \r\n	and name like @name union\r\nselect\r\n	info \r\nfrom\r\n	cm_file_my \r\nwhere\r\n	isfolder = 0 \r\n	and userid = @userid \r\n	and name like @name \r\n	limit 20',NULL),('文件-搜索扩展名文件','select\r\n	info \r\nfrom\r\n	cm_file_pub \r\nwhere\r\n	isfolder = 0 \r\n	and locate( extname, @extname ) \r\n	and name like @name union\r\nselect\r\n	info \r\nfrom\r\n	cm_file_my \r\nwhere\r\n	isfolder = 0 \r\n	and locate( extname, @extname ) \r\n	and userid = @userid \r\n	and name like @name \r\n	limit 20',NULL),('文件-搜索文件','select * from cm_file_pub where isfolder=0 and name like @name limit 20',NULL),('文章-已选专辑','select\r\n	a.id,\r\n	a.name \r\nfrom\r\n	cm_pub_album a\r\n	inner join cm_pub_post_album b on a.id = b.albumid \r\nwhere\r\n	b.postid = @postid',NULL),('文章-已选关键字','select keyword from cm_pub_post_keyword where postid=@postid',NULL),('文章-未选专辑','select\r\n	id,\r\n	name\r\nfrom\r\n	cm_pub_album a \r\nwhere\r\n	not exists ( select albumid from cm_pub_post_album where albumid = a.id and postid = @postid )',NULL),('文章-未选关键字','select\r\n	id \r\nfrom\r\n	cm_pub_keyword a \r\nwhere\r\n	not exists ( select keyword from cm_pub_post_keyword where keyword = a.id and postid = @postid )',NULL),('文章-模糊查询','select\r\n	ID,\r\n	Title,\r\n	IsPublish,\r\n	Dispidx,\r\n	Creator,\r\n	Ctime,\r\n	ReadCount,\r\n	CommentCount \r\nfrom\r\n	cm_pub_post\r\nwhere\r\n	Title like @input\r\norder by\r\n	Dispidx desc',NULL),('文章-管理列表','select\r\n	ID,\r\n	Title,\r\n	IsPublish,\r\n	Dispidx,\r\n	Creator,\r\n	Ctime,\r\n	ReadCount,\r\n	CommentCount \r\nfrom\r\n	cm_pub_post \r\norder by\r\n	Dispidx desc',NULL),('文章-编辑','select\r\n	a.*,\r\n	( CASE a.TempType WHEN 0 THEN \'普通\' WHEN 1 THEN \'封面标题混合\' ELSE \'普通\' END ) TempType_dsp \r\nfrom\r\n	cm_pub_post a \r\nwhere\r\n	id = @id',NULL),('权限-关联用户','select distinct (c.name)\r\n  from cm_role_prv a, cm_user_role b, cm_user c\r\n where a.roleid = b.roleid\r\n   and b.userid = c.id\r\n   and a.prvid = @prvid\r\n order by c.name',NULL),('权限-关联角色','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_role a \r\nWHERE\r\n	EXISTS ( SELECT RoleID FROM cm_role_per b WHERE a.ID = b.RoleID AND PerID = @ReleatedID )',NULL),('权限-名称重复','select count(id) from cm_permission where name=@name',NULL),('权限-未关联的角色','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_role a \r\nWHERE\r\n	NOT EXISTS ( SELECT RoleID FROM cm_role_per b WHERE a.ID = b.RoleID AND PerID = @ReleatedID )',NULL),('流程-前一活动执行者','select distinct\r\n	userid \r\nfrom\r\n	cm_wfi_item \r\nwhere\r\n	atviid in ( select id from cm_wfi_atv where prciid = @prciId and atvdid in ( select SrcAtvID from cm_wfd_trs where TgtAtvID = @atvdid ) )',NULL),('流程-前一活动的同部门执行者','select distinct\r\n	userid \r\nfrom\r\n	cm_xemp \r\nwhere\r\n	depid in (\r\nselect distinct\r\n	depid \r\nfrom\r\n	cm_xemp \r\nwhere\r\n	userid in (\r\nselect\r\n	userid \r\nfrom\r\n	cm_wfi_item \r\nwhere\r\n	atviid in ( select ID from cm_wfi_atv where prciid = @prciId and atvdid in ( select SrcAtvID from cm_wfd_trs where TgtAtvID = @atvdid ) ) \r\n	) \r\n	)',NULL),('流程-历史任务','select wi.id itemid,\r\n			 pi.id prciid,\r\n			 pd.id prcdid,\r\n			 ad.id atvdid,\r\n			 ai.id atviid,\r\n			 pd.name prcname,\r\n			 ( CASE pi.status WHEN 1 THEN \'已结束\' WHEN 2 THEN \'已终止\' ELSE ad.name END ) as atvname,\r\n			 pi.status,\r\n			 pi.name formname,\r\n			 wi.sender,\r\n			 wi.stime,\r\n			 max(wi.mtime) mtime,\r\n			 wi.reCount\r\n	from cm_wfi_atv ai,\r\n			 cm_wfi_prc pi,\r\n			 cm_wfd_atv ad,\r\n			 cm_wfd_prc pd,\r\n			 (select id,\r\n							 atviid,\r\n							 mtime,\r\n							 sender,\r\n							 stime,\r\n							 (select count(1)\r\n									from cm_wfi_item\r\n								 where atviid = t.atviid\r\n									 and AssignKind = 4\r\n									 and id <> t.id) as reCount\r\n					from cm_wfi_item t\r\n				 where status = 1\r\n					 and userid = @userID\r\n					 and (@start < \'1900-01-01\' or mtime >= @start)\r\n					 and (@end < \'1900-01-01\' or mtime <= @end)\r\n					 order by mtime desc) wi\r\n where wi.atviid = ai.id\r\n	 and ai.prciid = pi.id\r\n	 and pi.prcdid = pd.id\r\n	 and ai.atvdid = ad.id\r\n	 and wi.reCount = 0\r\n	 and (@status > 2 or pi.status = @status)\r\n group by prciid\r\n order by wi.stime desc',NULL),('流程-参与的流程','select distinct\r\n	p.id,\r\n	p.name\r\nfrom\r\n	cm_wfd_prc p,\r\n	cm_wfd_atv a,\r\n	cm_wfd_atv_role r,\r\n	cm_user_role u \r\nwhere\r\n	p.id = a.prcid \r\n	and a.id = r.atvid \r\n	and ( r.roleid = u.roleid or r.roleid = 1 ) \r\n	and u.userid = @userID\r\norder by\r\n	p.dispidx',NULL),('流程-可启动流程','select\r\n	pd.id,\r\n	name \r\nfrom\r\n	cm_wfd_prc pd,\r\n	(\r\nselect distinct\r\n	p.id \r\nfrom\r\n	cm_wfd_prc p,\r\n	cm_wfd_atv a,\r\n	cm_wfd_atv_role r,\r\n	cm_user_role u \r\nwhere\r\n	p.id = a.prcid \r\n	and a.id = r.atvid \r\n	and ( r.roleid = u.roleid or r.roleid = 1 ) \r\n	and u.userid = @userid \r\n	and p.islocked = 0 \r\n	and a.type = 1 \r\n	) pa \r\nwhere\r\n	pd.id = pa.id \r\norder by\r\n	dispidx;',NULL),('流程-同步活动实例数','select\r\n	count( * ) \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	prciid = @prciid \r\n	and atvdid = @atvdid',NULL),('流程-后续活动','select\r\n	atv.* \r\nfrom\r\n	cm_wfd_atv atv,\r\n	( select trs.TgtAtvID atvid from cm_wfd_trs trs where trs.SrcAtvID = @atvid and IsRollback = 0 ) trs \r\nwhere\r\n	atv.id = trs.atvid',NULL),('流程-后续活动工作项','select\r\n	a.IsAccept,\r\n	a.Status,\r\n	b.id atviid \r\nfrom\r\n	cm_wfi_item a,\r\n	cm_wfi_atv b \r\nwhere\r\n	a.atviid = b.id \r\n	and b.atvdid in ( select TgtAtvID from cm_wfd_trs d where d.SrcAtvID = @atvdid and d.IsRollback = 0 ) \r\n	and b.prciid = @prciid',NULL),('流程-回退活动实例','select\r\n	* \r\nfrom\r\n	cm_wfi_atv a \r\nwhere\r\n	prciid = @prciid \r\n	and exists ( select TgtAtvID from cm_wfd_trs b where SrcAtvID = @SrcAtvID and b.IsRollback = 1 and a.atvdid = b.TgtAtvID ) \r\norder by\r\n	mtime desc',NULL),('流程-实例id获取模板id','select PrcdID from cm_wfi_prc where id=@id',NULL),('流程-工作项个数','select\r\n	count( * ) \r\nfrom\r\n	cm_wfi_item \r\nwhere\r\n	atviid = @atviid \r\n	and status = 1',NULL),('流程-工作项的活动实例','select\r\n	* \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	id = ( select atviid from cm_wfi_item where id = @itemid )',NULL),('流程-工作项的流程实例','select\r\n	* \r\nfrom\r\n	cm_wfi_prc \r\nwhere\r\n	id = ( select prciid from cm_wfi_atv where id = ( select atviid from cm_wfi_item where id = @itemid ) )',NULL),('流程-已完成活动同部门执行者','select distinct\r\n	userid \r\nfrom\r\n	cm_xemp \r\nwhere\r\n	depid in (\r\nselect distinct\r\n	depid \r\nfrom\r\n	cm_xemp \r\nwhere\r\n	userid in ( select userid from cm_wfi_item where atviid in ( select id from cm_wfi_atv where prciid = @prciId and atvdid = @atvdid ) ) \r\n	)',NULL),('流程-已完成活动执行者','select distinct\r\n	userid \r\nfrom\r\n	cm_wfi_item \r\nwhere\r\n	atviid in ( select id from cm_wfi_atv where prciid = @prciId and atvdid = @atvdid )',NULL),('流程-待办任务','select wi.id   itemid,\r\n		 pi.id     prciid,\r\n		 pd.id     prcdid,\r\n		 pd.name   prcname,\r\n		 ad.name   atvname,\r\n		 pi.name   formname,\r\n		 wi.AssignKind,\r\n		 wi.sender,\r\n		 wi.stime,\r\n		 wi.IsAccept\r\nfrom cm_wfi_atv ai,\r\n		 cm_wfd_atv ad,\r\n		 cm_wfi_prc pi,\r\n		 cm_wfd_prc pd,\r\n		 (select id,\r\n						 atviid,\r\n						 sender,\r\n						 stime,\r\n						 IsAccept,\r\n						 AssignKind\r\n				from cm_wfi_item wi\r\n			 where status = 0\r\n				 and (userid = @userID or\r\n						 (userid is null and\r\n						 (exists (select 1\r\n													from cm_user_role\r\n												 where wi.roleid = roleid\r\n													 and userid = @userID)) or\r\n						 roleid = 1))) wi\r\nwhere ai.id = wi.atviid\r\n and ai.atvdid = ad.id\r\n and ai.prciid = pi.id\r\n and pi.prcdid = pd.id\r\norder by wi.stime desc',NULL),('流程-待办任务总数','select\r\n	sum( 1 ) allTask \r\nfrom\r\n	cm_wfi_prc a,\r\n	cm_wfi_atv b,\r\n	cm_wfi_item c \r\nwhere\r\n	a.id = b.prciid \r\n	and b.id = c.atviid \r\n	and c.status = 0 \r\n	and (\r\n	c.userid = @userid \r\n	or ( userid is null and exists ( select 1 from cm_user_role where c.roleid = roleid and userid = @userid ) ) \r\n	)',NULL),('流程-所有未过期用户','select id, name from cm_user where expired = 0',NULL),('流程-所有流程模板','select ID,Name,IsLocked,Singleton,Note,Dispidx,Ctime,Mtime from cm_wfd_prc order by Dispidx',NULL),('流程-所有流程模板名称','select\r\n	id,\r\n	name \r\nfrom\r\n	cm_wfd_prc \r\norder by\r\n	dispidx;',NULL),('流程-所有经办历史任务','select wi.id itemid,\r\n			 pi.id prciid,\r\n			 pd.id prcdid,\r\n			 ad.id atvdid,\r\n			 ai.id atviid,\r\n			 pd.name prcname,\r\n			 ad.name atvname,\r\n			 pi.status,\r\n			 pi.name formname,\r\n			 wi.sender,\r\n			 wi.stime,\r\n			 wi.mtime,\r\n			 wi.reCount\r\n	from cm_wfi_atv ai,\r\n			 cm_wfi_prc pi,\r\n			 cm_wfd_atv ad,\r\n			 cm_wfd_prc pd,\r\n			 (select id,\r\n							 atviid,\r\n							 mtime,\r\n							 sender,\r\n							 stime,\r\n							 (select count(1)\r\n									from cm_wfi_item\r\n								 where atviid = t.atviid\r\n									 and AssignKind = 4\r\n									 and id <> t.id) as reCount\r\n					from cm_wfi_item t\r\n				 where status = 1\r\n					 and userid = @userID\r\n					 and (@start < \'1900-01-01\' or mtime >= @start)\r\n					 and (@end < \'1900-01-01\' or mtime <= @end)) wi\r\n	where wi.atviid = ai.id\r\n	 and ai.prciid = pi.id\r\n	 and pi.prcdid = pd.id\r\n	 and ai.atvdid = ad.id\r\n	 and (@status > 2 or pi.status = @status)\r\n	order by wi.stime desc',NULL),('流程-日志目标项','select ( CASE username WHEN NULL THEN rolename ELSE username END ) accpname,\r\n			 atvdname,\r\n			 atvdtype,\r\n			 joinkind,\r\n			 atviid\r\n	from (select a.atviid,\r\n							 (select group_concat(name order by a.dispidx separator \'、\') from cm_user where id = a.userid) as username,\r\n							 (select group_concat(name order by a.dispidx separator \'、\') from cm_role where id = a.roleid) as rolename,\r\n							 max(a.dispidx) dispidx,\r\n							 c.name as atvdname,\r\n							 c.type as atvdtype,\r\n							 c.joinkind\r\n					from cm_wfi_item a,\r\n							 (select ti.TgtAtviID id\r\n									from cm_wfi_atv ai, cm_wfi_trs ti\r\n								 where ai.id = ti.SrcAtviID\r\n									 and ai.prciid = @prciid\r\n									 and ti.SrcAtviID = @atviid) b,\r\n							 cm_wfd_atv c,\r\n							 cm_wfi_atv d\r\n				 where a.atviid = b.id\r\n					 and b.id = d.id\r\n					 and d.atvdid = c.id\r\n				 group by a.atviid, c.name, c.type, c.joinkind) t\r\n order by dispidx',NULL),('流程-是否活动授权任何人','select\r\n	count(*) \r\nfrom\r\n	cm_wfd_atv_role \r\nwhere\r\n	roleid = 1 \r\n	and atvid = @atvid',NULL),('流程-最后工作项','select\r\n	wi.id itemid,\r\n	pi.PrcdID prcid \r\nfrom\r\n	cm_wfi_item wi,\r\n	cm_wfi_atv wa,\r\n	cm_wfi_prc pi \r\nwhere\r\n	wi.atviid = wa.id \r\n	and wa.PrciID = pi.id \r\n	and pi.id = @prciID \r\norder by\r\n	wi.mtime desc \r\n	LIMIT 0,\r\n	1',NULL),('流程-最后已完成活动ID','select\r\n	id \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	prciid = @prciid \r\n	and atvdid = @atvdid \r\n	and status = 1 \r\norder by\r\n	mtime desc',NULL),('流程-最近修改','select ID,Name,IsLocked,Singleton,Note,Dispidx,Ctime,Mtime from cm_wfd_prc WHERE to_days(now()) - to_days(mtime) <= 2',NULL),('流程-查找实例','select\r\n	id,\r\n	PrcdID,\r\n	name,\r\n	Status,\r\n	Ctime,\r\n	Mtime \r\nfrom\r\n	cm_wfi_prc \r\nwhere\r\n	PrcdID = @PrcdID \r\n	and ( @Status > 2 or `Status` = @Status ) \r\n	and ( @title = \'\' or name = @title ) \r\n	and ( @start < \'1900-01-01\' or Mtime >= @start ) \r\n	and ( @end < \'1900-01-01\' or Mtime <= @end ) \r\norder by\r\n	dispidx',NULL),('流程-模糊查询','select ID,Name,IsLocked,Singleton,Note,Dispidx,Ctime,Mtime from cm_wfd_prc WHERE NAME LIKE @input',NULL),('流程-活动前的迁移','select\r\n	* \r\nfrom\r\n	cm_wfd_trs \r\nwhere\r\n	TgtAtvID = @TgtAtvID',NULL),('流程-活动发送者','select\r\n	sender \r\nfrom\r\n	cm_wfi_item \r\nwhere\r\n	atviid = @atviid \r\norder by\r\n	mtime desc',NULL),('流程-活动实例状态','select\r\n	atvdid,\r\n	status \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	prciid = @prciid \r\norder by\r\n	ctime',NULL),('流程-活动实例的工作项','select\r\n	status,\r\n	AssignKind,\r\n	concat( sender, \' -> \', usr.name ) sendprc,\r\n	IsAccept,\r\n	wi.mtime \r\nfrom\r\n	cm_wfi_item wi\r\n	left join cm_user usr on wi.userid = usr.id \r\nwhere\r\n	atviid = @atviID \r\norder by\r\n	dispidx',NULL),('流程-活动实例的状态','select status \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	atvdid = @atvdid \r\n	and prciid = @prciid',NULL),('流程-活动未关联的角色','SELECT\r\n	a.id,\r\n	a.NAME\r\nFROM\r\n	cm_role a\r\nWHERE\r\n	NOT EXISTS ( SELECT roleid FROM cm_wfd_atv_role b WHERE a.id = b.roleid AND atvid = @atvid )',NULL),('流程-活动的所有执行者','select\r\n	id,\r\nname \r\nfrom\r\n	cm_user u \r\nwhere\r\n	exists (\r\nselect distinct\r\n	( userid ) \r\nfrom\r\n	cm_user_role ur \r\nwhere\r\n	exists ( select roleid from cm_wfd_atv_role ar where ur.roleid = ar.roleid and atvid = @atvid ) \r\n	and u.id = ur.userid \r\n	) \r\norder by\r\nname',NULL),('流程-活动的所有授权角色','select\r\n	id,\r\nname \r\nfrom\r\n	cm_role r \r\nwhere\r\n	exists ( select distinct ( roleid ) from cm_wfd_atv_role ar where r.id = ar.roleid and atvid = @atvid )',NULL),('流程-活动结束的实例数','select\r\n	count( * ) \r\nfrom\r\n	cm_wfi_atv \r\nwhere\r\n	atvdid = @atvdid \r\n	and prciid = @prciid \r\n	and status = 1',NULL),('流程-流程实例数','select count(*) from cm_wfi_prc where PrcdID=@PrcdID',NULL),('流程-流程实例的活动实例','select\r\n	atvi.id,\r\n	atvd.name,\r\n	status,\r\n	instcount \r\nfrom\r\n	cm_wfi_atv atvi,\r\n	cm_wfd_atv atvd \r\nwhere\r\n	atvi.atvdid = atvd.id \r\n	and atvi.prciid = @prciID \r\norder by\r\n	atvi.ctime',NULL),('流程-生成日志列表','select b.prciid,\r\n			 b.id atviid,\r\n			 c.status prcistatus,\r\n			 d.name atvdname,\r\n			 a.AssignKind,\r\n			 a.IsAccept,\r\n			 a.AcceptTime,\r\n			 a.status itemstatus,\r\n			 ( CASE userid WHEN NULL THEN (select name from cm_role t where t.id = a.roleid) ELSE (select name from cm_user t where t.id = a.userid) END ) username,\r\n			 a.note,\r\n			 a.ctime,\r\n			 a.mtime,\r\n			 c.mtime prcitime,\r\n			 a.sender\r\nfrom cm_wfi_item a, cm_wfi_atv b, cm_wfi_prc c, cm_wfd_atv d\r\nwhere a.atviid = b.id\r\n	 and b.prciid = c.id\r\n	 and b.atvdid = d.id\r\n	 and b.prciid = @prciid\r\n	 and (@atvdid = 0 or b.atvdid = @atvdid)\r\norder by a.dispidx',NULL),('流程-编辑活动授权','select\r\n	a.*,\r\n	b.name as role \r\nfrom\r\n	cm_wfd_atv_role a,\r\n	cm_role b \r\nwhere\r\n	a.roleid = b.id \r\n	and atvid in ( select id from cm_wfd_atv where prcid = @prcid )',NULL),('流程-编辑活动模板','select\r\n	a.*,\r\n	( CASE execscope WHEN 0 THEN \'一组用户\' WHEN 1 THEN \'所有用户\' WHEN 2 THEN \'单个用户\' WHEN 3 THEN \'任一用户\' END ) execscope_dsp,\r\n	( CASE execlimit WHEN 0 THEN \'无限制\' WHEN 1 THEN \'前一活动的执行者\' WHEN 2 THEN \'前一活动的同部门执行者\' WHEN 3 THEN \'已完成活动的执行者\' WHEN 4 THEN \'已完成活动的同部门执行者\' END ) execlimit_dsp,\r\n	( CASE JOINKIND WHEN 0 THEN \'全部任务\' WHEN 1 THEN \'任一任务\' WHEN 2 THEN \'即时同步\' END ) joinkind_dsp,\r\n	( CASE transkind WHEN 0 THEN \'自由选择\' WHEN 1 THEN \'全部\' WHEN 2 THEN \'独占式选择\' END ) transkind_dsp,\r\n	( select name from cm_wfd_atv where id = a.execatvid ) as execatvid_dsp \r\nfrom\r\n	cm_wfd_atv a \r\nwhere\r\n	prcid = @prcid',NULL),('流程-编辑流程模板','select * from cm_wfd_prc where id=@prcid',NULL),('流程-编辑迁移模板','select * from cm_wfd_trs where prcid=@prcid',NULL),('流程-获取用户ID','select id from cm_user where name = @name',NULL),('流程-起始活动','select * from cm_wfd_atv where prcid=@prcid and type=1',NULL),('流程-迁移模板ID','select\r\n	ID \r\nfrom\r\n	cm_wfd_trs \r\nwhere\r\n	prcid = @prcid \r\n	and SrcAtvID = @SrcAtvID \r\n	and TgtAtvID = @TgtAtvID \r\n	and IsRollback = @IsRollback',NULL),('流程-重复名称','select count(*) from cm_wfd_prc where name=@name',NULL),('用户-关联分组','select id,name from cm_group a where exists ( select GroupID from cm_user_group b where a.ID = b.GroupID and UserID=@ReleatedID )',NULL),('用户-关联角色','select id,name from cm_role a	where exists ( select RoleID from cm_user_role b where a.ID = b.RoleID and UserID=@ReleatedID )',NULL),('用户-具有的权限','SELECT id, NAME\r\nFROM\r\n	(\r\n		SELECT DISTINCT ( b.id ),\r\n			b.NAME\r\n		FROM\r\n			cm_role_per a\r\n			LEFT JOIN cm_permission b ON a.perid = b.id \r\n		WHERE\r\n			EXISTS (\r\n					SELECT\r\n						roleid \r\n					FROM\r\n						cm_user_role c \r\n					WHERE\r\n						a.roleid = c.roleid \r\n						AND userid = @userid\r\n				  UNION\r\n					SELECT\r\n						roleid \r\n					FROM\r\n						cm_group_role d \r\n					WHERE\r\n						a.roleid = d.roleid \r\n						AND EXISTS ( SELECT groupid FROM cm_user_group e WHERE d.groupid = e.groupid AND e.userid = @userid ) \r\n			) \r\n			OR a.roleid = 1 \r\n	) t \r\nORDER BY\r\n	id',NULL),('用户-可访问的菜单','select id,name\r\n  from (select distinct (b.id), b.name, dispidx\r\n          from cm_role_menu a\r\n          left join cm_menu b\r\n            on a.menuid = b.id\r\n         where exists\r\n         (select roleid\r\n                  from cm_user_role c\r\n                 where a.roleid = c.roleid\r\n                   and userid = @userid\r\n					union\r\n					select roleid\r\n					        from cm_group_role d\r\n									where a.roleid = d.roleid\r\n									  and exists (select groupid from cm_user_group e where d.groupid=e.groupid and e.userid=@userid)\r\n					) or a.roleid=1\r\n			 ) t\r\n order by dispidx',NULL),('用户-未关联的分组','SELECT\r\n	id,\r\n  name \r\nFROM\r\n	cm_group a \r\nWHERE\r\n	NOT EXISTS ( SELECT GroupID FROM cm_user_group b WHERE a.ID = b.GroupID AND UserID = @ReleatedID )',NULL),('用户-未关联的角色','SELECT\r\n	a.id,\r\n	a.NAME \r\nFROM\r\n	cm_role a \r\nWHERE\r\n	NOT EXISTS ( SELECT RoleID FROM cm_user_role b WHERE a.ID = b.RoleID AND UserID = @ReleatedID ) \r\n	AND a.ID !=1',NULL),('用户-角色列表的用户','SELECT DISTINCT(userid) FROM cm_user_role where FIND_IN_SET(roleid, @roleid)',NULL),('用户-重复手机号','select count(id) from cm_user where phone=@phone',NULL),('登录-手机号获取用户','select * from cm_user where phone=@phone',NULL),('菜单-id菜单项','SELECT\r\n	a.*,\r\n	b.NAME parentname \r\nFROM\r\n	cm_menu a\r\n	LEFT JOIN cm_menu b ON a.parentid = b.id \r\nWHERE\r\n	a.id = @id',NULL),('菜单-关联的角色','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_role a \r\nWHERE\r\n	EXISTS ( SELECT RoleID FROM cm_role_menu b WHERE a.ID = b.RoleID AND MenuID = @ReleatedID )',NULL),('菜单-分组树','SELECT\r\n	id,\r\n	NAME,\r\n	parentid \r\nFROM\r\n	cm_menu \r\nWHERE\r\n	isgroup = 1 \r\nORDER BY\r\n	dispidx',NULL),('菜单-完整树','SELECT\r\n	id,\r\n	NAME,\r\n	parentid,\r\n	isgroup,\r\n	icon,\r\n	dispidx\r\nFROM\r\n	cm_menu \r\nORDER BY\r\n	dispidx',NULL),('菜单-是否有子菜单','select count(*) from cm_menu where parentid=@parentid',NULL),('菜单-未关联的角色','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_role a \r\nWHERE\r\n	NOT EXISTS ( SELECT RoleID FROM cm_role_menu b WHERE a.ID = b.RoleID AND MenuID = @ReleatedID )',NULL),('角色-关联用户','SELECT\r\n	id,\r\n	NAME,\r\n	phone \r\nFROM\r\n	cm_user a \r\nWHERE\r\n	EXISTS ( SELECT UserID FROM cm_user_role b WHERE a.ID = b.UserID AND RoleID = @ReleatedID ) \r\nORDER BY\r\nNAME',NULL),('角色-关联的分组','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_group a \r\nWHERE\r\n	EXISTS ( SELECT GroupID FROM cm_group_role b WHERE a.ID = b.GroupID AND RoleID = @ReleatedID )',NULL),('角色-关联的权限','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_permission a \r\nWHERE\r\n	EXISTS ( SELECT PerID FROM cm_role_per b WHERE a.ID = b.PerID AND RoleID = @ReleatedID )',NULL),('角色-关联的菜单','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_menu a \r\nWHERE\r\n	EXISTS ( SELECT MenuID FROM cm_role_menu b WHERE a.ID = b.MenuID AND RoleID = @ReleatedID ) \r\nORDER BY\r\n	dispidx',NULL),('角色-名称重复','select count(id) from cm_role where name=@name',NULL),('角色-未关联的分组','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_group a \r\nWHERE\r\n	NOT EXISTS ( SELECT GroupID FROM cm_group_role b WHERE a.ID = b.GroupID AND RoleID = @ReleatedID )',NULL),('角色-未关联的权限','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_permission a \r\nWHERE\r\n	NOT EXISTS ( SELECT PerID FROM cm_role_per b WHERE a.ID = b.PerID AND RoleID = @ReleatedID )',NULL),('角色-未关联的用户','SELECT\r\n	id,\r\n	NAME,\r\n	phone \r\nFROM\r\n	cm_user a \r\nWHERE\r\n	NOT EXISTS ( SELECT UserID FROM cm_user_role b WHERE a.ID = b.UserID AND RoleID = @ReleatedID ) \r\nORDER BY\r\nNAME',NULL),('角色-未关联的菜单','SELECT\r\n	id,\r\nNAME \r\nFROM\r\n	cm_menu a \r\nWHERE\r\n	isgroup = 0 \r\n	AND NOT EXISTS ( SELECT menuid FROM cm_role_menu b WHERE a.id = b.menuid AND roleid = @ReleatedID ) \r\nORDER BY\r\n	dispidx',NULL),('角色-系统角色','select * from cm_role where id < 1000',NULL),('选项-分类选项','SELECT a.*,b.Name as GroupName FROM cm_option a, cm_option_group b where a.GroupID=b.ID and a.GroupID=@ParentID order by Dispidx',NULL),('选项-分组名称重复','select count(*) from cm_option_group where name=@name',NULL),('选项-子项个数','SELECT count(*) FROM cm_option where groupid=@groupid',NULL);
/*!40000 ALTER TABLE `cm_sql` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_user`
--

DROP TABLE IF EXISTS `cm_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_user` (
  `ID` bigint(20) NOT NULL COMMENT '用户标识',
  `Phone` char(11) NOT NULL COMMENT '手机号，唯一',
  `Name` varchar(32) NOT NULL COMMENT '姓名',
  `Pwd` char(32) NOT NULL COMMENT '密码的md5',
  `Sex` tinyint(4) unsigned NOT NULL DEFAULT '1' COMMENT '#Gender#性别',
  `Photo` varchar(255) NOT NULL DEFAULT '' COMMENT '头像',
  `Expired` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否停用',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `idx_phone` (`Phone`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='系统用户';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_user`
--

LOCK TABLES `cm_user` WRITE;
/*!40000 ALTER TABLE `cm_user` DISABLE KEYS */;
INSERT INTO `cm_user` VALUES (1,'13511111111','Windows','af3303f852abeccd793068486a391626',1,'[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]',0,'2019-10-24 09:06:38','2023-03-16 08:35:39'),(2,'13522222222','安卓','b59c67bf196a4758191e42f76670ceba',2,'[[\"photo/2.jpg\",\"2\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]',0,'2019-10-24 13:03:19','2023-03-16 08:36:23'),(3,'13533333333','苹果','674f3c2c1a8a6f90461e8a66fb5550ba',1,'[[\"photo/3.jpg\",\"3\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]',0,'0001-01-01 00:00:00','2023-03-16 08:36:46');
/*!40000 ALTER TABLE `cm_user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_user_group`
--

DROP TABLE IF EXISTS `cm_user_group`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_user_group` (
  `UserID` bigint(20) NOT NULL COMMENT '用户标识',
  `GroupID` bigint(20) NOT NULL COMMENT '组标识',
  PRIMARY KEY (`UserID`,`GroupID`),
  KEY `fk_usergroup_groupid` (`GroupID`),
  KEY `fk_usergroup_userid` (`UserID`),
  CONSTRAINT `fk_usergroup_groupid` FOREIGN KEY (`GroupID`) REFERENCES `cm_group` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_usergroup_userid` FOREIGN KEY (`UserID`) REFERENCES `cm_user` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='用户一组多对多';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_user_group`
--

LOCK TABLES `cm_user_group` WRITE;
/*!40000 ALTER TABLE `cm_user_group` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_user_group` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_user_params`
--

DROP TABLE IF EXISTS `cm_user_params`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_user_params` (
  `UserID` bigint(20) NOT NULL COMMENT '用户标识',
  `ParamID` bigint(20) NOT NULL COMMENT '参数标识',
  `Value` varchar(255) NOT NULL COMMENT '参数值',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`UserID`,`ParamID`) USING BTREE,
  KEY `fk_userparams_userid` (`UserID`),
  KEY `fk_userparams_paramsid` (`ParamID`),
  CONSTRAINT `fk_userparams_paramsid` FOREIGN KEY (`ParamID`) REFERENCES `cm_params` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_userparams_userid` FOREIGN KEY (`UserID`) REFERENCES `cm_user` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='用户参数值';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_user_params`
--

LOCK TABLES `cm_user_params` WRITE;
/*!40000 ALTER TABLE `cm_user_params` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_user_params` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_user_role`
--

DROP TABLE IF EXISTS `cm_user_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_user_role` (
  `UserID` bigint(20) NOT NULL COMMENT '用户标识',
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`UserID`,`RoleID`) USING BTREE,
  KEY `fk_userrole_userid` (`UserID`),
  KEY `fk_userrole_roleid` (`RoleID`),
  CONSTRAINT `fk_userrole_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_userrole_userid` FOREIGN KEY (`UserID`) REFERENCES `cm_user` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='用户一角色多对多';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_user_role`
--

LOCK TABLES `cm_user_role` WRITE;
/*!40000 ALTER TABLE `cm_user_role` DISABLE KEYS */;
INSERT INTO `cm_user_role` VALUES (1,2),(2,2),(3,2);
/*!40000 ALTER TABLE `cm_user_role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfd_atv`
--

DROP TABLE IF EXISTS `cm_wfd_atv`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfd_atv` (
  `ID` bigint(20) NOT NULL COMMENT '活动标识',
  `PrcID` bigint(20) NOT NULL COMMENT '流程标识',
  `Name` varchar(64) NOT NULL COMMENT '活动名称，同时作为状态名称',
  `Type` tinyint(4) unsigned NOT NULL COMMENT '#WfdAtvType#活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动',
  `ExecScope` tinyint(4) unsigned NOT NULL COMMENT '#WfdAtvExecScope#执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户',
  `ExecLimit` tinyint(4) unsigned NOT NULL COMMENT '#WfdAtvExecLimit#执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者',
  `ExecAtvID` bigint(20) DEFAULT NULL COMMENT '在执行者限制为3或4时选择的活动',
  `AutoAccept` tinyint(1) NOT NULL COMMENT '是否自动签收，打开工作流视图时自动签收工作项',
  `CanDelete` tinyint(1) NOT NULL COMMENT '能否删除流程实例和业务数据，0否 1',
  `CanTerminate` tinyint(1) NOT NULL COMMENT '能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能',
  `CanJumpInto` tinyint(1) NOT NULL COMMENT '是否可作为跳转目标，0不可跳转 1可以',
  `TransKind` tinyint(4) unsigned NOT NULL COMMENT '#WfdAtvTransKind#当前活动的后续迁移方式 0:自由选择 1:并行 2:独占式选择',
  `JoinKind` tinyint(4) unsigned NOT NULL COMMENT '#WfdAtvJoinKind#同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '修改时间',
  PRIMARY KEY (`ID`),
  KEY `fk_wfdatv_prcid` (`PrcID`),
  CONSTRAINT `fk_wfdatv_prcid` FOREIGN KEY (`PrcID`) REFERENCES `cm_wfd_prc` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='活动模板';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfd_atv`
--

LOCK TABLES `cm_wfd_atv` WRITE;
/*!40000 ALTER TABLE `cm_wfd_atv` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_wfd_atv` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfd_atv_role`
--

DROP TABLE IF EXISTS `cm_wfd_atv_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfd_atv_role` (
  `AtvID` bigint(20) NOT NULL COMMENT '活动标识',
  `RoleID` bigint(20) NOT NULL COMMENT '角色标识',
  PRIMARY KEY (`AtvID`,`RoleID`),
  KEY `fk_wfdatvrole_roleid` (`RoleID`),
  CONSTRAINT `fk_wfdatvrole_atvid` FOREIGN KEY (`AtvID`) REFERENCES `cm_wfd_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfdatvrole_roleid` FOREIGN KEY (`RoleID`) REFERENCES `cm_role` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='活动授权';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfd_atv_role`
--

LOCK TABLES `cm_wfd_atv_role` WRITE;
/*!40000 ALTER TABLE `cm_wfd_atv_role` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_wfd_atv_role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfd_prc`
--

DROP TABLE IF EXISTS `cm_wfd_prc`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfd_prc` (
  `ID` bigint(20) NOT NULL COMMENT '流程标识',
  `Name` varchar(64) NOT NULL COMMENT '流程名称',
  `Diagram` varchar(21000) DEFAULT NULL COMMENT '流程图',
  `IsLocked` tinyint(1) NOT NULL COMMENT '锁定标志，0表未锁定；1表锁定，不能创建流程实例，已启动的流程实例继续执行',
  `Singleton` tinyint(1) NOT NULL COMMENT '同一时刻只允许有一个激活的流程实例，0表非单实例，1表单实例',
  `Note` varchar(255) DEFAULT NULL COMMENT '描述',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后修改时间',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='流程模板';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfd_prc`
--

LOCK TABLES `cm_wfd_prc` WRITE;
/*!40000 ALTER TABLE `cm_wfd_prc` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_wfd_prc` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfd_trs`
--

DROP TABLE IF EXISTS `cm_wfd_trs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfd_trs` (
  `ID` bigint(20) NOT NULL COMMENT '迁移标识',
  `PrcID` bigint(20) NOT NULL COMMENT '流程模板标识',
  `SrcAtvID` bigint(20) NOT NULL COMMENT '起始活动模板标识',
  `TgtAtvID` bigint(20) NOT NULL COMMENT '目标活动模板标识',
  `IsRollback` tinyint(1) NOT NULL COMMENT '是否为回退迁移',
  `TrsID` bigint(20) DEFAULT NULL COMMENT '类别为回退迁移时对应的常规迁移标识',
  PRIMARY KEY (`ID`),
  KEY `fk_wfdtrs_prcid` (`PrcID`),
  CONSTRAINT `fk_wfdtrs_prcid` FOREIGN KEY (`PrcID`) REFERENCES `cm_wfd_prc` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='迁移模板';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfd_trs`
--

LOCK TABLES `cm_wfd_trs` WRITE;
/*!40000 ALTER TABLE `cm_wfd_trs` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_wfd_trs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfi_atv`
--

DROP TABLE IF EXISTS `cm_wfi_atv`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfi_atv` (
  `ID` bigint(20) NOT NULL COMMENT '活动实例标识',
  `PrciID` bigint(20) NOT NULL COMMENT '流程实例标识',
  `AtvdID` bigint(20) NOT NULL COMMENT '活动模板标识',
  `Status` tinyint(4) unsigned NOT NULL COMMENT '#WfiAtvStatus#活动实例的状态 0活动 1结束 2终止 3同步活动',
  `InstCount` int(11) NOT NULL COMMENT '活动实例在流程实例被实例化的次数',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`ID`),
  KEY `fk_wfiatv_prciid` (`PrciID`),
  KEY `fk_wfiatv_atvdid` (`AtvdID`),
  CONSTRAINT `fk_wfiatv_atvdid` FOREIGN KEY (`AtvdID`) REFERENCES `cm_wfd_atv` (`ID`),
  CONSTRAINT `fk_wfiatv_prciid` FOREIGN KEY (`PrciID`) REFERENCES `cm_wfi_prc` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='活动实例';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfi_atv`
--

LOCK TABLES `cm_wfi_atv` WRITE;
/*!40000 ALTER TABLE `cm_wfi_atv` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_wfi_atv` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfi_item`
--

DROP TABLE IF EXISTS `cm_wfi_item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfi_item` (
  `ID` bigint(20) NOT NULL COMMENT '工作项标识',
  `AtviID` bigint(20) NOT NULL COMMENT '活动实例标识',
  `Status` tinyint(4) unsigned NOT NULL COMMENT '#WfiItemStatus#工作项状态 0活动 1结束 2终止 3同步活动',
  `AssignKind` tinyint(4) unsigned NOT NULL COMMENT '#WfiItemAssignKind#指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派',
  `Sender` varchar(32) NOT NULL COMMENT '发送者',
  `Stime` datetime NOT NULL COMMENT '发送时间',
  `IsAccept` tinyint(1) NOT NULL COMMENT '是否签收此项任务',
  `AcceptTime` datetime DEFAULT NULL COMMENT '签收时间',
  `RoleID` bigint(20) DEFAULT NULL COMMENT '执行者角色标识',
  `UserID` bigint(20) DEFAULT NULL COMMENT '执行者用户标识',
  `Note` varchar(255) DEFAULT NULL COMMENT '工作项备注',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`ID`),
  KEY `fk_wfiitem_atviid` (`AtviID`),
  CONSTRAINT `fk_wfiitem_atviid` FOREIGN KEY (`AtviID`) REFERENCES `cm_wfi_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='工作项';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfi_item`
--

LOCK TABLES `cm_wfi_item` WRITE;
/*!40000 ALTER TABLE `cm_wfi_item` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_wfi_item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfi_prc`
--

DROP TABLE IF EXISTS `cm_wfi_prc`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfi_prc` (
  `ID` bigint(20) NOT NULL COMMENT '流程实例标识，同时为业务数据主键',
  `PrcdID` bigint(20) NOT NULL COMMENT '流程模板标识',
  `Name` varchar(255) NOT NULL COMMENT '流转单名称',
  `Status` tinyint(4) unsigned NOT NULL COMMENT '#WfiPrcStatus#流程实例状态 0活动 1结束 2终止',
  `Dispidx` int(11) NOT NULL COMMENT '显示顺序',
  `Ctime` datetime NOT NULL COMMENT '创建时间',
  `Mtime` datetime NOT NULL COMMENT '最后一次状态改变的时间',
  PRIMARY KEY (`ID`),
  KEY `fk_wfiprc_prcdid` (`PrcdID`),
  CONSTRAINT `fk_wfiprc_prcdid` FOREIGN KEY (`PrcdID`) REFERENCES `cm_wfd_prc` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='流程实例';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfi_prc`
--

LOCK TABLES `cm_wfi_prc` WRITE;
/*!40000 ALTER TABLE `cm_wfi_prc` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_wfi_prc` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cm_wfi_trs`
--

DROP TABLE IF EXISTS `cm_wfi_trs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cm_wfi_trs` (
  `ID` bigint(20) NOT NULL COMMENT '迁移实例标识',
  `TrsdID` bigint(20) NOT NULL COMMENT '迁移模板标识',
  `SrcAtviID` bigint(20) NOT NULL COMMENT '起始活动实例标识',
  `TgtAtviID` bigint(20) NOT NULL COMMENT '目标活动实例标识',
  `IsRollback` tinyint(1) NOT NULL COMMENT '是否为回退迁移，1表回退',
  `Ctime` datetime NOT NULL COMMENT '迁移时间',
  PRIMARY KEY (`ID`),
  KEY `fk_wfitrs_trsdid` (`TrsdID`),
  KEY `fk_wfitrs_srcatviid` (`SrcAtviID`),
  KEY `fk_wfitrs_tgtatviid` (`TgtAtviID`),
  CONSTRAINT `fk_wfitrs_srcatviid` FOREIGN KEY (`SrcAtviID`) REFERENCES `cm_wfi_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfitrs_tgtatviid` FOREIGN KEY (`TgtAtviID`) REFERENCES `cm_wfi_atv` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_wfitrs_trsdid` FOREIGN KEY (`TrsdID`) REFERENCES `cm_wfd_trs` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='迁移实例';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cm_wfi_trs`
--

LOCK TABLES `cm_wfi_trs` WRITE;
/*!40000 ALTER TABLE `cm_wfi_trs` DISABLE KEYS */;
/*!40000 ALTER TABLE `cm_wfi_trs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `fsm_file`
--

DROP TABLE IF EXISTS `fsm_file`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fsm_file` (
  `ID` bigint(20) unsigned NOT NULL COMMENT '文件标识',
  `Name` varchar(512) NOT NULL COMMENT '文件名称',
  `Path` varchar(512) NOT NULL COMMENT '存放路径：卷/两级目录/id.ext',
  `Size` bigint(20) unsigned NOT NULL COMMENT '文件长度',
  `Info` varchar(512) DEFAULT NULL COMMENT '文件描述',
  `Uploader` bigint(20) unsigned NOT NULL COMMENT '上传人id',
  `Ctime` datetime NOT NULL COMMENT '上传时间',
  `Downloads` bigint(20) unsigned NOT NULL COMMENT '下载次数',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `idx_fsm_file_path` (`Path`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fsm_file`
--

LOCK TABLES `fsm_file` WRITE;
/*!40000 ALTER TABLE `fsm_file` DISABLE KEYS */;
/*!40000 ALTER TABLE `fsm_file` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `fsm_sql`
--

DROP TABLE IF EXISTS `fsm_sql`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fsm_sql` (
  `id` varchar(128) NOT NULL COMMENT 'sql键值',
  `sql` varchar(20000) NOT NULL COMMENT 'sql内容',
  `note` varchar(255) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fsm_sql`
--

LOCK TABLES `fsm_sql` WRITE;
/*!40000 ALTER TABLE `fsm_sql` DISABLE KEYS */;
INSERT INTO `fsm_sql` VALUES ('上传文件','INSERT INTO fsm_file ( id, NAME, path, size, uploader, info, ctime, downloads )\r\nVALUES\r\n	( @id, @NAME, @path, @size, @uploader, @info, now( ), 0 )',NULL),('增加下载次数','update fsm_file set downloads=downloads+1 where path=@path',NULL);
/*!40000 ALTER TABLE `fsm_sql` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `msg_sql`
--

DROP TABLE IF EXISTS `msg_sql`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `msg_sql` (
  `id` varchar(128) NOT NULL COMMENT 'sql键值',
  `sql` varchar(20000) NOT NULL COMMENT 'sql内容',
  `note` varchar(255) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `msg_sql`
--

LOCK TABLES `msg_sql` WRITE;
/*!40000 ALTER TABLE `msg_sql` DISABLE KEYS */;
/*!40000 ALTER TABLE `msg_sql` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sequence`
--

DROP TABLE IF EXISTS `sequence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sequence` (
  `id` varchar(64) NOT NULL COMMENT '序列名称',
  `val` int(11) NOT NULL COMMENT '序列的当前值',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='模拟Sequence';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sequence`
--

LOCK TABLES `sequence` WRITE;
/*!40000 ALTER TABLE `sequence` DISABLE KEYS */;
INSERT INTO `sequence` VALUES ('cm_menu+dispidx',89),('cm_option+dispidx',1031),('cm_pub_post+dispidx',167),('cm_wfd_prc+dispidx',11),('cm_wfi_item+dispidx',176),('cm_wfi_prc+dispidx',65);
/*!40000 ALTER TABLE `sequence` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'initdb'
--

--
-- Dumping routines for database 'initdb'
--
/*!50003 DROP FUNCTION IF EXISTS `nextval` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`dt`@`%` FUNCTION `nextval`( v_seq_name VARCHAR ( 200 ) ) RETURNS int(11)
BEGIN
DECLARE
	res INTEGER;
UPDATE sequence 
SET val = val + 1 
WHERE
	id = v_seq_name;

SET res = 0;
SELECT
	val INTO res 
FROM
	sequence 
WHERE
	id = v_seq_name;
RETURN res;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-04-26 14:18:24
