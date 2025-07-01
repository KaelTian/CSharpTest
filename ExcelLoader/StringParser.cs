namespace ExcelLoader
{
    using System;
    using System.Text.RegularExpressions;

    public class StringParser
    {
        /// <summary>
        /// 将类似 "STRING[127]" 的字符串转换为 "varchar(127)"
        /// </summary>
        /// <param name="input">输入字符串，如 "STRING[127]"</param>
        /// <returns>转换后的字符串，如 "varchar(127)"</returns>
        public static string ConvertToVarchar(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("输入字符串不能为空或空白");
            }

            // 使用正则表达式匹配模式：任意字符后跟[数字]
            var match = Regex.Match(input, @"^(.+?)\[(\d+)\]$");

            if (!match.Success)
            {
                throw new ArgumentException("输入字符串格式不正确，应为类似 'STRING[127]' 的格式");
            }

            // 获取数字部分
            string length = match.Groups[2].Value;

            // 构造 varchar 类型字符串
            return $"varchar({length})";
        }
    }
}
