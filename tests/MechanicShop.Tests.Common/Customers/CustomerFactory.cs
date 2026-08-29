using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;

namespace MechanicShop.Tests.Common.Customers;

public static class CustomerFactory
{
    public static Result<Customer> CreateCustomer(
        Guid? id = null,
        string? name = null,
        string? phoneNumber = null,
        string? email = null,
        List<Vehicle>? vehicles = null)
    {
        return Customer.Create(
            id ?? Guid.NewGuid(),
            name ?? "khaled abu al-majd",
            phoneNumber ?? "+202358933",
            email ?? "khaledabualmajd06@gmail.com",
            vehicles ?? [VehicleFactory.CreateVehicle().Value]);
    }
}
