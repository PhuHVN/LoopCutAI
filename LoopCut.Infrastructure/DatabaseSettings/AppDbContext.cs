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

        public DbSet<Subscriptions> Subcriptions { get; set; }

        public DbSet<ServicePlans> ServicePlans { get; set; }

        public DbSet<ServiceDefinitions> Services { get; set; }

        public DbSet<SubscriptionEmailLog> SubscriptionEmailLogs { get; set; }

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

            modelBuilder.Entity<ServicePlans>(e =>
            {
                e.HasOne<Accounts>()
                 .WithMany(a => a.ModifiedServicePlans)
                 .HasForeignKey(s => s.ModifiedByID);
            });

            modelBuilder.Entity<ServiceDefinitions>(e =>
            {
                e.HasOne<Accounts>()
                 .WithMany(a => a.ModifiedServices)
                 .HasForeignKey(s => s.ModifiedByID);
            });

        }
    }
}
//dotnet ef migrations add AddAccountTable -p LoopCut.Infrastructure -s LoopCut.Api
//dotnet ef database update -p LoopCut.Infrastructure -s LoopCut.Api
