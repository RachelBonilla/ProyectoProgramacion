using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionG7.Data;
using ProyectoProgramacionG7.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var connectionString = builder.Configuration.GetConnectionString("MysqlConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseMySql(
//         builder.Configuration.GetConnectionString("MysqlConnection"),
//         ServerVersion.AutoDetect(
//             builder.Configuration.GetConnectionString("MysqlConnection")
//         )
//     );
// });


// Servicio de Bitácora
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

//Repositorio
//CapaBussine

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
