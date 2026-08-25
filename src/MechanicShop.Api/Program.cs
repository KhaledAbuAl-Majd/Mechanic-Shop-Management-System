using MechanicShop.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   await app.InitialiseDatabaseAsync();
}

app.UseCoreMiddlewares(builder.Configuration);

app.MapControllers();

app.Run();
