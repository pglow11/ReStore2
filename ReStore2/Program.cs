using Microsoft.EntityFrameworkCore;
using ReStore2.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// For SqlLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StoreContext>(opt => opt.UseSqlite(connectionString));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        context.Database.Migrate();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Problem migrating data");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
