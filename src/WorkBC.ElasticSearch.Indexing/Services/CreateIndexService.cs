using Microsoft.Extensions.Configuration;
using System;
using WorkBC.Shared.Utilities;

namespace WorkBC.ElasticSearch.Indexing.Services
{
    public class CreateIndexService
    {
        private readonly ElasticRequestService _elasticRequestService;
        private readonly string _index;
        private readonly string _server;

        public CreateIndexService(string index, IConfiguration configuration)
        {
            _server = configuration["ConnectionStrings:ElasticSearchServer"];
            _index = index;
            _elasticRequestService = new ElasticRequestService(configuration);
        }

        public void Create(bool useSynonymFile = true)
        {
            //Both English and French job index should be exactly the same, so using the same index file for both.
            string structure = ResourceFileHelper.ReadFile("jobs_index.json");

            //Synonym check
            if (useSynonymFile)
            {
                //Use synonyms in an external file (stored in the Elasticsearch 'analysis' folder)
                string synonymFile = ResourceFileHelper.ReadFile("synonym_file.json");
                structure = structure.Replace("##SYNONYM##", synonymFile);
            }
            else
            {
                //Use hard-coded synonyms in the JSON file (for unit tests only)
                string synonymPreDefined = ResourceFileHelper.ReadFile("synonym_predefined.json");
                structure = structure.Replace("##SYNONYM##", synonymPreDefined);
            }

            //Create index
            if (!_elasticRequestService.Send($"{_server}/{_index}", structure, "PUT"))
            {
                string failMessage = $"Failed to create index. {_server}/{_index}.";

                if (useSynonymFile)
                {
                    failMessage +=
                        " This sometimes happens because Elasticsearch isn't running, or the [Elasticsearch Root]\\config\\analysis\\synonym.txt file does not exist on the Elasticsearch server.";
                    throw new Exception(failMessage);
                }
            }
        }
    }
}