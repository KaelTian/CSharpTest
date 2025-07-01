namespace ExcelLoader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var converter = new ExcelToPlcConfigConverter();

                //////// 1. 解析Excel
                ////var configs = converter.GetPlcDataMappingConfigs(
                ////   filePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"D:\works\005\005-backup\02.文档\3.相关数据和文档\01捷佳\MES数采数据明细-捷佳.xlsx"),
                ////   targetWorksheet: "变量表",
                ////   startRow: 2,
                ////   plcPointNameColumn: 1);

                //////// 2. 保存为JSON
                ////converter.SaveAsJson(configs, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlcDataMapping.json"));

                ////Console.WriteLine("转换完成！");

                ////converter.ParseJieJiaExcel(@"D:\works\005\005-backup\02.文档\3.相关数据和文档\01捷佳\MES数采数据明细-捷佳.xlsx");

                //converter.GenerateJJSystemSettingsConfig(
                //    filePath: @"D:\works\005\005-backup\02.文档\3.相关数据和文档\01捷佳\捷佳参数表.xlsx",
                //    outputPath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JJSystemSettingsConfig.json")
                //    );

                // 使用示例
                //MysqlTableGenerator.GenerateFromJsonFile(
                //    inputJsonPath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JJSystemSettingsConfig.json"),
                //    outputSqlPath: "create_table.sql",
                //    tableName: "jj_plc_system_settings"
                //);

                //converter.GenerateJJ_TableScript(
                //    excelPath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"D:\works\005\005-backup\02.文档\3.相关数据和文档\01捷佳\MES数采数据明细-捷佳.xlsx"),
                //    dbName: "005_mes",
                //    tableName: "jj_plc_all_points",
                //    outputPath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jj_plc_all_points.sql"));

                //converter.GenerateJJAlarmsConfig(
                //    filePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"D:\works\005\005-backup\02.文档\3.相关数据和文档\01捷佳\捷佳报警明细.xlsx"),
                //    outputPath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JJ_Alarms_Config.json"));

                //converter.GenerateJJ_Alarm_TableScript(
                //    excelPath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"D:\works\005\005-backup\02.文档\3.相关数据和文档\01捷佳\捷佳报警明细.xlsx"),
                //    dbName: "005_mes",
                //    tableName: "jj_plc_alarm",
                //    outputPath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jj_plc_alarm.sql"));

                //converter.GenerateJJAlarmsDBMappingConfig(
                //    filePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"D:\works\005\005-backup\02.文档\3.相关数据和文档\01捷佳\捷佳报警明细.xlsx"),
                //    outputPath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlcDBMapping_JJ_Alarms.json"));


                //converter.GenerateJJAlarmsConfig(
                //    filePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"D:\works\005\005-backup\02.文档\3.相关数据和文档\01捷佳\捷佳报警明细.xlsx"),
                //    outputPath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JJ_Alarms_Config.json"));

                #region 欣奕华上下料单元

                {
                    var xinyihuaConverter = new XinYiHuaLoadingAndUnloadingConverter();
                    // 配置Json转换成实体类
                    xinyihuaConverter.ConvertToClassFromJson(
                        jsonFilePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"D:\works\005\latest-codes\03.开发\Collect\Start\bin\Debug\net8.0\Line7\Melsec_MES.json"),
                        outputFilePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelsecXinYiHuaLoadingAndUnloading.cs")
                    );
                    //// 从Excel文件解析PLC数据映射配置并保存为JSON文件
                    //xinyihuaConverter.ParsePlcDataMappingFromExcel(
                    //    excelFilePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"D:\works\005\latest-codes\02.文档\3.相关数据和文档\02欣奕华\上下料物流线MES交互.xlsx"),
                    //    outputFilePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlcDataMapping_XYH_LoadingAndUnloading.json")
                    //);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
        }
    }
}
