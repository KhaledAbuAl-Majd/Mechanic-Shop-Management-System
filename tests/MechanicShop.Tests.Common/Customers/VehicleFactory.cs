using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;

namespace MechanicShop.Tests.Common.Customers;

public static class VehicleFactory
{
    public static Result<Vehicle> CreateVehicle(
        Guid? id = null,
        string? make = null,
        string? model = null,
        int? year = null,
        string? licensePlate = null)
    {
        return Vehicle.Create(
            id ?? Guid.NewGuid(),
            make ?? "BMW",
            model ?? "M5",
            year ?? 2025,
            licensePlate ?? "tec 353");
    }
}
