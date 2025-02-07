using Microsoft.EntityFrameworkCore;
using PrimeVilla_VillaAPI;
using PrimeVilla_VillaAPI.Data;
using PrimeVilla_VillaAPI.Repository;
using PrimeVilla_VillaAPI.Repository.IRepository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//SeriLog
//Log.Logger = new LoggerConfiguration().MinimumLevel.Warning()
//    .WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Hour).CreateLogger();
//builder.Host.UseSerilog();

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSqlConnection"));
});

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services
.AddControllers(option => {
    //option.ReturnHttpNotAcceptable = true;
})
.AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();

var app = builder.Build();

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
