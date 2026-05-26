# MySQL 替代 Redis 说明

## 架构变更

### 之前
```
IRedisService (StackExchange.Redis)
  └── RedisService ──→ Redis Server
```

### 现在
```
IRedisService (同一接口)
  └── MySqlStorageService ──→ MySQL (mes_prod.mes_kv_store)
```

**业务代码零改动** — 所有使用 `IRedisService` 的地方无需修改。

## 数据存储映射

| Redis 类型 | MySQL 存储方式 | 示例 key |
|-----------|--------------|----------|
| String | `value` 直接存 JSON 字符串 | `mes:user:10001` |
| Hash | `value` 存 `{"field1":"val1"}` JSON | `mes:role:admin` |
| List | `value` 存 `["val1","val2"]` JSON 数组 | `mes:operation:LOT-001` |
| Set | `value` 存 `["member1","member2"]` JSON 数组 | `mes:wo:index` |

## 快速启动

### 1. 安装 MySQL

```bash
# Windows 推荐使用 Chocolatey
choco install mysql
# 或从官网下载 https://dev.mysql.com/downloads/

# 启动 MySQL 服务
net start MySQL
```

### 2. 初始化数据库

```bash
# 方法 1：命令行
mysql -u root < docs/mysql_init.sql

# 方法 2：如果设置了密码
mysql -u root -p < docs/mysql_init.sql
```

### 3. 修改连接字符串

客户端：[App.xaml.cs](file:///e:/AiProj/MES_NEW/src/Client/MES.Shell/App.xaml.cs#L116)

```csharp
var mysqlConnStr = "Server=localhost;Database=mes_prod;Uid=root;Pwd=你的密码;";
```

服务端：[Program.cs](file:///e:/AiProj/MES_NEW/src/Server/MES.Api/Program.cs) 或 `appsettings.json`

```json
{
  "ConnectionStrings": {
    "MesDb": "Server=localhost;Database=mes_prod;Uid=root;Pwd=你的密码;"
  }
}
```

### 4. 运行

```bash
# 客户端
dotnet run --project src/Client/MES.Shell/MES.Shell.csproj

# 服务端 API
dotnet run --project src/Server/MES.Api/MES.Api.csproj
```

## 数据库表结构

```sql
mes_prod
└── mes_kv_store
    ├── key (VARCHAR 255, PK)   -- Redis key，如 "mes:user:10001"
    ├── value (LONGTEXT)        -- JSON 值
    ├── data_type (VARCHAR 20)  -- string/hash/list/set
    ├── created_at (DATETIME)
    └── updated_at (DATETIME)
```

## 查询示例

```sql
-- 查看所有用户
SELECT `key`, SUBSTRING(`value`, 1, 100) as data 
FROM mes_kv_store 
WHERE `key` LIKE 'mes:user:%';

-- 查看所有批次
SELECT `key`, JSON_EXTRACT(`value`, '$.LotId') as lot_id, 
       JSON_EXTRACT(`value`, '$.Status') as status
FROM mes_kv_store 
WHERE `key` LIKE 'mes:lot:%';

-- 查看工单列表
SELECT `value` FROM mes_kv_store WHERE `key` = 'mes:wo:index';

-- 清空所有数据（慎用）
DELETE FROM mes_kv_store;
```
