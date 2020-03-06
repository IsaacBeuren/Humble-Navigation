using Br.Scania.ExternalAGV.Model;
using Br.Scania.ExternalAGV.Model.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Br.Scania.ExternalAGV.Data
{
    public partial class dataContext : DbContext
    {
        public dataContext()
        {
        }

        public dataContext(DbContextOptions<dataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PointsModel> Point { get; set; }
        public virtual DbSet<ConfigPointsModel> ConfigPoints { get; set; }
        public virtual DbSet<ConfigModel> Config { get; set; }
        public virtual DbSet<LastPositionModel> LastPosition { get; set; }
        public virtual DbSet<RouteModel> Route { get; set; }
        public virtual DbSet<CallsModel> Calls { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();

                string connectionString = configuration.GetSection("DataBase").GetSection("connectionString").Value;

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<PointsModel>(entity =>
            {
                entity.HasKey(e => new { e.Lat,e.Lng});
            });

            modelBuilder.Entity<ConfigPointsModel>(entity =>
            {
                entity.HasKey(e => new { e.ID });
            });

            modelBuilder.Entity<ConfigModel>(entity =>
            {
                entity.HasKey(e => new { e.ID});
            });

            modelBuilder.Entity<LastPositionModel>(entity =>
            {
                entity.HasKey(e => new { e.ID });
            });

            modelBuilder.Entity<RouteModel>(entity =>
            {
                entity.HasKey(e => new { e.ID });
            });

            modelBuilder.Entity<CallsModel>(entity =>
            {
                entity.HasKey(e => new { e.ID });
            });
        }
    }
}
