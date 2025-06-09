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
            
            // Определяем путь к базе данных
            var dbFolder = Environment.GetEnvironmentVariable("DB_FOLDER") ?? 
                          Path.Combine(Directory.GetCurrentDirectory(), "Database");
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
                    _logger.LogInformation($"SQLite database created at {_dbPath}");
                    _logger.LogInformation($"Database schema created successfully");
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
                
                // Включаем подробное логирование для отладки
                optionsBuilder.LogTo(message => _logger.LogDebug(message))
                             .EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                // Конфигурация первичного ключа
                entity.HasKey(p => p.Id)
                      .HasName("PK_Products");

                // Настройка генерации GUID
                entity.Property(p => p.Id)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("hex(randomblob(16))");

                // Настройка остальных полей
                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.Definition)
                      .HasMaxLength(500);

                entity.Property(p => p.Price)
                      .IsRequired()
                      .HasColumnType("DECIMAL(10,2)");

                entity.Property(p => p.Image)
                      .HasMaxLength(255);

                entity.Property(p => p.Weight)
                      .HasColumnType("DECIMAL(5,2)")
                      .HasDefaultValue(0.1m);

                entity.Property(p => p.Status)
                      .HasDefaultValue("Available");
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error saving changes to database");
                throw;
            }
        }
    }
}
