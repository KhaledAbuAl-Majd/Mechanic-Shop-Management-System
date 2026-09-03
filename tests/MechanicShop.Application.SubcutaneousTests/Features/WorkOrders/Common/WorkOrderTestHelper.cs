using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Domain.WorkOrders.Enums;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.Employees;
using MechanicShop.Tests.Common.RepairTasks;
using MediatR;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Common;

public static class WorkOrderTestHelper
{
    public static async Task<WorkOrderDto> CreateValidWorkOrder(
        IMediator mediator,
        IAppDbContext context,
        CancellationToken cancellationToken = default,
        int hoursOffset = 0,
        Spot spot = Spot.D)
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var labor = EmployeeFactory.CreateLabor().Value;
        var repairTask = RepairTaskFactory.CreateRepairTask().Value;

        context.Customers.Add(customer);
        context.Employees.Add(labor);
        context.RepairTasks.Add(repairTask);
        await context.SaveChangesAsync(cancellationToken);

        var scheduledAt = GetTomorrowOpening().AddHours(hoursOffset);

        var command = new CreateWorkOrderCommand(spot, vehicle.Id, scheduledAt, [repairTask.Id], labor.Id);

        var result = await mediator.Send(command, cancellationToken);

        return result.Value;
    }

    public static DateTimeOffset GetTomorrowOpening()
    {
        var tomorrow = GetTomorrow();

        return new DateTimeOffset(
        tomorrow.ToDateTime(AppSettingsTestData.DefaultOpeningTime),
        TimeSpan.Zero);
    }

    public static DateOnly GetTomorrow() => DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
}
