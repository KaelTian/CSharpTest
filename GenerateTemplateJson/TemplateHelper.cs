namespace GenerateTemplateJson
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class TemplateHelper
    {
        public static JObject FillTemplateWithTypedValues(string template, Dictionary<string, object> values)
        {
            var jObj = JObject.Parse(template);

            void ReplaceTokens(JToken token, Dictionary<string, object> values)
            {
                if (token.Type == JTokenType.Object)
                {
                    foreach (var property in ((JObject)token).Properties())
                    {
                        ReplaceTokens(property.Value, values);
                    }
                }
                else if (token.Type == JTokenType.Array)
                {
                    foreach (var item in (JArray)token)
                    {
                        ReplaceTokens(item, values);
                    }
                }
                else if (token.Type == JTokenType.String)
                {
                    var str = token.ToString();

                    // 新增：尝试判断这个字符串是否也是个合法 JSON，如果是就再递归进去替换
                    if (IsJson(str))
                    {
                        var innerToken = JToken.Parse(str);
                        ReplaceTokens(innerToken, values);
                        token.Replace(innerToken.ToString(Formatting.None)); // 注意转回字符串
                        return;
                    }

                    var match = Regex.Match(str, @"\{\{(\w+)\}\}");
                    while (match.Success)
                    {
                        var key = match.Groups[1].Value;
                        if (values.TryGetValue(key, out var val))
                        {
                            if (str == $"{{{{{key}}}}}")
                            {
                                token.Replace(JToken.FromObject(val));
                            }
                            else
                            {
                                str = str.Replace($"{{{{{key}}}}}", val?.ToString() ?? "");
                                token.Replace(str);
                            }
                        }

                        match = match.NextMatch();
                    }
                }
            }


            ReplaceTokens(jObj, values);
            return jObj;

            bool IsJson(string str)
            {
                str = str.Trim();
                if ((str.StartsWith("{") && str.EndsWith("}")) ||
                    (str.StartsWith("[") && str.EndsWith("]")))
                {
                    try
                    {
                        JToken.Parse(str);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            }

        }
    }

}
