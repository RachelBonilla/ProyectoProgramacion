using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Modelos.Models;
using ProyectoProgramacionG7.Models;
using Microsoft.EntityFrameworkCore;

namespace ProyectoProgramacionG7.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablas del comercio
        public DbSet<Comercio> Comercios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
        }
    }
}

