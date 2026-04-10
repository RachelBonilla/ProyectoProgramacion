using Microsoft.EntityFrameworkCore;
using Modelos.Models;

namespace ProyectoProgramacionG7.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Comercio> Comercios { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Caja> Cajas { get; set; }
        public DbSet<Sinpe> Sinpes { get; set; }
        public DbSet<ConfiguracionComercio> Configuraciones { get; set; }
        public DbSet<ReporteMensual> Reportes { get; set; }
        public DbSet<BitacoraEvento> BitacoraEventos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo a las tablas reales (prefijo G7_)
            modelBuilder.Entity<Comercio>().ToTable("G7_Comercios");
            modelBuilder.Entity<Usuario>().ToTable("G7_Usuarios");
            modelBuilder.Entity<Caja>().ToTable("G7_Cajas");
            modelBuilder.Entity<Sinpe>().ToTable("G7_Sinpes");
            modelBuilder.Entity<ConfiguracionComercio>().ToTable("G7_ConfiguracionesComercio");
            modelBuilder.Entity<ReporteMensual>().ToTable("G7_ReportesMensuales");
            modelBuilder.Entity<BitacoraEvento>().ToTable("G7_Bitacora");

            // Relaciones
            modelBuilder.Entity<Caja>()
                .HasOne(c => c.Comercio)
                .WithMany()
                .HasForeignKey(c => c.IdComercio)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sinpe>()
                .HasOne(s => s.Caja)
                .WithMany()
                .HasForeignKey(s => s.CajaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Comercio)
                .WithMany()
                .HasForeignKey(u => u.IdComercio)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConfiguracionComercio>()
                .HasOne(c => c.Comercio)
                .WithMany()
                .HasForeignKey(c => c.IdComercio)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReporteMensual>()
                .HasOne(r => r.Comercio)
                .WithMany()
                .HasForeignKey(r => r.IdComercio)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}