using Microsoft.EntityFrameworkCore;
using Modelos.Models;

namespace ProyectoProgramacionG7.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Caja> Cajas { get; set; }
        public DbSet<Comercio> Comercios { get; set; }
        public DbSet<BitacoraEvento> BitacoraEventos { get; set; }
        public DbSet<Sinpe> Sinpes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Caja>()
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