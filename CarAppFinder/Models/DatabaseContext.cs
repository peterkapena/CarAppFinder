using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using static CarAppFinder.Models.Pub.Pub;

namespace CarAppFinder.Models
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DbSet<ErrorLog> Errors { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Coordinates> Coordinates { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Identity Customizd entities
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(name: "User");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRole");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaim");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogin");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaim");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserToken");
            });

            modelBuilder.Entity<Car>()
                .HasIndex(c => new { c.TrackerId, c.Id })
                .IsUnique(true);
            modelBuilder.Entity<Coordinates>()
                 .HasKey(c => new { c.Time, c.CarId });

        }
    }
    public class ConnectionString
    {
        public string Dev { get; set; }
        public string Test { get; set; }
        public string Prod { get; set; }
    }
}
