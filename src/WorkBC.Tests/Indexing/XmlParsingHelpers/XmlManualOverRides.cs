using System.Collections.Generic;

namespace WorkBC.ElasticSearch.Indexing.ParsingHelpers
{
    public static class XmlManualOverRides
    {
        /// <summary>
        ///     These are cities found in the Wanted data or Federal data where the name was not
        ///     available in the EDM_Location table of the WorkBC_Enterprise db.
        ///     If you add values to this  dictionary, USE LOWERCASE FOR THE KEY!!!
        /// </summary>
        public static Dictionary<string, string> AlternateCityNames = new Dictionary<string, string>
        {
            {"one hundred mile house", "100 Mile House"},
            {"queen charlotte", "Daajing Giids ( Queen Charlotte City )"},
            {"queen charlotte city", "Daajing Giids ( Queen Charlotte City )"},
            {"cowichan valley a", "Lake Cowichan"},
            {"cowichan", "Lake Cowichan"},
            {"denny island", "Bella Bella"},
            {"barrière", "Barriere"},
            {"garden", "Garden Village"}
        };
    }
}