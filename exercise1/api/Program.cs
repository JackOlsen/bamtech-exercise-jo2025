using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Logging;
using StargateAPI.Business.Services;
using StargateAPI.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options => options.Filters.Add<ExceptionFilter>());

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddDbContext<StargateContext>(options => 
        options.UseSqlite(builder.Configuration.GetConnectionString("StarbaseApiDatabase")))
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly))
    .AddScoped<PersonAstronautService>()
    .AddScoped<ProcessLoggingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ProcessLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }