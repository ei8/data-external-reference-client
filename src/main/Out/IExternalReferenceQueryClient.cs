using System.Threading;
using System.Threading.Tasks;
using ei8.Data.ExternalReference.Common;

namespace ei8.Data.ExternalReference.Client.Out
{
    public interface IExternalReferenceQueryClient
    {
        Task<ItemData> GetItemById(string outBaseUrl, string id, CancellationToken token = default(CancellationToken)); 
    }
}
