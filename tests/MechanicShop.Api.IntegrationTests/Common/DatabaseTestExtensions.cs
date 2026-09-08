using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.RepairTasks;

namespace MechanicShop.Api.IntegrationTests.Common;

public static class DatabaseTestExtensions
{
    public static async Task<Customer> SeedCustomerAsync(this IAppDbContext context, CancellationToken ct = default)
    {
        var email = Guid.NewGuid().ToString()[..15] + "@gmail.com";
        var phoneNumber = "+" + string.Join("", Enumerable.Range(1, 11).Select(_ => Random.Shared.Next(1, 10)));

        var customer = CustomerFactory.CreateCustomer(email: email, phoneNumber: phoneNumber, setListIfNull: true).Value;

        context.Customers.Add(customer);

        await context.SaveChangesAsync(ct);

        return customer;
    }

    public static async Task<RepairTask> SeedRepairTaskAsync(this IAppDbContext context, CancellationToken ct = default)
    {
        var partName = $"OilFilter-{Guid.NewGuid().ToString()[..8]}";
        var part = PartFactory.CreatePart(name: partName).Value;

        var taskName = $"OilChange-{Guid.NewGuid().ToString()[..8]}";
        var repairTask = RepairTaskFactory.CreateRepairTask(name: taskName, parts: [part]).Value;

        context.RepairTasks.Add(repairTask);
        await context.SaveChangesAsync(ct);

        return repairTask;
    }
}
