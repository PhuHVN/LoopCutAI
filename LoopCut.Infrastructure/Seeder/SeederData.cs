using LoopCut.Domain.Entities;
using LoopCut.Infrastructure.DatabaseSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace LoopCut.Infrastructure.Seeder
{
    public class SeederData
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public SeederData(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task SeedAdminAccountAsync()
        {
            var adminEmail = _configuration["SeederData:DefaultAdminEmail"];
            var existingAdmin = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == adminEmail && a.Role == Domain.Enums.RoleEnum.Admin && a.Status == Domain.Enums.StatusEnum.Active);
            if (existingAdmin == null)
            {
                var adminAccount = new Accounts
                {
                    Email = adminEmail,
                    Password = BCrypt.Net.BCrypt.HashPassword(_configuration["SeederData:DefaultAdminPassword"]),
                    FullName = "Administrator",
                    CreatedAt = DateTime.UtcNow,
                    Role = Domain.Enums.RoleEnum.Admin,
                    Status = Domain.Enums.StatusEnum.Active
                };
                await _context.Accounts.AddAsync(adminAccount);
                await _context.SaveChangesAsync();
            }
        }

    }
}
