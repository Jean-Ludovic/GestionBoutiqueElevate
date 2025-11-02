using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IOrderRepository _orders;
        public InvoiceService(IOrderRepository orders) { _orders = orders; }

        public async Task<byte[]> RenderOrderPdfAsync(int orderId)
        {
            var o = await _orders.GetAsync(orderId);
            if (o == null) return Array.Empty<byte>();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text($"Facture #{o.Id}").FontSize(20).SemiBold();
                    page.Content().Column(c =>
                    {
                        c.Item().Text($"Client : {o.ClientName}");
                        c.Item().Text($"Employé : {o.EmployeeName} ({o.EmployeeCode})");
                        c.Item().Text($"Paiement : {o.PaymentMethod}");
                        c.Item().Table(t =>
                        {
                            t.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2);
                                cols.RelativeColumn();
                                cols.ConstantColumn(60);
                                cols.ConstantColumn(80);
                            });
                            t.Header(h =>
                            {
                                h.Cell().Text("Produit").SemiBold();
                                h.Cell().Text("SKU").SemiBold();
                                h.Cell().AlignRight().Text("Qté").SemiBold();
                                h.Cell().AlignRight().Text("Total").SemiBold();
                            });
                            foreach (var it in o.Items)
                            {
                                t.Cell().Text(it.Name);
                                t.Cell().Text(it.Sku);
                                t.Cell().AlignRight().Text(it.Quantity.ToString());
                                t.Cell().AlignRight().Text((it.UnitPrice * it.Quantity).ToString("C"));
                            }
                        });
                        c.Item().Row(r =>
                        {
                            r.RelativeItem();
                            r.ConstantItem(220).Column(cc =>
                            {
                                cc.Item().Row(rr => { rr.RelativeItem().Text("Sous-total"); rr.ConstantItem(100).AlignRight().Text(o.Subtotal.ToString("C")); });
                                cc.Item().Row(rr => { rr.RelativeItem().Text("Remise"); rr.ConstantItem(100).AlignRight().Text(o.DiscountAmount.ToString("C")); });
                                cc.Item().Row(rr => { rr.RelativeItem().Text($"Taxes ({o.TaxRate * 100:F0}%)"); rr.ConstantItem(100).AlignRight().Text(o.Tax.ToString("C")); });
                                cc.Item().LineHorizontal(1);
                                cc.Item().Row(rr => { rr.RelativeItem().Text("Total"); rr.ConstantItem(100).AlignRight().Text(o.Total.ToString("C")); });
                            });
                        });
                    });
                });
            }).GeneratePdf();

            return pdf;
        }
    }
}
