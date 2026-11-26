using AuthService;
using AuthService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/auth-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = Config.Settings["Keycloak:Authority"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        Log.Error(feature?.Error, "Необработанная ошибка при запросе {Path}", context.Request.Path);

        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Произошла ошибка. Попробуйте позже.");
    });
});

using (var context = new AppDbContext())
{
    context.Database.Migrate();
}

Log.Information("AuthService запущен");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run(Config.Settings["ListenUrl"]);