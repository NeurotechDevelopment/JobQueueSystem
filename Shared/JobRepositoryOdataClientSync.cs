using Contracts;
using RestSharp;
using Shared.Queries;

namespace Shared
{
    public partial class JobRepositoryClient
    {
        #region ODATA

        public IEnumerable<Job> QueryJobs(JobOdataQueryBuilder? queryBuilder = null)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var odataFilterParams = queryBuilder?.ToRequestParams();
                var request = new RestRequest(OdataJobApiResource);

                if (odataFilterParams != null)
                {
                    foreach (var odataRequestParam in odataFilterParams)
                    {
                        request.AddParameter(odataRequestParam.Key, odataRequestParam.Value);
                    }
                }

                var response = client.Get<ODataJobResponse>(request);
                return response.Value;
            }
        }

        public int CountJobs(JobOdataQueryBuilder? queryBuilder = null)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var odataFilterParams = queryBuilder?.ToRequestParams();
                var request = new RestRequest($"{OdataJobApiResource}/$count");

                foreach (var odataRequestParam in odataFilterParams)
                {
                    request.AddParameter(odataRequestParam.Key, odataRequestParam.Value);
                }

                var response = client.Get(request);
                return int.Parse(response.Content);
            }
        }

        #endregion
    }
}
