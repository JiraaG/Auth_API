using Auth_API.Domain.Entities.Account;
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
                entity.Property(u => u.PseudonymizedUserId).IsRequired();

                // Converte l'enum MessageCategory in stringa nel DB
                entity.Property(a => a.TwoFactorMethod).IsRequired().HasConversion<string>().HasMaxLength(50);
            });
        }
    }
}
