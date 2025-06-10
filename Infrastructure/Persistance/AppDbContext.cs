using Auth_API.Domain.Entities.Account;
using Auth_API.Domain.Entities.Warehouses;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth_API.Infrastructure.Persistance
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Warehouses> Warehouses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Chiamata al base per configurare le entità di Identity
            base.OnModelCreating(modelBuilder);

            // Configurazioni personalizzate per ApplicationUser
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName).IsRequired();
                entity.Property(u => u.LastName).IsRequired();

                entity.Property(u => u.AgreeTerm).IsRequired();
                entity.Property(u => u.TwoFactorEnabled).IsRequired();

                entity.Property(u => u.CreatedAt).IsRequired();
                //entity.Property(u => u.PseudonymizedUserId).IsRequired();

                // Converte l'enum MessageCategory in stringa nel DB
                entity.Property(a => a.TwoFactorMethod).IsRequired().HasConversion<string>().HasMaxLength(50);
            });

            // Configurazione 1:N User ↔ Warehouses
            modelBuilder.Entity<Warehouses>(entity =>
            {
                entity.HasKey(w => w.Id);

                entity.Property(w => w.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(w => w.Description)
                      .IsRequired();
                entity.Property(w => w.CreationDate)
                    .IsRequired(); 
                entity.Property(w => w.ModifiedDate)
                      .IsRequired();

                entity.HasOne(w => w.User)
                      .WithMany(u => u.Warehouses)
                      .HasForeignKey(w => w.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
