using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Skysense_persistencia.EntidadesBateriasMonitoreo
{
    public class BateriasMonitoreoContext:DbContext
    {
        private readonly string _connectionString;

        public BateriasMonitoreoContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        public DbSet<MedicionesBess> MedicionesBess { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MedicionesBess>()
                .HasKey(m => m.IdMedicion);

        }
    }
}
