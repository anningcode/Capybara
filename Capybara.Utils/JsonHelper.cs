using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Capybara.Utils
{
    /// <summary>JSON 验证与补全工具，用于修复大模型输出被截断的 JSON</summary>
    public static class JsonHelper
    {
        /// <summary>验证字符串是否为合法 JSON</summary>
        public static bool IsValidJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            try
            {
                JToken.Parse(input);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        /// <summary>尝试补全不完整的 JSON（如截断的 }] 和字符串）</summary>
        public static bool TryCompleteJson(string input, out string? fixedJson)
        {
            fixedJson = null;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();

            try
            {
                JToken.Parse(input);
                fixedJson = input;
                return true;
            }
            catch (JsonException)
            {
                // 需要修复
            }

            var result = input;

            // 修复被截断的字符串值：以未闭合的双引号结尾
            result = FixTruncatedString(result);

            // 删除末尾的逗号、键名等不完整内容
            result = RemoveTrailingGarbage(result);

            // 补全缺失的括号和引号
            result = BalanceBrackets(result);

            // 尝试重新解析
            if (string.IsNullOrEmpty(result))
                return false;

            try
            {
                JToken.Parse(result);
                fixedJson = result;
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        /// <summary>强制补全 JSON，不验证结果合法性</summary>
        public static string ForceCompleteJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "{}";

            input = input.Trim();
            var result = FixTruncatedString(json: input);
            result = RemoveTrailingGarbage(result);
            result = BalanceBrackets(result);

            return string.IsNullOrEmpty(result) ? "{}" : result;
        }

        /// <summary>修复被截断的字符串值</summary>
        private static string FixTruncatedString(string json)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            var sb = new StringBuilder(json);
            var inString = false;
            var escaped = false;

            for (var i = 0; i < sb.Length; i++)
            {
                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (sb[i] == '\\')
                {
                    escaped = true;
                    continue;
                }

                if (sb[i] == '"')
                    inString = !inString;
            }

            // 如果字符串未闭合，补上引号
            if (inString)
                sb.Append('"');

            return sb.ToString();
        }

        /// <summary>删除末尾不完整的 token（不完整的键名、逗号等）</summary>
        private static string RemoveTrailingGarbage(string json)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            // 从末尾开始删除，直到遇到结构字符 } ] , 为止
            var end = json.Length - 1;

            while (end >= 0)
            {
                var c = json[end];
                if (c is '}' or ']' or '"')
                    break;

                if (c == ',')
                {
                    end--;
                    // 从逗号位置继续往前删空白
                    while (end >= 0 && char.IsWhiteSpace(json[end]))
                        end--;
                    // 如果前面是 { 或 [，则逗号是多余的
                    if (end < 0 || json[end] is '{' or '[')
                    {
                        end--; // 跳过前括号，让 BalanceBrackets 处理
                        continue;
                    }
                    // 否则逗号是合法的，保留
                    end++; // 加上逗号
                    break;
                }

                // 跳过空白和冒号
                if (char.IsWhiteSpace(c) || c == ':')
                {
                    end--;
                    continue;
                }

                // 遇到非法字符（字符串片段、不完整的键名等）
                if (char.IsLetterOrDigit(c) || c is '_' or '-' or '.')
                {
                    end--;
                    continue;
                }

                break;
            }

            return json[..(end + 1)];
        }

        /// <summary>补全缺失的闭合括号和方括号</summary>
        private static string BalanceBrackets(string json)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            var stack = new Stack<char>();
            var inStr = false;
            var escaped = false;

            foreach (var ch in json)
            {
                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (ch == '\\')
                {
                    escaped = true;
                    continue;
                }

                if (ch == '"')
                {
                    inStr = !inStr;
                    continue;
                }

                if (inStr)
                    continue;

                if (ch is '{' or '[')
                {
                    stack.Push(ch);
                }
                else if (ch is '}' or ']')
                {
                    if (stack.Count > 0)
                    {
                        var open = stack.Peek();
                        if ((open == '{' && ch == '}') || (open == '[' && ch == ']'))
                            stack.Pop();
                    }
                }
            }

            var sb = new StringBuilder(json);
            while (stack.Count > 0)
            {
                sb.Append(stack.Pop() switch
                {
                    '{' => '}',
                    '[' => ']',
                    _ => ""
                });
            }

            return sb.ToString();
        }
    }
}