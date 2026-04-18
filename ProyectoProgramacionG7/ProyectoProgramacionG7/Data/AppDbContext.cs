using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modelos.Models;

namespace ProyectoProgramacionG7.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Caja> Cajas { get; set; }
        public DbSet<Comercio> Comercios { get; set; }
        public DbSet<BitacoraEvento> BitacoraEventos { get; set; }
        public DbSet<Sinpe> Sinpes { get; set; }
        public DbSet<ReporteMensual> Reportes { get; set; }
        public DbSet<ConfiguracionComercio> Configuraciones { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo de tablas Identity
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUser>().ToTable("G7_AspNetUsers");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().ToTable("G7_AspNetRoles");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("G7_AspNetUserRoles");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("G7_AspNetUserClaims");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("G7_AspNetUserLogins");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("G7_AspNetUserTokens");
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("G7_AspNetRoleClaims");

            // Mapeo a las tablas en MySQL
            modelBuilder.Entity<Comercio>().ToTable("G7_Comercios");
            modelBuilder.Entity<Usuario>().ToTable("G7_Usuarios");
            modelBuilder.Entity<Caja>().ToTable("G7_Cajas");
            modelBuilder.Entity<Sinpe>().ToTable("G7_Sinpes");
            modelBuilder.Entity<ConfiguracionComercio>().ToTable("G7_ConfiguracionesComercio");
            modelBuilder.Entity<ReporteMensual>().ToTable("G7_ReportesMensuales");
            modelBuilder.Entity<BitacoraEvento>().ToTable("G7_Bitacora");


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

            modelBuilder.Entity<ConfiguracionComercio>()
                .HasOne(c => c.Comercio)
                .WithMany()
                .HasForeignKey(c => c.IdComercio)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Caja>(entity =>
            {
                entity.Property(e => e.Nombre).IsRequired(false);
                entity.Property(e => e.Descripcion).IsRequired(false);
                entity.Property(e => e.TelefonoSINPE).IsRequired(false);
            });
        }
    }
}