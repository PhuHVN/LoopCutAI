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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            // Model configurations go here

            
        }
    }
}
//dotnet ef migrations add AddAccountTable -p LoopCut.Infrastructure -s LoopCut.Api
//dotnet ef database update -p LoopCut.Infrastructure -s LoopCut.Api
