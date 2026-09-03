using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Commands.DeleteWorkOrder;
using MechanicShop.Application.SubcutaneousTests.Common;
using MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.SubcutaneousTests.Features.WorkOrders.Commands.DeleteWorkOrder
{
    [Collection(WebAppFactoryCollection.CollectionName)]
    public class DeleteWorkOrderCommandHandlerTests(WebAppFactory factory) : IAsyncLifetime
    {
        private readonly IMediator _mediator = factory.CreateMediator();
        private readonly IAppDbContext _context = factory.CreateAppDbContext();

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
        public async Task InitializeAsync()
        {
            await factory.ResetDatabaseAsync();
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenWorkOrderNotFound()
        {
            var ct = CancellationToken.None;
            var command = new DeleteWorkOrderCommand(Guid.NewGuid());

            var result = await _mediator.Send(command, ct);

            Assert.False(result.IsSuccess);
            Assert.Equal(ApplicationErrors.WorkOrderNotFound.Code, result.TopError.Code);
        }


        [Theory]
        [InlineData(WorkOrderState.InProgress, 0)]
        [InlineData(WorkOrderState.Completed, 1)]
        [InlineData(WorkOrderState.Cancelled, 2)]
        public async Task Handle_ShouldFail_WhenReadOnly(WorkOrderState state, int hoursOffset)
        {
            var ct = CancellationToken.None;

            var workOrderDto = await WorkOrderTestHelper.CreateValidWorkOrder(_mediator, _context, ct, hoursOffset: hoursOffset);

            var workOrder = await _context.WorkOrders.FirstOrDefaultAsync(wo => wo.Id == workOrderDto.WorkOrderId, ct);

            ArgumentNullException.ThrowIfNull(workOrder);

            if (state is WorkOrderState.InProgress)
                workOrder.UpdateState(WorkOrderState.InProgress);

            if (state is WorkOrderState.Completed)
            {
                workOrder.UpdateState(WorkOrderState.InProgress);
                workOrder.UpdateState(WorkOrderState.Completed);
            }

            //it's business rules
            if (state is WorkOrderState.Cancelled)
            {
                workOrder.UpdateState(WorkOrderState.Cancelled);
            }

            var saveResult = await _context.SaveChangesAsync(ct);

            var command = new DeleteWorkOrderCommand(workOrderDto.WorkOrderId);

            var result = await factory.SendAsync(command, ct);

            Assert.False(result.IsSuccess);
            Assert.Equal(WorkOrderErrors.Readonly.Code, result.TopError.Code);
        }

        [Fact]
        public async Task Handle_ShouldSuccess_WhenValidData()
        {
            var ct = CancellationToken.None;

            var workOrderDto = await WorkOrderTestHelper.CreateValidWorkOrder(_mediator, _context, ct,hoursOffset:5,spot:Spot.B);

            var command = new DeleteWorkOrderCommand(workOrderDto.WorkOrderId);

            var result = await _mediator.Send(command, ct);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Deleted, result.Value);

            var exists = await _context.WorkOrders.AnyAsync(wo => wo.Id == workOrderDto.WorkOrderId, ct);
            Assert.False(exists);
        }

    }
}
