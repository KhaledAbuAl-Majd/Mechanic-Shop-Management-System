using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Commands.AssignLabor;
using MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enums;
using MechanicShop.Tests.Common.Customers;
using MechanicShop.Tests.Common.Employees;
using MechanicShop.Tests.Common.RepairTasks;
using MediatR;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.AssignLabor;

[Collection(WebAppFactoryCollection.CollectionName)]
public class AssignLaborCommandHandlerTests(WebAppFactory factory) : IAsyncLifetime
{
    private readonly IMediator _mediator = factory.CreateMediator();
    private readonly IAppDbContext _context = factory.CreateAppDbContext();

    public async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
    }
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }


    [Fact]
    public async Task Handle_ShouldSuccess_WhenValidData()
    {
        var cancellationToken = CancellationToken.None;
        var workOrderDto = await CreateValidWorkOrder(cancellationToken, hoursOffset: 3);

        var labor = EmployeeFactory.CreateLabor().Value;
        _context.Employees.Add(labor);
        await _context.SaveChangesAsync(cancellationToken);

        var command = new AssignLaborCommand(workOrderDto.WorkOrderId, labor.Id);

        var result = await _mediator.Send(command, cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(Result.Updated, result.Value);
    }


    [Fact]
    public async Task Handle_ShouldFail_WhenWorkOrderNotFound()
    {
        var cancellationToken = CancellationToken.None;

        var labor = EmployeeFactory.CreateLabor().Value;
        _context.Employees.Add(labor);
        await _context.SaveChangesAsync(cancellationToken);

        var command = new AssignLaborCommand(Guid.NewGuid(), labor.Id);

        var result = await _mediator.Send(command, cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrors.WorkOrderNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenLaborNotFound()
    {
        var cancellationToken = CancellationToken.None;
        var workOrderDto = await CreateValidWorkOrder(cancellationToken);

        var command = new AssignLaborCommand(workOrderDto.WorkOrderId, Guid.NewGuid());

        var result = await _mediator.Send(command, cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(ApplicationErrors.LaborNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenLaborConflict()
    {
        var cancellationToken = CancellationToken.None;

        //same time - different spots and labors
        var workOrderDto1 = await CreateValidWorkOrder(cancellationToken, spot: Spot.A);
        var workOrderDto2 = await CreateValidWorkOrder(cancellationToken, spot: Spot.B);

        var command = new AssignLaborCommand(workOrderDto2.WorkOrderId, workOrderDto1.Labor!.LaborId);

        var result = await _mediator.Send(command, cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal("Labor.Occupied", result.TopError.Code);
    }



    private async Task<WorkOrderDto> CreateValidWorkOrder(CancellationToken cancellationToken, int hoursOffset = 0, Spot spot = Spot.D)
    {
        var customer = CustomerFactory.CreateCustomer().Value;
        var vehicle = customer.Vehicles.First();
        var labor = EmployeeFactory.CreateLabor().Value;
        var repairTask = RepairTaskFactory.CreateRepairTask().Value;

        _context.Customers.Add(customer);
        _context.Employees.Add(labor);
        _context.RepairTasks.Add(repairTask);
        await _context.SaveChangesAsync(cancellationToken);

        var scheduledAt = GetTommoryOpening().AddHours(hoursOffset);

        var command = new CreateWorkOrderCommand(spot, vehicle.Id, scheduledAt, [repairTask.Id], labor.Id);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Value;
    }

    public static DateTimeOffset GetTommoryOpening()
    {
        var tomorrow = GetTomorrow();

        return new DateTimeOffset(
        tomorrow.ToDateTime(AppSettingsTestData.DefaultOpeningTime),
        TimeSpan.Zero);
    }

    public static DateOnly GetTomorrow() => DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
}
