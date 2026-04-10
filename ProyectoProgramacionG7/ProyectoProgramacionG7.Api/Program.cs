using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionG7.Api.Data;
using ProyectoProgramacionG7.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conexión a MySQL
var connectionString = builder.Configuration.GetConnectionString("MysqlConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Servicio de Bitácora
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();