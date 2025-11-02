namespace GestionBoutiqueElevate.Services
{
    public interface IInvoiceService
    {
        Task<byte[]> RenderOrderPdfAsync(int orderId);
    }
}
