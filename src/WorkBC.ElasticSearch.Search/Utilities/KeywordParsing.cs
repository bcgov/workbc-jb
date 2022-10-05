using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WorkBC.ElasticSearch.Search.Utilities
{
    /// <summary>
    ///     Utilities for turning a user inputted keyword string into the format required by an ElasticSearch
    ///     simple_query_string query
    /// </summary>
    /// <remarks>
    ///     BRD Requirements, as of Nov 4, 2019
    ///     Multiple words entered within the keyword field will be treated as follows:
    ///     o Multiple words entered with spaces will be treated with an ‘AND’ clause. E.g.  program manager - the words
    ///     “program” and “manager” need to both be present within the search fields checked.  For example, a Job Title
    ///     called ‘Sales Program Area Manager’ would return a result as both ‘Program and Manager’ appear in the job title.
    ///     o Multiple words separated with commas will be treated with an ‘OR’ clause (e.g. baker, cook” should return all
    ///     job posting that have “baker” or “cook”).   If multiple words are entered with commas in between, then the ‘AND’
    ///     clause will apply to the multiple words with spaces (see above) and the ‘OR’ clause will apply to each set of
    ///     words separated by commas.   For example, if ‘baker manager, cook’ is entered then all job posting that have
    ///     either ‘baker AND manager’ OR ‘cook’ will be returned in the job search results.
    ///     o Multiple words in double quotations should always be interpreted as an exact phrase. The search should look for
    ///     the exact phrase within each of the selected search fields, e.g. search for "Gold Mine" would return job
    ///     postings that have both words together.
    /// </remarks>
    public class KeywordParsing
    {
        /// <summary>
        ///     Turns a user inputted keyword string into the format required by an ElasticSearch
        ///     simple_query_string query
        /// </summary>
        public static string BuildSimpleQueryString(string keywords)
        {
            keywords = SanitizeKeywords(keywords);

            // add a space before and after a comma so it will become it's own segment during the split
            keywords = keywords.Replace(",", " , ");

            // get the quoted and unquoted segments as an array
            string[] segments = SplitQuotedSegments(keywords);

            // remove any segments that just contain the word and (all queries are and queries by default)
            segments = segments.Where(s => s.ToLower() != "and").ToArray();

            if (segments.Length == 0)
            {
                return string.Empty;
            }

            // if there is only 1 segment then exit and return it
            if (segments.Length == 1)
            {
                return segments[0];
            }

            // loop through the segments to do some initial tidying up
            for (var i = 0; i < segments.Length; i++)
            {
                // trim spaces from beginning and end of the segment
                segments[i] = segments[i].Trim();

                // if a segment just contains the word OR then turn it into a comma since an OR search was the
                // user's intention anyway
                if (segments[i].ToLower() == "or")
                {
                    segments[i] = ",";
                }
            }

            // if there are no ',' (OR) segments then just join the segments back together and return
            if (segments.All(s => s != ","))
            {
                return string.Join(' ', segments);
            }

            int lastOrPosition = -1;

            // loop through the segments
            for (var i = 0; i < segments.Length; i++)
            {
                // if we find a comma (OR search)...
                if (segments[i] == ",")
                {
                    // turn it into a pipe character (used by elasticsearch simple_query_string  for OR)
                    segments[i] = "|";

                    // turn the words before the comma into 'AND' (+) conditions and group them with brackets around them
                    if (i - lastOrPosition >= 3)
                    {
                        // loop though all the segments after the last comma and join them as ANDs
                        int start = lastOrPosition + 1;
                        int stop = i - 1;
                        MergeAndSegments(ref segments, start, stop);
                    }

                    lastOrPosition = i;
                }

                // handle special condition for the last segment in the list.
                if (i == segments.Length - 1 && i - lastOrPosition >= 2)
                {
                    // loop though all the segments after the last comma and join them as ANDs
                    int start = lastOrPosition + 1;
                    int stop = segments.Length - 1;
                    MergeAndSegments(ref segments, start, stop);
                }
            }

            // remove the blank segments and then join ,all the segments 
            return string.Join("",segments.Where(s => s != "").ToArray());
        }

        /// <summary>
        ///     Joins segments between start and end into a query like (+dog +cat).
        ///     Stores the new value in the first segment position and clears the other
        ///     segment positions
        /// </summary>
        private static void MergeAndSegments(ref string[] segments, int startIndex, int endIndex)
        {
            var ands = "";
            for (int j = startIndex; j <= endIndex; j++)
            {
                ands += $"{segments[j]} ";
                segments[j] = "";
            }

            // put round brackets around the joined segments
            segments[startIndex] = $"({ands.Trim()})";
        }

        /// <summary>
        ///     Pre-processes the string before parsing
        /// </summary>
        public static string SanitizeKeywords(string keywords)
        {
            // turn pipe character into comma so it can be used as OR condition
            keywords = keywords.Replace("|", ",");

            // remove characters that will break elasticsearch or impact the query results
            keywords = Regex.Replace(keywords, @"(\(|\)|~|\{|\}|\#)", " ");

            // remove everything except A-Z, a-z, 0-9, À-ÿ, *, (comma), |, ', ", -, (space)
            keywords = Regex.Replace(keywords, "[^\u0100-\uFFFFa-zA-Z0-9À-ÿ*,|_'\" -]", " ");

            // trim whitespace from the beginning and end
            keywords = keywords.Trim();

            // remove whitespace characters and turn consecutive spaces into single spaces
            keywords = Regex.Replace(keywords, @"\s+", " ");

            // turn consecutive commas into single commas (comma means OR)
            keywords = Regex.Replace(keywords, @",{2,}", ",");

            // remove spaces around commas and commas at the beginning and end
            keywords = keywords.Replace(" ,", ",")
                .Replace(", ", ",")
                .Trim(',');

            return keywords;
        }

        /// <summary>
        ///     Parses a string into quoted and unquoted segments
        ///     e.g. 'The quick "brown fox" jumps "over the" "lazy" dog'
        ///     becomes ['The', 'quick', '"brown fox"', 'jumps', '"over the"', '"lazy"', 'dog']
        /// </summary>
        public static string[] SplitQuotedSegments(string line)
        {
            var insideQuotes = false;
            int start = -1;

            var parts = new List<string>();

            for (var i = 0; i < line.Length; i++)
            {
                if (char.IsWhiteSpace(line[i]))
                {
                    if (!insideQuotes)
                    {
                        if (start != -1)
                        {
                            parts.Add(line.Substring(start, i - start));
                            start = -1;
                        }
                    }
                }
                else if (line[i] == '"')
                {
                    if (start != -1)
                    {
                        parts.Add("\"" + line.Substring(start, i - start) + "\"");
                        start = -1;
                    }

                    insideQuotes = !insideQuotes;
                }
                else
                {
                    if (start == -1)
                    {
                        start = i;
                    }
                }
            }

            if (start != -1)
            {
                if (insideQuotes)
                {
                    parts.Add("\"" + line.Substring(start) + "\"");
                }
                else
                {
                    parts.Add(line.Substring(start));
                }
            }

            return parts.ToArray();
        }
    }
}