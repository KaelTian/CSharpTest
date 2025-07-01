namespace ExcelLoader
{
    using System;
    using System.Text.RegularExpressions;

    public class TypeConverter
    {
        public static string GetColumnType(string type)
        {
            // 先处理明确的简单类型
            switch (type)
            {
                case "INT":
                    return "int DEFAULT NULL";
                case "REAL":
                    return "float DEFAULT NULL";
                case "BOOL":
                    return "bit(1) DEFAULT b'0'";
            }

            // 处理 STRING[length] 格式
            var stringMatch = Regex.Match(type, @"^STRING\[(\d+)\]$");
            if (stringMatch.Success)
            {
                return $"varchar({stringMatch.Groups[1].Value}) DEFAULT NULL";
            }

            // 处理 ARRAY[range] OF type 格式
            var arrayMatch = Regex.Match(type, @"^ARRAY\[(\d+)\.\.(\d+)\] OF (BOOL|REAL|INT|STRING(\[\d+\])?)$");
            if (arrayMatch.Success)
            {
                var start = int.Parse(arrayMatch.Groups[1].Value);
                var end = int.Parse(arrayMatch.Groups[2].Value);
                var elementType = arrayMatch.Groups[3].Value;

                // 计算数组长度
                int length = end - start + 1;

                // 转换元素类型
                string sqlElementType = GetColumnType(elementType);

                // 返回数组类型 - 这里假设你使用JSON存储数组
                return $"json DEFAULT NULL COMMENT 'Array of {length} {sqlElementType}'";
            }

            throw new Exception($"Unsupported type: {type}");
        }

        // 测试代码
        public static void Test()
        {
            Console.WriteLine(GetColumnType("INT"));                  // int DEFAULT NULL
            Console.WriteLine(GetColumnType("STRING[127]"));          // varchar(127) DEFAULT NULL
            Console.WriteLine(GetColumnType("ARRAY[0..999] OF BOOL")); // json DEFAULT NULL COMMENT 'Array of 1000 bit(1) DEFAULT b'0''
            Console.WriteLine(GetColumnType("ARRAY[0..4] OF REAL"));   // json DEFAULT NULL COMMENT 'Array of 5 float DEFAULT NULL'
        }
    }
}
