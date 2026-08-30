using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;

namespace MechanicShop.Tests.Common.Customers;

public static class CustomerFactory
{
    public static Result<Customer> CreateCustomer(
        Guid? id = null,
        string? name = "khaled abu al-majd",
        string? phoneNumber = "+202358933",
        string? email = "khaledabualmajd06@gmail.com",
        List<Vehicle>? vehicles = null)
    {
        return Customer.Create(
            id ?? Guid.NewGuid(),
            name!,
            phoneNumber!,
            email!,
            vehicles ?? [VehicleFactory.CreateVehicle().Value]);
    }

}
