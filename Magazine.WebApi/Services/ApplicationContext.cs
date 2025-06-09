using Microsoft.EntityFrameworkCore;
using Magazine.Core.Models;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Magazine.WebApi.Services
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        private readonly ILogger<ApplicationContext> _logger;
        private readonly string _dbPath;

        public ApplicationContext(
            DbContextOptions<ApplicationContext> options,
            ILogger<ApplicationContext> logger) : base(options)
        {
            _logger = logger;
            
            // Путь к базе данных (работает и в Docker, и локально)
            var dbFolder = Environment.GetEnvironmentVariable("DB_FOLDER") ?? Path.Combine(Directory.GetCurrentDirectory(), "Database");
            _dbPath = Path.Combine(dbFolder, "products.db");
            
            // Создаем папку если не существует
            if (!Directory.Exists(dbFolder))
            {
                Directory.CreateDirectory(dbFolder);
                _logger.LogInformation($"Created database directory at {dbFolder}");
            }

            try
            {
                if (Database.EnsureCreated())
                {
                    _logger.LogInformation($"Database created at {_dbPath}");
                }
                else
                {
                    _logger.LogInformation($"Using existing database at {_dbPath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database initialization failed");
                throw;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={_dbPath}");
                optionsBuilder.LogTo(message => _logger.LogDebug(message))
                             .EnableSensitiveDataLogging();
            }
        }

        // ... остальной код OnModelCreating ...
    }
}
