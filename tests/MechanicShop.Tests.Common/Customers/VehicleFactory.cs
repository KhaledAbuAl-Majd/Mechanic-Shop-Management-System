using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;

namespace MechanicShop.Tests.Common.Customers;

public static class VehicleFactory
{
    public static Result<Vehicle> CreateVehicle(
        Guid? id = null,
        string? make = "BMW",
        string? model = "M5",
        int? year = null,
        string? licensePlate = "tec 353")
    {
        return Vehicle.Create(
            id ?? Guid.NewGuid(),
            make!,
            model!,
            year ?? 2025,
            licensePlate!);
    }
}
