using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Billing.InvoiceLineItems;

namespace MechanicShop.Tests.Common.WorkOrders.Billing;

public static class InvoiceLineItemFactory
{
    public static Result<InvoiceLineItem> CreateInvoiceLineItem(
        Guid? invoiceId = null,
        int? lineNumber = null,
        string? description = null,
        int? quantity = null,
        decimal? unitPrice = null)
    {
        return InvoiceLineItem.Create(
            invoiceId ?? Guid.NewGuid(),
            lineNumber ?? 1,
            description ?? "invoice line description",
            quantity ?? 3,
            unitPrice ?? 90m);

    }
}
