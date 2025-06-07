using CoreLibrary.Component;
using CoreLibrary.Interface;
using Dm.util;
using SpliteTableDBComponentDemo.Entities;
using SqlSugar;

namespace SpliteTableDBComponentDemo
{
    public enum ProgramLogLevel
    {
        INFO,
        ERROR,
        DEBUG
    }
    internal class Program
    {
        //static SpliteTableDBComponent? splitComponent;
        static DbComponent? dbComponent;
        static void Main(string[] args)
        {
            try
            {
                dbComponent = new SpliteTableDBComponent();
                //dbComponent = new DbComponent();
                dbComponent.DbType = SqlSugar.DbType.MySql;
                dbComponent.ConnectionString = "server=192.168.0.122;Database=005_split_db;Uid=root;Pwd=root;AllowLoadLocalInfile=true";
                dbComponent.Start();
                DateTime startDate = new DateTime(2020, 1, 1);
                DateTime endDate = new DateTime(2026, 1, 1);
                int count = 10000;
                RemoveSplitTables<DbAutoIncreaseIdentity>(dbComponent);
                GenerateTestSplitAutoIncreaseIdentityData(dbComponent, count, startDate, endDate);
                var datas = QuerySplitDatas<DbAutoIncreaseIdentity>(dbComponent, startDate, endDate);
                //GenerateTestData(dbComponent, count, startDate, endDate);
                //GenerateDb4000TestData(dbComponent);
            }
            catch (Exception ex)
            {
                LogMessage($"分表测试异常: {ex}", ProgramLogLevel.ERROR);
            }

            Console.WriteLine("Press Any Key to exit.");
            Console.Read();
        }


