using WorkBC.ElasticSearch.Search.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace WorkBC.Tests
{
    public class QueryParsingTests
    {
        public QueryParsingTests(ITestOutputHelper output)
        {
        }

        [Fact]
        public void ParseAndsAndOrs()
        {
            string query = KeywordParsing.BuildSimpleQueryString("family caregiver,child caregiver");
            Assert.True(query == "(family caregiver)|(child caregiver)", query);

            query = KeywordParsing.BuildSimpleQueryString("family caregiver or child caregiver");
            Assert.True(query == "(family caregiver)|(child caregiver)", query);

            query = KeywordParsing.BuildSimpleQueryString("family caregiver | child caregiver");
            Assert.True(query == "(family caregiver)|(child caregiver)", query);

            query = KeywordParsing.BuildSimpleQueryString("family caregiver child,caregiver");
            Assert.True(query == "(family caregiver child)|caregiver", query);

            query = KeywordParsing.BuildSimpleQueryString("family,caregiver child,caregiver");
            Assert.True(query == "family|(caregiver child)|caregiver", query);

            query = KeywordParsing.BuildSimpleQueryString("family,caregiver child caregiver");
            Assert.True(query == "family|(caregiver child caregiver)", query);

            query = KeywordParsing.BuildSimpleQueryString(",family caregiver child caregiver");
            Assert.True(query == "family caregiver child caregiver", query);

            query = KeywordParsing.BuildSimpleQueryString("family and caregiver and child and caregiver");
            Assert.True(query == "family caregiver child caregiver", query);
        }

        [Fact]
        public void ParseLine()
        {
            string[] query = KeywordParsing.SplitQuotedSegments(@"""hello, world"" 123");

            Assert.True(query[0] == "\"hello, world\"");
            Assert.True(query[1] == "123");

            query = KeywordParsing.SplitQuotedSegments(@"hello world ""123""");

            Assert.True(query[0] == "hello");
            Assert.True(query[1] == "world");
            Assert.True(query[2] == "\"123\"");
        }

        [Fact]
        public void ParseMultipleWords()
        {
            string query = KeywordParsing.BuildSimpleQueryString("test 123");
            Assert.True(query == "test 123");

            query = KeywordParsing.BuildSimpleQueryString("test 123 hello");
            Assert.True(query == "test 123 hello", query);
        }

        [Fact]
        public void ParseSingleWord()
        {
            string query = KeywordParsing.BuildSimpleQueryString("test");
            Assert.True(query == "test", query);
        }

        [Fact]
        public void ParseWordAndPhrase()
        {
            string query = KeywordParsing.BuildSimpleQueryString(@"""test 123"" hello");
            Assert.True(query == "\"test 123\" hello", query);

            query = KeywordParsing.BuildSimpleQueryString(@"""test, 123"" hello");
            Assert.True(query == "\"test , 123\" hello", query);
        }

        [Fact]
        public void SanitizeKeywords()
        {
            string sanitized = KeywordParsing.SanitizeKeywords("Parentheses Squiggle ()~{} Brace!");
            Assert.True(sanitized == "Parentheses Squiggle Brace", sanitized);

            sanitized = KeywordParsing.SanitizeKeywords("* # Asterisk Hash");
            Assert.True(sanitized == "* Asterisk Hash", sanitized);

            sanitized = KeywordParsing.SanitizeKeywords("\r\nWhite\tSpace.\f");
            Assert.True(sanitized == "White Space", sanitized);

            sanitized = KeywordParsing.SanitizeKeywords("\"Hyphen-Pipe|Comma,Quote\"Apostrophe'");
            Assert.True(sanitized == "\"Hyphen-Pipe,Comma,Quote\"Apostrophe'", sanitized);

            sanitized = KeywordParsing.SanitizeKeywords("é");
            Assert.True(sanitized == "é", sanitized);

            sanitized = KeywordParsing.SanitizeKeywords("丽枫");
            Assert.True(sanitized == "丽枫", sanitized);

            sanitized = KeywordParsing.SanitizeKeywords("data_analyst");
            Assert.True(sanitized == "data_analyst", sanitized);
        }
    }
}