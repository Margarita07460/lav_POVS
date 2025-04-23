using Microsoft.EntityFrameworkCore;
using Magazine.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Magazine.WebApi.Services
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        private readonly ILogger<ApplicationContext> _logger;
        private readonly IConfiguration _configuration;

        public ApplicationContext(
            DbContextOptions<ApplicationContext> options,
            ILogger<ApplicationContext> logger,
            IConfiguration configuration) : base(options)
        {
            _logger = logger;
            _configuration = configuration;

            try
            {
                // Важно: используем только EnsureCreated без миграций
                if (Database.EnsureCreated())
                {
                    _logger.LogInformation("Database and tables created successfully");
                }
                else
                {
                    _logger.LogInformation("Database already exists");
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
                optionsBuilder.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
            }

            // Включаем логирование только для разработки
            if (_configuration.GetValue<bool>("Logging:EnableDbLogging", false))
            {
                optionsBuilder.LogTo(message => _logger.LogDebug(message))
                             .EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                // Явное указание первичного ключа
                entity.HasKey(p => p.Id)
                      .HasName("PK_Products");

                // Указываем, что Id генерируется при добавлении
               entity.Property(p => p.Id)
                .HasColumnName("Id");
                //.ValueGeneratedOnAdd()
                //.HasDefaultValueSql("hex(randomblob(16))");


                // Настройки полей
                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.Definition)
                      .HasMaxLength(500);  // Добавлено ограничение длины

                entity.Property(p => p.Price)
                      .IsRequired()
                      .HasColumnType("DECIMAL(10,2)");

                entity.Property(p => p.Image)
                      .HasMaxLength(255);  // Добавлено ограничение длины

                entity.Property(p => p.Weight)
                      .HasColumnType("DECIMAL(5,2)")  // Исправлено на DECIMAL
                      .HasDefaultValue(0.1m);

                //// Индексы
                //entity.HasIndex(p => p.Id)
                //      .HasDatabaseName("IX_Products_Id")
                //      .IsUnique();

                //entity.HasIndex(p => p.Name)
                //      .HasDatabaseName("IX_Products_Name");
            });
        }
    }
}