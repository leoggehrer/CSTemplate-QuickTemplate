//@BaseCode
//MdStart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonBase.Extensions
{
    public static partial class FormatterExtensions
    {
        public static IEnumerable<string> FormatCSharpCode(this IEnumerable<string> lines, bool removeBlockComments = true, bool removeLineComments = true)
        {
            return lines.FormatCSharpCode(0, removeBlockComments, removeLineComments);
        }
        public static IEnumerable<string> FormatCSharpCode(this IEnumerable<string> lines, int indent, bool removeBlockComments, bool removeLineComments)
        {
            var result = new List<string>();
            var text = lines.ToText();

            if (string.IsNullOrEmpty(text) == false)
            {
                if (removeBlockComments)
                {
                    text = text.RemoveBlockComments();
                }
                if (removeLineComments)
                {
                    text = text.RemoveLineComments();
                }

                if (text.HasFullCodeBlock())
                {
                    text.FormatCSharpCodeBlock(indent, result);
                }
                else
                {
                    result.AddRange(lines);
                }
            }
            return result;
        }

        #region Helpers
        private static string RemoveLineComments(this string source)
        {
            var sb = new StringBuilder();
            int idx = 0, sIdx = 0, eIdx = -1;

            do
            {
                sIdx = source.IndexOf("//", sIdx);
                if (sIdx >= 0 && IsSourceString(source, sIdx) == false)
                {
                    eIdx = sIdx;
                    do
                    {
                        eIdx = source.IndexOf(Environment.NewLine, eIdx + 1);
                    } while (eIdx != -1 && IsSourceString(source, eIdx));
                }
                if (sIdx > -1 && eIdx > -1)
                {
                    while (idx < sIdx)
                    {
                        sb.Append(source[idx++]);
                    }
                    sIdx = eIdx + Environment.NewLine.Length;
                    idx = sIdx;
                }
            } while (sIdx != -1 && IsSourceString(source, sIdx) == false);
            while (idx < source.Length)
            {
                sb.Append(source[idx++]);
            }
            return sb.ToString();
        }
        private static string RemoveBlockComments(this string source)
        {
            var sb = new StringBuilder();
            int idx = 0, sIdx = 0, eIdx = -1;

            do
            {
                sIdx = source.IndexOf("/*", sIdx);
                if (sIdx >= 0 && IsSourceString(source, sIdx) == false)
                {
                    eIdx = sIdx;
                    do
                    {
                        eIdx = source.IndexOf("*/", eIdx + 1);
                    } while (eIdx != -1 && IsSourceString(source, eIdx));
                }
                if (sIdx > -1 && eIdx > -1)
                {
                    while (idx < sIdx)
                    {
                        sb.Append(source[idx++]);
                    }
                    sIdx = eIdx + Environment.NewLine.Length;
                    idx = sIdx;
                }
            } while (sIdx != -1 && IsSourceString(source, sIdx) == false);

            while (idx < source.Length)
            {
                sb.Append(source[idx++]);
            }
            return sb.ToString();
        }

        private static bool HasFullCodeBlock(this string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var codeBegin = 0;
            var codeEnd = 0;

            foreach (var chr in text)
            {
                if (chr == '{')
                    codeBegin++;
                else if (chr == '}')
                    codeEnd++;
            }
            return codeBegin > 0 && codeBegin == codeEnd;
        }
        private static string TrimCSharpLine(this string text)
        {
            if (string.IsNullOrEmpty(text) == false)
            {
                var trimmer = new Regex(@"\s\s+");

                text = text.Replace("\t", string.Empty);
                text = text.Replace(Environment.NewLine, string.Empty);

                text = text.Trim();
                text = trimmer.Replace(text, " ");
            }
            return text;
        }
        private static bool GetCodeBlockPositions(string text, ref int start, ref int end)
        {
            var cornerBraket = 0;
            var blockBegin = 0;
            var blockEnd = 0;
            var quotationMarks = 0;

            start = end = -1;
            for (var idx = 0; idx >= 0 && idx < text.Length && (start == -1 || end == -1); idx++)
            {
                var chr = text[idx];

                if (chr == '"')
                {
                    quotationMarks++;
                }
                else if (quotationMarks % 2 == 0)
                {
                    if (chr == '[')
                        cornerBraket++;
                    else if (chr == ']')
                        cornerBraket++;
                    else if (chr == '{' && cornerBraket % 2 == 0)
                    {
                        blockBegin++;
                        if (blockBegin == 1)
                            start = idx;
                    }
                    else if (chr == '}' && cornerBraket % 2 == 0)
                    {
                        blockEnd++;
                        if (blockEnd == blockBegin)
                            end = idx;
                    }
                }
            }
            return blockBegin > 0 && blockEnd > 0 && blockBegin == blockEnd;
        }

        private static IEnumerable<string> SplitSourceLine(this string line)
        {
            var result = new List<string>();
            var splitLines = line.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in splitLines)
            {
                if (item.Trim().Length > 0)
                {
                    if (item.Trim().StartsWith("#"))
                    {
                        result.Add(item.Trim());
                    }
                    else
                    {
                        result.AddRange(item.SplitCSharpAssignments());
                    }
                }
            }
            return result;
        }
        private static IEnumerable<string> SplitCSharpAssignments(this string line)
        {
            var result = new List<string>();
            var startIdx = -1;
            var partialStartIdx = -1;
            int partialEndIdx;

            while ((partialEndIdx = line.IndexOf(';', startIdx + 1)) >= 0)
            {
                if (IsAssignmentSemicolon(line, partialEndIdx))
                {
                    result.Add(line.Partialstring(partialStartIdx + 1, partialEndIdx).TrimCSharpLine());
                    partialStartIdx = partialEndIdx;
                    startIdx = partialStartIdx;
                }
                else
                {
                    startIdx++;
                }
            }
            var endPartial = line.Partialstring(partialStartIdx + 1, line.Length - 1).TrimCSharpLine();

            if (endPartial.Length > 0)
            {
                result.Add(endPartial);
            }
            return result;
        }
        private static IEnumerable<string> SplitCSharpLine(this string line)
        {
            var result = new List<string>();
            var lines = new List<string>(line.SplitSourceLine());
            string blockBegin = "/*", blockEnd = "*/";

            for (var i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length > 0)
                {
                    int sIdx, eIdx;
                    if (lines[i].StartsWith("#"))
                    {
                        result.Add(lines[i]);
                    }
                    // add comment
                    else if ((sIdx = lines[i].IndexOf("//", StringComparison.Ordinal)) >= 0
                         &&
                        (lines[i].IndexOf("\\r\\n", sIdx + 1, StringComparison.Ordinal)) >= 0)
                    {
                        result.Add(lines[i]);
                    }
                    else if ((sIdx = lines[i].IndexOf(blockBegin, StringComparison.Ordinal)) >= 0
                              &&
                             (eIdx = lines[i].IndexOf(blockEnd, sIdx + 1, StringComparison.Ordinal)) >= 0)
                    {
                        result.Add(lines[i].Partialstring(sIdx, eIdx + blockEnd.Length - 1));
                        result.Add(lines[i].Partialstring(eIdx + blockEnd.Length, lines[i].Length - 1));
                    }
                    else if ((sIdx = lines[i].IndexOf(blockBegin, StringComparison.Ordinal)) >= 0)
                    {
                        if (sIdx > 1)
                        {
                            string partLine = lines[i].Partialstring(0, sIdx - 1).TrimCSharpLine();

                            if (partLine.Length > 0)
                                result.Add(partLine);
                        }

                        result.Add(blockBegin);
                        if (sIdx + 2 < lines[i].Length - 1)
                        {
                            string partLine = lines[i].Partialstring(sIdx + 2, lines[i].Length - 1).TrimCSharpLine();

                            if (partLine.Length > 0)
                                result.Add(partLine);
                        }
                    }
                    else if ((sIdx = lines[i].IndexOf(blockEnd, StringComparison.Ordinal)) >= 0)
                    {
                        if (sIdx > 1)
                        {
                            string partLine = lines[i].Partialstring(0, sIdx - 1).TrimCSharpLine();

                            if (partLine.Length > 0)
                                result.Add(partLine);
                        }

                        result.Add(blockEnd);
                        if (sIdx + 2 < lines[i].Length - 1)
                        {
                            string partLine = lines[i].Partialstring(sIdx + 2, lines[i].Length - 1).TrimCSharpLine();

                            if (partLine.Length > 0)
                                result.Add(partLine);
                        }
                    }
                    else
                    {
                        result.AddRange(lines[i].SplitCSharpLine('[', ']'));
                    }
                }
            }
            return result;
        }
        private static IEnumerable<string> SplitCSharpLine(this string line, char left, char right)
        {
            var lastIdx = -1;
            var startIdx = -1;
            var result = new List<string>();

            static int IndexOf(string text, int start, char chr)
            {
                int result = -1;
                int quotationMarks = 0;

                for (int i = start; i < text.Length && result == -1; i++)
                {
                    if (text[i] == '"')
                        quotationMarks++;
                    else if (quotationMarks % 2 == 0 && text[i] == chr)
                        result = i;
                }
                return result;
            }

            int endIdx;
            while ((startIdx = IndexOf(line, startIdx + 1, left)) >= 0
                   && (endIdx = IndexOf(line, startIdx + 1, right)) > startIdx
                   && endIdx - startIdx > 1)
            {
                result.Add(line.Partialstring(startIdx, endIdx).TrimCSharpLine());
                lastIdx = startIdx = endIdx;
            }
            string endPartial = line.Partialstring(lastIdx + 1, line.Length - 1).TrimCSharpLine();

            if (endPartial.Length > 0)
            {
                result.Add(endPartial);
            }
            return result;
        }

        private static bool IsAssignmentSemicolon(string text, int pos)
        {
            return IsLiteralCharacter(text, pos) == false && IsSourceString(text, pos) == false;
        }
        private static bool IsLiteralCharacter(string text, int pos)
        {
            return pos > 0 && pos + 1 < text.Length && text[pos - 1] == '\'' && text[pos + 1] == '\'';
        }
        private static bool IsSourceString(string text, int pos)
        {
            var limiterCount = 0;

            for (var i = 0; i < pos && i < text.Length; i++)
            {
                if (text[i] == '\"')
                    limiterCount++;
            }
            return limiterCount % 2 > 0;
        }

        private static void FormatCSharpCodeBlock(this string text, int indent, List<string> lines)
        {
            var beginPos = 0;
            var endPos = 0;

            void AddCodeLines(string txt, int idt, List<string> list)
            {
                var items = txt.SplitCSharpLine();

                list.AddRange(items.Where(l => l.Length > 0)
                    .Select(l => l.StartsWith("#") ? l : l.SetIndent(idt))
                    .ToArray());
            }

            if (GetCodeBlockPositions(text, ref beginPos, ref endPos))
            {
                AddCodeLines(text.Partialstring(0, beginPos - 1), indent, lines);
                lines.Add("{".SetIndent(indent));
                text.Partialstring(beginPos + 1, endPos - 1).FormatCSharpCodeBlock(indent + 1, lines);
                lines.Add("}".SetIndent(indent));
                text.Partialstring(endPos + 1, text.Length - 1).FormatCSharpCodeBlock(indent, lines);
            }
            else
            {
                AddCodeLines(text, indent, lines);
            }
        }
        #endregion Helpers
    }
}
//MdEnd