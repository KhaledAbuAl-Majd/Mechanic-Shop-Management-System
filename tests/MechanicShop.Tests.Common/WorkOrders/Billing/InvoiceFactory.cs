using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Domain.WorkOrders.Billing.InvoiceLineItems;

namespace MechanicShop.Tests.Common.WorkOrders.Billing;

public static class InvoiceFactory
{
    public static Result<Invoice> CreateInvoice(
        Guid? id = null,
        Guid? workOrderId = null,
        List<InvoiceLineItem>? items = null,
        decimal? discountAmount = null,
        decimal? taxAmount = null,
        TimeProvider? datetime = null)
    {
        return Invoice.Create(
            id ?? Guid.NewGuid(),
            workOrderId ?? Guid.NewGuid(),
            items ?? [InvoiceLineItemFactory.CreateInvoiceLineItem().Value],
            discountAmount ?? 0,
            taxAmount ?? 0,
            datetime ?? TimeProvider.System);
    }
}