using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProyectoProgramacionG7.Data;
using ProyectoProgramacionG7.Services;
//using ProyectoProgramacionG7.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySQL(
    builder.Configuration.GetConnectionString("MysqlConnection")
);
});



//Repositorio
//CapaBussine

builder.Services.AddScoped<IBitacoraService, BitacoraService>();
builder.Services.AddScoped<ISinpeService, SinpeService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.UseDeveloperExceptionPage();
}
else
{
app.UseExceptionHandler("/Home/Error");
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
