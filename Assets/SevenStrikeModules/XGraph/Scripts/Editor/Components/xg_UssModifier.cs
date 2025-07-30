namespace SevenStrikeModules.XGraph
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    public class xg_UssModifier
    {
        public static bool ModifyCssRule(string cssFilePath, string selector, string property, string newValue)
        {
            try
            {
                // 读取 CSS 文件内容
                string cssContent = File.ReadAllText(cssFilePath, Encoding.UTF8);

                // 构建更精确的正则表达式模式
                string pattern = $@"({Regex.Escape(selector)}\s*{{)([^}}]*?)({Regex.Escape(property)}\s*:\s*)([^;}}]*)([;}}])([^}}]*)(}})";
                Regex regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);

                // 替换属性值
                string newCssContent;
                if (regex.IsMatch(cssContent))
                {
                    newCssContent = regex.Replace(cssContent, match =>
                    {
                        // 保留原始的分隔符（; 或 }），但确保只有一个
                        string separator = match.Groups[5].Value == ";" ? ";" : "";
                        return $"{match.Groups[1].Value}{match.Groups[2].Value}{property}: {newValue}{separator}{match.Groups[6].Value}{match.Groups[7].Value}";
                    });
                }
                else
                {
                    // 如果没有找到匹配项，尝试添加新规则
                    newCssContent = AddOrUpdateCssRule(cssContent, selector, property, newValue);
                }

                // 写回文件
                File.WriteAllText(cssFilePath, newCssContent, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"修改 CSS 文件时出错: {ex.Message}");
                return false;
            }
        }

        private static string AddOrUpdateCssRule(string cssContent, string selector, string property, string value)
        {
            // 更精确的匹配选择器
            string selectorPattern = $@"({Regex.Escape(selector)}\s*{{)([^}}]*)(}})";
            Regex selectorRegex = new Regex(selectorPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);

            if (selectorRegex.IsMatch(cssContent))
            {
                return selectorRegex.Replace(cssContent, match =>
                {
                    string prefix = match.Groups[1].Value;
                    string content = match.Groups[2].Value;
                    string suffix = match.Groups[3].Value;

                    // 处理属性
                    var propRegex = new Regex($@"({Regex.Escape(property)}\s*:\s*)([^;}}]*)([;}}])");
                    if (propRegex.IsMatch(content))
                    {
                        content = propRegex.Replace(content, m =>
                        {
                            // 保留原始的分隔符（; 或 }），但确保只有一个
                            string separator = m.Groups[3].Value == ";" ? ";" : "";
                            return $"{property}: {value}{separator}";
                        });
                    }
                    else
                    {
                        // 添加新属性，确保有分号
                        content = content.TrimEnd();
                        if (!string.IsNullOrEmpty(content))
                            content += ";";
                        content += $"{property}: {value}";
                    }

                    return $"{prefix}{content}{suffix}";
                });
            }
            else
            {
                return cssContent + $"\n{selector} {{\n  {property}: {value};\n}}\n";
            }
        }
    }
}