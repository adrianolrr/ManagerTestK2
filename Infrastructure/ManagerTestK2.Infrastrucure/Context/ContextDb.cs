using ManagerTestK2.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ManagerTestK2.Infrastrucure.Context
{
    public class ContextDb : DbContext
    {
        private readonly IConfiguration _configuration;

        public ContextDb(DbContextOptions<ContextDb> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Phone> Phones { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(_configuration.GetConnectionString("AutenticacaoJWTDB"));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").IsRequired();
                entity.Property(e => e.Name).HasColumnName("Name");
                entity.Property(e => e.Email).HasColumnName("Email");
                entity.Property(e => e.IsAdmin).HasColumnName("IsAdmin");
                entity.Property(e => e.Password).HasColumnName("Password");
                entity.Property(e => e.Salt).HasColumnName("Salt");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.DocumentNumber).IsUnique();
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Email).IsRequired();

                // Relação 1:N com Phone
                entity.HasMany(e => e.Phones)
                      .WithOne()
                      .OnDelete(DeleteBehavior.Cascade);

                // Relação de auto-referência para o gestor
                entity.HasOne(e => e.Manager)
                      .WithMany()
                      .HasForeignKey(e => e.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração da entidade Phone
            modelBuilder.Entity<Phone>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Number).IsRequired();
            });
        }
    }
}
