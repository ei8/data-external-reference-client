using NLog;
using neurUL.Common.Http;
using Polly;
using Splat;
using System;
using System.Threading;
using System.Threading.Tasks;
using ei8.Data.ExternalReference.Common;

namespace ei8.Data.ExternalReference.Client.Out
{
    public class HttpExternalReferenceQueryClient : IExternalReferenceQueryClient
    {
        private readonly IRequestProvider requestProvider;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => HttpExternalReferenceQueryClient.logger.Error(ex, "Error occurred while communicating with ei8 ExternalReference. " + ex.InnerException?.Message)
            );
        private static readonly string GetExternalReferencesPathTemplate = "data/externalreferences";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public HttpExternalReferenceQueryClient(IRequestProvider requestProvider = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
        }

        public async Task<ItemData> GetItemById(string outBaseUrl, string id, CancellationToken token = default(CancellationToken)) =>
           await HttpExternalReferenceQueryClient.exponentialRetryPolicy.ExecuteAsync(
               async () => await this.GetItemByIdInternal(outBaseUrl, id, token).ConfigureAwait(false));
        
        private async Task<ItemData> GetItemByIdInternal(string outBaseUrl, string id, CancellationToken token = default)
        {
            return await requestProvider.GetAsync<ItemData>(
                           $"{outBaseUrl}{HttpExternalReferenceQueryClient.GetExternalReferencesPathTemplate}/{id}",
                           token: token
                           );
        }
    }
}
