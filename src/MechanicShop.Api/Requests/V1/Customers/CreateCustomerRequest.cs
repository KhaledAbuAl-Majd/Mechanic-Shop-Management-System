namespace MechanicShop.Api.Requests.V1.Customers
{
    public sealed record CreateCustomerRequest(string Name, string PhoneNumber, string Email, List<CreateVehicleRequest> Vehicles);

    public sealed record CreateVehicleRequest(string Make, string Model, int Year, string LicensePlate);

    public sealed record UpdateCustomerRequest(string Name, string PhoneNumber, string Email, List<UpdateVehicleRequest> Vehicles);

    public sealed record UpdateVehicleRequest(Guid VehicleId, string Make, string Model, int Year, string LicensePlate);
}
