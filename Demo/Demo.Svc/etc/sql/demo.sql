-- @key: sql无参数
SELECT * FROM crud_基础

-- @key: 含参数sql
SELECT * FROM crud_基础
WHERE id = @id;

-- @key: 含格式化串sql
SELECT * FROM crud_基础
WHERE {0};

-- @key: 含格式化串含参数sql
SELECT * FROM crud_基础
WHERE 序列>@序列 and 名称 like {0};

-- @key: GetUserById
/*前置多行注释
删掉*/
--前置单行注释删掉
SELECT * FROM [User] 
WHERE Id = @Id; /*中间注释保留*/ -- 行尾注释删掉
/*末尾整块注释删掉*/

-- @key: QueryDemo
--前置注释删掉
SELECT 'abc--123' AS Col /*中间注释保留*/ FROM User; --末尾注释