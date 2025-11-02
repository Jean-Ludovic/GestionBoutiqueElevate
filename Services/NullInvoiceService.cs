using System;
using System.Threading.Tasks;

namespace GestionBoutiqueElevate.Services
{
    public class NullInvoiceService : IInvoiceService
    {
        public Task<byte[]> RenderOrderPdfAsync(int orderId)
            => Task.FromResult(Array.Empty<byte>());
    }
}
