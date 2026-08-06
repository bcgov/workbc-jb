using Microsoft.Extensions.Configuration;
using WorkBC.Shared.Constants;

namespace WorkBC.ElasticSearch.Indexing.Services
{
    public class DeleteIndexService
    {
        private readonly ElasticRequestService _requestService;
        private readonly string _server;

        public DeleteIndexService(IConfiguration configuration)
        {
            _server = configuration["ConnectionStrings:ElasticSearchServer"];
            _requestService = new ElasticRequestService(configuration);
        }

        public void Delete(string[] indexes = null)
        {
            if (indexes == null)
            {
                indexes = General.Indexes;
            }

            //loop through each index
            foreach (string indexName in indexes)
            {
                //delete index
                _requestService.Send($"{_server}/{indexName}", string.Empty, "DELETE");
            }
        }
    }
}