using QueryBasedProducer.Models;
using System.Threading.Tasks;

namespace QueryBasedProducer.Repositories
{
    public interface IPurchaseOrderProducerRepository
    {
        Task<PurchaseOrder> GetPurchaseOrderForProcessingAsync();
        Task TombstonePurchaseOrder(int id);
    }
}
