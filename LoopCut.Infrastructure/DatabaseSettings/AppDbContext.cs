using LoopCut.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Infrastructure.DatabaseSettings
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        //DbSets 
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<UserMembership> UserMemberships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            // Model configurations go here

            modelBuilder.Entity<Accounts>().ToTable("users");

            modelBuilder.Entity<Membership>(e =>
            {
                e.ToTable("memberships");
                e.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<UserMembership>(e =>
            {
                e.ToTable("user_memberships");
            });
        }
    }
}
//dotnet ef migrations add AddAccountTable -p LoopCut.Infrastructure -s LoopCut.Api
//dotnet ef database update -p LoopCut.Infrastructure -s LoopCut.Api
