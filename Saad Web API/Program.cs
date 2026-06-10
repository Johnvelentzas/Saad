using Microsoft.EntityFrameworkCore;
using Saad_Web_API.Data;
using Saad_Web_API.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.AddOpenApi();

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("SaadDb"));
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IProductionWorkflowService, ProductionWorkflowService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// --- AUTO MIGRATION START ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // This looks at the database and applies any new changes automatically
        context.Database.Migrate();

        // If you are still using your test data seeder, keep this here:
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}
// --- AUTO MIGRATION END ---

app.Run();
