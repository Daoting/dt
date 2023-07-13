/*
 Navicat Premium Data Transfer

 Source Server         : server2
 Source Server Type    : MySQL
 Source Server Version : 50721 (5.7.21-log)
 Source Host           : 10.10.1.2:3306
 Source Schema         : dt

 Target Server Type    : Oracle
 Target Server Version : 110100
 File Encoding         : 65001

 Date: 03/07/2023 16:50:34
*/


-- ----------------------------
-- Table structure for cm_file_my
-- ----------------------------
CREATE TABLE "CM_FILE_MY" (
  "ID" NUMBER(20) ,
  "PARENTID" NUMBER(20),
  "NAME" NVARCHAR2(255) ,
  "ISFOLDER" NUMBER(4) ,
  "EXTNAME" NVARCHAR2(8),
  "INFO" NVARCHAR2(512) ,
  "CTIME" DATE ,
  "USERID" NUMBER(20) 
)
;
COMMENT ON COLUMN "CM_FILE_MY"."ID" IS '文件标识';
COMMENT ON COLUMN "CM_FILE_MY"."PARENTID" IS '上级目录，根目录的parendid为空';
COMMENT ON COLUMN "CM_FILE_MY"."NAME" IS '名称';
COMMENT ON COLUMN "CM_FILE_MY"."ISFOLDER" IS '是否为文件夹';
COMMENT ON COLUMN "CM_FILE_MY"."EXTNAME" IS '文件扩展名';
COMMENT ON COLUMN "CM_FILE_MY"."INFO" IS '文件描述信息';
COMMENT ON COLUMN "CM_FILE_MY"."CTIME" IS '创建时间';
COMMENT ON COLUMN "CM_FILE_MY"."USERID" IS '所属用户';
COMMENT ON TABLE "CM_FILE_MY" IS '个人文件';
