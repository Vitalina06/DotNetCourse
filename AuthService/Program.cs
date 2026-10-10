using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/auth-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

using (var context = new AppDbContext())
{
    context.Database.Migrate();
}

Log.Information("AuthService запущен");

app.MapControllers();
app.Run("http://localhost:5100");
