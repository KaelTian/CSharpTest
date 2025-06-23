namespace ExcelLoader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var converter = new ExcelToPlcConfigConverter();

                //// 1. 解析Excel
                //var configs = converter.ParseExcel(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"D:\works\005\latest-codes\02.文档\3.相关数据和文档\MES数采数据明细.xlsx"));

                //// 2. 保存为JSON
                //converter.SaveAsJson(configs, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "捷佳主机_config.json"));

                //Console.WriteLine("转换完成！");

                converter.ParseJieJiaExcel(@"D:\works\005\005-backup\02.文档\3.相关数据和文档\01捷佳\MES数采数据明细-捷佳.xlsx");
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
