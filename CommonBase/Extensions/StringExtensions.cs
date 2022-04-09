//@BaseCode
//MdStart
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonBase.Extensions
{
    public static partial class StringExtensions
    {
        public static void CheckNotNullOrEmpty(this string source, string argName)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentException("String is null or empty!", argName);
        }

        public static bool TryParse(this string value, Type type, out object? typeValue)
        {
            bool result = false;

            if (value == null)
            {
                result = true;
                typeValue = null;
            }
            else if (type.IsEnum)
            {
                result = Enum.TryParse(type, value, out typeValue);
            }
            else if (type == typeof(TimeSpan))
            {
                typeValue = TimeSpan.Parse(value);
                result = true;
            }
            else if (type == typeof(DateTime))
            {
                typeValue = DateTime.Parse(value);
                result = true;
            }
            else if (type == typeof(string))
            {
                typeValue = value;
            }
            else
            {
                typeValue = Convert.ChangeType(value, type);
                result = true;
            }
            return result;
        }

        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            return source.Contains(toCheck, StringComparison.OrdinalIgnoreCase);
        }
        public static bool Contains(this string source, string toCheck, StringComparison comparison)
        {
            return source?.IndexOf(toCheck, comparison) >= 0;
        }
        public static bool Contains(this string source, params string[] toChecks)
        {
            return source.Contains(StringComparison.CurrentCultureIgnoreCase, toChecks);
        }
        public static bool Contains(this string source, StringComparison comparison, params string[] toChecks)
        {
            source.CheckArgument(nameof(source));

            var result = toChecks.Length > 0;

            foreach (var item in toChecks)
            {
                result = result && source.Contains(item, comparison);
            }
            return result;
        }

        public static string ReplaceBetween(this string source, string startText, string endText, string replaceText)
        {
            source.CheckArgument(nameof(source));
            startText.CheckArgument(nameof(startText));
            endText.CheckArgument(nameof(endText));
            replaceText.CheckArgument(nameof(replaceText));

            string result;
            var sIdx = source.IndexOf(startText);
            var eIdx = source.IndexOf(endText);

            if (sIdx > -1 && eIdx > -1 && sIdx <= eIdx)
            {
                result = source[..(sIdx + startText.Length)];
                result += replaceText;
                result += source[eIdx..];
            }
            else
            {
                result = source;
            }
            return result;
        }
        public static IEnumerable<string[]> Split(this IEnumerable<string> source, string separator)
        {
            source.CheckArgument(nameof(source));

            return source.Select(l => string.IsNullOrEmpty(l) ? Array.Empty<string>() : l.Split(separator));
        }
        public static IEnumerable<T> SplitAndMap<T>(this IEnumerable<string> source, string separator, Func<string[], T> mapper)
        {
            source.CheckArgument(nameof(source));
            mapper.CheckArgument(nameof(mapper));

            return source.Split(separator).Select(d => mapper(d));
        }
        public static IEnumerable<T> SplitAndMap<T>(this IEnumerable<string> source, string separator, Func<string[], string[], T> mapper)
        {
            source.CheckArgument(nameof(source));
            mapper.CheckArgument(nameof(mapper));

            var splitSource = source.Split(separator);
            var header = splitSource.FirstOrDefault();

            return splitSource.Skip(1).Select(d => mapper(d, header ?? Array.Empty<string>()));
        }

        public static string GetValue(this string source, string defaultValue)
        {
            return string.IsNullOrEmpty(source) ? defaultValue : source;
        }

        public static string Separator { get; set; } = ";";
        public static string NullText { get; set; } = "<NULL>";
        private static Regex Trimmer { get; } = new Regex(@"\s\s+");

        public class DivideInfo : TagInfo
        {
            public DivideInfo(int startIdx, int endIdx)
            {
                StartTag = string.Empty;
                StartTagIndex = startIdx;
                EndTag = string.Empty;
                EndTagIndex = endIdx;
            }

            public DivideInfo(TagInfo tagInfo)
            {
                tagInfo.CheckArgument(nameof(tagInfo));

                StartTag = tagInfo.StartTag;
                StartTagIndex = tagInfo.StartTagIndex;
                EndTag = tagInfo.EndTag;
                EndTagIndex = tagInfo.EndTagIndex;
            }
            public string Text { get; internal set; } = String.Empty;
        }

        public static IEnumerable<DivideInfo> Divide(this string text, string[] tags)
        {
            List<DivideInfo> result = new();
            int startIdx = 0;
            var tagInfos = text.GetAllTags(tags);

            foreach (var tagInfo in tagInfos)
            {
                if (startIdx < tagInfo.StartTagIndex)
                {
                    result.Add(new DivideInfo(startIdx, tagInfo.StartTagIndex - 1)
                    {
                        Text = text.Partialstring(startIdx, tagInfo.StartTagIndex - 1),
                    });
                    result.Add(new DivideInfo(tagInfo)
                    {
                        Text = text.Partialstring(tagInfo.StartTagIndex, tagInfo.EndTagIndex),
                    });
                    startIdx = tagInfo.EndTagIndex + 1;
                }
                else if (startIdx == tagInfo.StartTagIndex)
                {
                    result.Add(new DivideInfo(tagInfo)
                    {
                        Text = text.Partialstring(tagInfo.StartTagIndex, tagInfo.EndTagIndex),
                    });
                    startIdx = tagInfo.EndTagIndex + 1;
                }
            }
            if (startIdx < text.Length - 1)
            {
                result.Add(new DivideInfo(startIdx, text.Length)
                {
                    Text = text.Partialstring(startIdx, text.Length - 1),
                });
            }
            return result;
        }

        public static IEnumerable<TagInfo> GetAllTags(this string text, string[] tags)
        {
            tags.CheckArgument(nameof(tags));

            List<TagInfo> result = new();

            for (int i = 0; i + 1 < tags.Length; i += 2)
            {
                result.AddRange(text.GetAllTags(tags[i], tags[i + 1]));
            }
            return result;
        }
        public static IEnumerable<TagInfo> GetAllTags(this string text, string startTag, string endTag)
        {
            return text.GetAllTags<TagInfo>(startTag, endTag);
        }
        public static IEnumerable<TagInfo> GetAllTags(this string text, string startTag, string endTag, params char[] excludeBlocks)
        {
            return text.GetAllTags<TagInfo>(startTag, endTag, excludeBlocks);
        }
        public static IEnumerable<T> GetAllTags<T>(this string text, string startTag, string endTag, params char[] excludeBlocks)
            where T : TagInfo, new()
        {
            text.CheckArgument(nameof(text));

            var parseIndex = 0;
            int startTagIndex;
            int endTagIndex;
            var result = new List<T>();
            var tagHeader = new TagInfo.TagHeader(text);

            do
            {
                startTagIndex = text.IndexOf(startTag, parseIndex, StringComparison.CurrentCultureIgnoreCase);
                var startTagEndIndex = startTagIndex > -1 ? startTagIndex + startTag.Length : parseIndex;
                endTagIndex = startTagEndIndex >= 0 ? text.IndexOf(endTag, startTagEndIndex, StringComparison.CurrentCultureIgnoreCase) : -1;

                if (startTagIndex > -1 && endTagIndex > startTagIndex)
                {
                    int idx = startTagEndIndex;
                    int endTagSearchPosAt = startTagEndIndex;
                    var blockCounter = new int[excludeBlocks.Length];

                    while (idx < endTagIndex)
                    {
                        for (int j = 0; j < blockCounter.Length; j++)
                        {
                            if (text[idx] == excludeBlocks[j])
                            {
                                endTagSearchPosAt = idx;
                                blockCounter[j] += j % 2 == 0 ? 1 : -1;
                            }
                        }
                        idx++;
                    }
                    while (idx < text.Length && blockCounter.Sum() != 0)
                    {
                        for (int j = 0; j < blockCounter.Length; j++)
                        {
                            if (text[idx] == excludeBlocks[j])
                            {
                                endTagSearchPosAt = idx;
                                blockCounter[j] += j % 2 == 0 ? 1 : -1;
                            }
                        }
                        idx++;
                    }
                    if (endTagSearchPosAt > endTagIndex && blockCounter.Sum() == 0)
                    {
                        endTagIndex = text.IndexOf(endTag, endTagSearchPosAt, StringComparison.CurrentCultureIgnoreCase);
                    }
                }

                if (startTagIndex > -1 && endTagIndex > startTagIndex)
                {
                    result.Add(new T
                    {
                        Header = tagHeader,
                        StartTag = startTag,
                        StartTagIndex = startTagIndex,
                        EndTag = endTag,
                        EndTagIndex = endTagIndex,
                    });
                    parseIndex = startTagEndIndex;
                }
            } while (startTagIndex > -1 && endTagIndex > -1);
            return result;
        }

        /// <summary>
        /// Indicates whether the specified string is null or an Empty string.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// Indicates whether the specified string has a content.
        /// </summary>
        /// <param name="source">The string to test.</param>
        /// <returns>true if the value parameter is not null and not empty; otherwise, false.</returns>
        public static bool HasContent(this string source)
        {
            return !string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// Indicates whether the index is within the string.
        /// </summary>
        /// <param name="source">The string in which the index is tested.</param>
        /// <param name="index">The index to test.</param>
        /// <returns></returns>
        public static bool InRange(this string source, int index)
        {
            return source != null && index > -1 && index < source.Length;
        }

        /// <summary>
        /// Determines if two string objects have unequal values.
        /// </summary>
        /// <param name="source">The string to compare to the value.</param>
        /// <param name="value">The string to compare to the source.</param>
        /// <returns>True if the values are not equals, false else.</returns>
        public static bool NotEquals(this string source, string value)
        {
            return source.AreEquals(value) == false;
        }
        /// <summary>
        /// Determines if two string objects have equal values.
        /// </summary>
        /// <param name="source">The string to compare to the value.</param>
        /// <param name="value">The string to compare to the source.</param>
        /// <returns>True if the values are equals, false else.</returns>
        public static bool AreEquals(this string source, string value)
        {
            return source == null && value == null || value != null && source != null && source.Equals(value);
        }
        /// <summary>
        /// Determines if two string objects have equal values.
        /// </summary>
        /// <param name="source">The string to compare to the value.</param>
        /// <param name="value">The string to compare to the source.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <returns>True if the values are equals, false else.</returns>
        public static bool AreEquals(this string source, string value, StringComparison stringComparison)
        {
            return source == null && value == null || value != null && source != null && source.Equals(value, stringComparison);
        }

        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if the value parameter is null or String.Empty, or if value consists exclusively of white-space characters.</returns>
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        /// <summary>
        /// Gets the value of the current object or the default value of the parameter.
        /// </summary>
        /// <param name="text">The current objet.</param>
        /// <returns>The value of the current object, or an empty string.</returns>
        public static string GetValueOrDefault(this string text)
        {
            return text.GetValueOrDefault(string.Empty);
        }

        /// <summary>
        /// Gets the value of the current object or the default value of the parameter.
        /// </summary>
        /// <param name="text">The current objet.</param>
        /// <param name="defaultValue">The default value if the object is null or empty.</param>
        /// <returns>The value of the current object, or the parameter default value.</returns>
        public static string GetValueOrDefault(this string text, string defaultValue)
        {
            return string.IsNullOrEmpty(text) ? defaultValue : text;
        }

        /// <summary>
        /// Diese Methode entfernt alle redundanten Leerzeichen aus dem Text.
        /// </summary>
        /// <param name="text">Text aus welchem die redundanten Leerzeichen entfernt werden sollen.</param>
        /// <returns>Text ohne redundante Leerzeichen.</returns>
        public static string Fulltrim(this string text)
        {
            text.CheckArgument(nameof(text));

            if (String.IsNullOrEmpty(text) == false)
            {
                text = Trimmer.Replace(text, " ");
                text = text.Trim();
                if (text.Equals(" "))
                {
                    text = String.Empty;
                }
            }
            return text;
        }

        /// <summary>
        /// Diese Methode fuegt einem Text n Einzuege hinzu.
        /// </summary>
        /// <param name="text">Text, dem die Einzuege hinzugefuegt werden.</param>
        /// <param name="count">Anzahl der Einzuege die dem Text hinzugefuegt werden.</param>
        /// <returns>Text mit Anzahl von Einzuegen.</returns>
        public static string SetIndent(this string text, int count)
        {
            StringBuilder sb = new();

            if (text != null)
            {
                for (int i = 0; i < count; i++)
                    sb.Append("    ");        // Four spaces for one indent.
            }
            sb.Append(text);
            return sb.ToString();
        }

        /// <summary>
        /// This method adds n indents to a string array.
        /// </summary>
        /// <param name="lines">String array to which the indents are added.</param>
        /// <param name="count">Number of indents that are added to the string array.</param>
        /// <returns>String array with number of indents.</returns>
        public static string[] SetIndent(this string[] lines, int count)
        {
            lines.CheckArgument(nameof(lines));

            if (lines != null)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = lines[i].SetIndent(count);
                }
            }
            return lines ?? Array.Empty<string>();
        }
        /// <summary>
        /// This method adds n indents to a string array.
        /// </summary>
        /// <param name="lines">String array to which the indents are added.</param>
        /// <param name="count">Number of indents that are added to the string array.</param>
        /// <returns>String array with number of indents.</returns>
        public static IEnumerable<string> SetIndent(this IEnumerable<string> lines, int count)
        {
            lines.CheckArgument(nameof(lines));

            return lines.ToArray().SetIndent(count);
        }

        /// <summary>
        /// This method converts a string array to a coherent text.
        /// </summary>
        /// <param name="lines">The String-Array.</param>
        /// <returns>The compound text.</returns>
        public static string ToText(this IEnumerable<string> lines)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            StringBuilder sb = new();

            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        /// <summary>
        /// This method converts a string array to a coherent text.
        /// </summary>
        /// <param name="lines">The String-Array.</param>
        /// <param name="lineConvert">Lines converter.</param>
        /// <returns>The compound text.</returns>
        public static string ToText(this IEnumerable<string> lines, Func<string, string> lineConvert)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            StringBuilder sb = new();

            foreach (var line in lines)
            {
                sb.AppendLine(lineConvert == null ? line : lineConvert(line));
            }
            return sb.ToString();
        }

        /// <summary>
        /// This method converts text with line breaks into a string array.
        /// </summary>
        /// <param name="text">The text to be converted.</param>
        /// <returns>The string-array from the text.</returns>
        public static IEnumerable<string> ToLines(this string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            List<string> result = new();

            foreach (var line in text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                result.Add(line);
            }
            return result;
        }

        public static IEnumerable<string?> Trim(this IEnumerable<string> source)
        {
            source.CheckArgument(nameof(source));

            var result = new List<string?>();
            var prvEmpty = true;

            foreach (var item in source)
            {
                if (string.IsNullOrEmpty(item) && prvEmpty == false)
                {
                    result.Add(string.Empty);
                    prvEmpty = true;
                }
                else if (string.IsNullOrEmpty(item) == false)
                {
                    result.Add(item);
                    prvEmpty = false;
                }
            }
            var lastElem = result.LastOrDefault();

            if (string.IsNullOrEmpty(lastElem))
            {
                _ = result.Remove(lastElem);
            }
            return result;
        }

        /// <summary>
        /// Extracts a substring from a string (excludes from to).
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="from">Starttext</param>
        /// <param name="to">Endtext</param>
        /// <returns>The substring.</returns>
        public static string Partialstring(this string text, string from, string to)
        {
            var result = default(string);

            if (text.HasContent())
            {
                int f = text.IndexOf(from);
                int t = text.IndexOf(to, f + 1) + to.Length - 1;

                result = text.Partialstring(f, t);
            }
            return result ?? String.Empty;
        }

        /// <summary>
        /// Extracts a substring from a string (includes from to).
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="from">Starttext</param>
        /// <param name="to">Endtext</param>
        /// <returns>The substring.</returns>
        public static string Betweenstring(this string text, string from, string to)
        {
            var result = default(string);

            if (text.HasContent())
            {
                int f = text.IndexOf(from) + from.Length;
                int t = text.IndexOf(to, f + 1) - 1;

                result = text.Partialstring(f, t);
            }
            return result ?? String.Empty;
        }

        /// <summary>
        /// Extracts a substring from a string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="from">Startposition</param>
        /// <param name="to">Endposition</param>
        /// <returns>The substring.</returns>
        public static string Partialstring(this string text, int from, int to)
        {
            StringBuilder sb = new();

            if (string.IsNullOrEmpty(text) == false)
            {
                for (int i = from; i >= 0 && i <= to && i < text.Length; i++)
                {
                    sb.Append(text[i]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Removes the text from the start tag to the end tag.
        /// </summary>
        /// <param name="text">The text from which the partial text should be removed.</param>
        /// <param name="startTag">Startposition</param>
        /// <param name="endTag">Endposition</param>
        /// <returns>The text with the missing partial text.</returns>
        public static string Remove(this string text, string startTag, string endTag)
        {
            text.CheckArgument(nameof(text));
            startTag.CheckArgument(nameof(startTag));
            endTag.CheckArgument(nameof(endTag));

            StringBuilder result = new();
            int parseIndex = 0;
            int startTagIndex;
            int endTagIndex;

            do
            {
                startTagIndex = text.IndexOf(startTag, parseIndex, StringComparison.CurrentCultureIgnoreCase);
                var startTagEndIndex = startTagIndex > -1 ? startTagIndex + startTag.Length : parseIndex;
                endTagIndex = startTagEndIndex >= 0 ? text.IndexOf(endTag, startTagEndIndex, StringComparison.CurrentCultureIgnoreCase) : -1;
                var endTagEndIndex = endTagIndex > -1 ? endTagIndex + endTag.Length : parseIndex;

                if (startTagIndex > -1 && endTagIndex > startTagIndex)
                {
                    result.Append(text[parseIndex..startTagIndex]);
                    parseIndex = endTagEndIndex;
                }
            } while (startTagIndex > -1 && endTagIndex > -1);

            if (parseIndex < text.Length)
            {
                result.Append(text[parseIndex..]);
            }
            return result.ToString();
        }

        public static string ReplaceUmlauts(this string text)
        {
            StringBuilder sb = new();

            if (text != null)
            {
                foreach (var item in text)
                {
                    if (item == 223) // sz is replaced by ss
                    {
                        sb.Append("ss");
                    }
                    else if (item == 196)
                    {
                        sb.Append("Ae");
                    }
                    else if (item == 228)
                    {
                        sb.Append("ae");
                    }
                    else if (item == 214)
                    {
                        sb.Append("Oe");
                    }
                    else if (item == 246)
                    {
                        sb.Append("oe");
                    }
                    else if (item == 220)
                    {
                        sb.Append("Ue");
                    }
                    else if (item == 252)
                    {
                        sb.Append("ue");
                    }
                    else
                    {
                        sb.Append(item);
                    }
                }
            }
            return sb.ToString();
        }
        public static string ReplaceAll(this string text, TagInfo tagInfo, Func<string, string> replace)
        {
            text.CheckArgument(nameof(text));
            tagInfo.CheckArgument(nameof(tagInfo));

            StringBuilder result = new();
            int parseIndex = 0;
            int startTagIndex;
            int endTagIndex;

            do
            {
                startTagIndex = text.IndexOf(tagInfo.StartTag, parseIndex, StringComparison.CurrentCultureIgnoreCase);
                int startTagEndIndex = startTagIndex > -1 ? startTagIndex + tagInfo.StartTag.Length : parseIndex;
                endTagIndex = startTagEndIndex >= 0 ? text.IndexOf(tagInfo.EndTag, startTagEndIndex, StringComparison.CurrentCultureIgnoreCase) : -1;
                int endTagEndIndex = endTagIndex > -1 ? endTagIndex + tagInfo.EndTag.Length : parseIndex;

                if (startTagIndex > -1 && endTagIndex > startTagIndex)
                {
                    string substr = text.Substring(startTagIndex, endTagIndex - startTagIndex + tagInfo.EndTag.Length);

                    result.Append(text[parseIndex..startTagIndex]);
                    if (replace != null)
                    {
                        result.Append(replace(substr));
                    }
                    parseIndex = endTagEndIndex;
                }
            } while (startTagIndex > -1 && endTagIndex > -1);

            if (parseIndex < text.Length)
            {
                result.Append(text[parseIndex..]);
            }
            return result.ToString();
        }
        public static string ReplaceAll(this string text, string startTag, string endTag, string replaceText)
        {
            text.CheckArgument(nameof(text));

            return text.ReplaceAll(startTag, endTag, s => replaceText);
        }
        public static string ReplaceAll(this string text, string startTag, string endTag, Func<string, string> replace)
        {
            text.CheckArgument(nameof(text));
            startTag.CheckArgument(nameof(startTag));
            endTag.CheckArgument(nameof(endTag));

            int parseIndex = 0;
            int startTagIndex;
            int endTagIndex;
            StringBuilder result = new();

            do
            {
                startTagIndex = text.IndexOf(startTag, parseIndex, StringComparison.CurrentCultureIgnoreCase);
                int startTagEndIndex = startTagIndex > -1 ? startTagIndex + startTag.Length : parseIndex;
                endTagIndex = startTagEndIndex >= 0 ? text.IndexOf(endTag, startTagEndIndex, StringComparison.CurrentCultureIgnoreCase) : -1;
                int endTagEndIndex = endTagIndex > -1 ? endTagIndex + endTag.Length : parseIndex;

                if (startTagIndex > -1 && endTagIndex > startTagIndex)
                {
                    string substr = text.Substring(startTagIndex, endTagIndex - startTagIndex + endTag.Length);

                    result.Append(text[parseIndex..startTagIndex]);
                    if (replace != null)
                    {
                        result.Append(replace(substr));
                    }
                    parseIndex = endTagEndIndex;
                }
            } while (startTagIndex > -1 && endTagIndex > -1);

            if (parseIndex < text.Length)
            {
                result.Append(text[parseIndex..]);
            }
            return result.ToString();
        }
        /// <summary>
        /// Removes all spaces from a string.
        /// </summary>
        /// <param name="source">String from which the spaces are removed.</param>
        /// <returns>String with no spaces.</returns>
        public static string RemoveAll(this string source)
        {
            return source.RemoveAll(" ");
        }
        public static string RemoveAll(this string source, params string[] removeItems)
        {
            source.CheckArgument(nameof(source));

            var result = source;

            foreach (var item in removeItems)
            {
                result = result.Replace(item, string.Empty);
            }
            return result;
        }

        public static int ToInt(this string text)
        {
            int result = 0;

            if (text != null)
            {
                foreach (var item in text)
                {
                    if (char.IsDigit(item))
                    {
                        result *= 10;
                        result = result + item - '0';
                    }
                }
            }
            return result;
        }
        public static long ToLong(this string text)
        {
            long result = 0;

            if (text != null)
            {
                foreach (var item in text)
                {
                    if (char.IsDigit(item))
                    {
                        result *= 10;
                        result = result + item - '0';
                    }
                }
            }
            return result;
        }
        public static string ToFileName(this string text)
        {
            if (text != null)
            {
                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    text = text.Replace(c, '_');
                }
            }
            return text ?? String.Empty;
        }

        public static IEnumerable<T?> ToEnumerable<T>(this string source, string separator)
        {
            List<T?> result = new();

            if (string.IsNullOrEmpty(source) == false)
            {
                string[] items = source.Split(separator);

                foreach (var item in items)
                {
                    if (item.Equals(NullText))
                    {
                        result.Add(default);
                    }
                    else
                    {
                        try
                        {
                            result.Add((T)Convert.ChangeType(item, typeof(T)));
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            result.Add(default);
                        }
                    }
                }
            }
            return result;
        }

        public static byte[] ToByteArray(this string source)
        {
            byte[]? result = null;

            if (source != null)
            {
                result = new ASCIIEncoding().GetBytes(source);
            }
            return result ?? Array.Empty<byte>();
        }

        public static string Encrypt(this string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using var aesAlg = Aes.Create();
            using var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            var iv = aesAlg.IV;

            var decryptedContent = msEncrypt.ToArray();

            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            return Convert.ToBase64String(result);
        }
        public static string Decrypt(this string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            string result;
            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(key, iv);
            using (MemoryStream msDecrypt = new(cipher))
            {
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                result = srDecrypt.ReadToEnd();
            }

            return result;
        }

        private static bool IsWord(this string text)
        {
            var result = text.IsNullOrEmpty() == false;

            for (int i = 0; result && i < text.Length; i++)
            {
                result = char.IsLetter(text[i]);
            }
            return result;
        }
        public static string CreatePluralWord(this string wordInSingular)
        {
            string result = wordInSingular;

            if (wordInSingular.IsWord())
            {
                if (wordInSingular.EndsWith("y", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular[0..^1]}ies";
                }
                else if (wordInSingular.EndsWith("h", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular}es";
                }
                else if (wordInSingular.EndsWith("x", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular}es";
                }
                else if (wordInSingular.EndsWith("f", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular[0..^1]}ves";
                }
                else if (wordInSingular.EndsWith("ss", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular}es";
                }
                else if (wordInSingular.EndsWith("s", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular}";
                }
                else
                {
                    result = $"{wordInSingular}s";
                }
            }
            return result;
        }
    }
}
//MdEnd