        private static void GenerateTestSplitAutoIncreaseIdentityData(DbComponent dbComponent, int count, DateTime startDate, DateTime endDate)
        {
            try
            {
                var random = new Random();
                var dateRange = (endDate.Year - startDate.Year);
                RandomDateTimeGenerator randomDateTimeGenerator = new RandomDateTimeGenerator();
                var paymentMethods = new[] { "CreditCard", "PayPal", "Alipay", "WeChatPay", "BankTransfer" };
                var list = new List<DbAutoIncreaseIdentity>();

                for (int i = 0; i < count; i++)
                {
                    var createTime = randomDateTimeGenerator.GenerateRandomDateTime(startDate.Year + random.Next(dateRange));
                    var amount = (decimal)random.Next(10, 10000) + (decimal)random.NextDouble();
                    var customerId = random.Next(1, 100000);
                    var status = random.Next(0, 5);
                    var id = SnowFlakeSingle.Instance.NextId();
                    var data = new DbAutoIncreaseIdentity
                    {
                        Id = id,// Id 字段会自动增长，所以不需要手动设置
                        OrderNumber = $"ORD{DateTime.Now:yyyyMMddHHmmssfff}{random.Next(1000, 9999)}",
                        CreateTime = createTime,
                        Amount = amount,
                        CustomerName = $"Customer{random.Next(1, 10000)}",
                        CustomerPhone = $"1{random.Next(100000000, 999999999)}",
                        Status = status,
                        Address = $"{random.Next(1, 1000)} Main St, City{random.Next(1, 100)}",
                        CustomerId = customerId,
                        PaymentMethod = paymentMethods[random.Next(0, paymentMethods.Length)]
                    };

                    list.Add(data);
                }

                int result = dbComponent.Insert(list);
                if (result > 0)
                {
                    LogMessage($"生成DbAutoIncreaseIdentity测试数据成功, 共插入{result}条数据。");
                }
                else
                {
                    LogMessage($"生成DbAutoIncreaseIdentity测试数据失败。", ProgramLogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"生成DbAutoIncreaseIdentity测试数据异常: " + ex.ToString(), ProgramLogLevel.ERROR);
                throw;
            }
        }

        private static List<T> QuerySplitDatas<T>(DbComponent dbComponent, DateTime startDate, DateTime endDate)
            where T : class, new()
        {
            try
            {
                var client = dbComponent.Client;
                var datas = client.Queryable<T>()
                    .SplitTable(startDate, endDate)
                    .ToList();
                return datas;
            }
            catch (Exception ex)
            {
                LogMessage($"检索 {typeof(T)} 数据异常: " + ex.ToString(), ProgramLogLevel.ERROR);
                throw;
            }
        }

        private static void RemoveSplitTables<T>(DbComponent dbComponent)
             where T : class, new()
        {
            try
            {
                var client = dbComponent.Client;
                var splitTables = client.SplitHelper<T>().GetTables();
                foreach (var splitTable in splitTables)
                {
                    if (client.DbMaintenance.IsAnyTable(splitTable.TableName, false))
                    {
                        client.DbMaintenance.DropTable(splitTable.TableName);
                        LogMessage($"已删除表: {splitTable.TableName}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"RemoveTables 执行异常: {ex}");
            }
        }

        private static void GenerateTestSplitData(DbComponent dbComponent, int count, DateTime startDate, DateTime endDate)
        {
            try
            {
                var random = new Random();
                var dateRange = (endDate.Year - startDate.Year);
                RandomDateTimeGenerator randomDateTimeGenerator = new RandomDateTimeGenerator();
                var paymentMethods = new[] { "CreditCard", "PayPal", "Alipay", "WeChatPay", "BankTransfer" };
                var db4000List = new List<db4080>();

                for (int i = 0; i < count; i++)
                {
                    var createTime = randomDateTimeGenerator.GenerateRandomDateTime(startDate.Year + random.Next(dateRange));
                    var amount = (decimal)random.Next(10, 10000) + (decimal)random.NextDouble();
                    var customerId = random.Next(1, 100000);
                    var status = random.Next(0, 5);
                    var id = SnowFlakeSingle.Instance.NextId();
                    // 插入带索引的表
                    var db400 = new db4080
                    {
                        Id = id,
                        OrderNumber = $"ORD{DateTime.Now:yyyyMMddHHmmssfff}{random.Next(1000, 9999)}",
                        CreateTime = createTime,
                        Amount = amount,
                        CustomerName = $"Customer{random.Next(1, 10000)}",
                        CustomerPhone = $"1{random.Next(100000000, 999999999)}",
                        Status = status,
                        Address = $"{random.Next(1, 1000)} Main St, City{random.Next(1, 100)}",
                        CustomerId = customerId,
                        PaymentMethod = paymentMethods[random.Next(0, paymentMethods.Length)]
                    };

                    db4000List.Add(db400);
                }

                int result = dbComponent.Insert(db4000List);
                if (result > 0)
                {
                    LogMessage($"生成db4000测试数据成功, 共插入{result}条数据。");
                }
                else
                {
                    LogMessage($"生成db4000测试数据失败。", ProgramLogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"生成db4000测试数据异常: " + ex.ToString(), ProgramLogLevel.ERROR);
                throw;
            }
        }

        private static void GenerateDb4000TestData(DbComponent dbComponent)
        {
            // Generate test data
            var testData = TestDataGenerator.GenerateTestData(100); // Generate 100 test records

            int result = dbComponent.Insert(testData, isReturnIdentity: true);
            if (result > 0)
            {
                LogMessage($"生成Db4000测试数据成功, 共插入{result}条数据。");
            }
            else
            {
                LogMessage($"生成Db4000测试数据失败。", ProgramLogLevel.ERROR);
            }
        }

        private static void GenerateTestData(DbComponent dbComponent, int count, DateTime startDate, DateTime endDate)
        {
            try
            {
                // 初始化实体表格（Initialize entity tables）
                dbComponent.Client.CodeFirst.InitTables<db5000>();
                var random = new Random();
                var dateRange = (endDate.Year - startDate.Year);
                RandomDateTimeGenerator randomDateTimeGenerator = new RandomDateTimeGenerator();
                var paymentMethods = new[] { "CreditCard", "PayPal", "Alipay", "WeChatPay", "BankTransfer" };
                var db5000List = new List<db5000>();

                for (int i = 0; i < count; i++)
                {
                    var createTime = randomDateTimeGenerator.GenerateRandomDateTime(startDate.Year + random.Next(dateRange));
                    var amount = (decimal)random.Next(10, 10000) + (decimal)random.NextDouble();
                    var customerId = random.Next(1, 100000);
                    var status = random.Next(0, 5);
                    var id = i + 10;
                    // 插入带索引的表
                    var db5000 = new db5000
                    {
                        //Id = id,
                        OrderNumber = $"ORD{DateTime.Now:yyyyMMddHHmmssfff}{random.Next(1000, 9999)}",
                        CreateTime = createTime,
                        Amount = amount,
                        CustomerName = $"Customer{random.Next(1, 10000)}",
                        CustomerPhone = $"1{random.Next(100000000, 999999999)}",
                        Status = status,
                        Address = $"{random.Next(1, 1000)} Main St, City{random.Next(1, 100)}",
                        CustomerId = customerId,
                        PaymentMethod = paymentMethods[random.Next(0, paymentMethods.Length)]
                    };

                    db5000List.Add(db5000);
                }

                int result = dbComponent.Insert(db5000List, isReturnIdentity: true);
                if (result > 0)
                {
                    LogMessage($"生成db5000测试数据成功, 共插入{result}条数据。");
                }
                else
                {
                    LogMessage($"生成db5000测试数据失败。", ProgramLogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"生成db5000测试数据异常: " + ex.ToString(), ProgramLogLevel.ERROR);
                throw;
            }
        }

        static void LogMessage(string message, ProgramLogLevel logLevel = ProgramLogLevel.INFO)
        {
            // to do : 后期可以添加日志记录功能
            Console.WriteLine($"[{logLevel}] {message}");
        }

        static void InsertTest()
        {

        }
    }
}
