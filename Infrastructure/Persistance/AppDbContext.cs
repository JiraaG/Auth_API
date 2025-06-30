using Auth_API.Domain.Entities.Account;
using Auth_API.Domain.Entities.Warehouses;
using Auth_API.Domain.Entities.Warehouses.Products;
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
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }

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

            // Configurazione di Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Code)
                        .HasMaxLength(50);

                entity.Property(p => p.Name)
                        .IsRequired()
                        .HasMaxLength(200);
                entity.Property(p => p.Description)
                        .IsRequired();

                entity.Property(p => p.Quantity)
                        .IsRequired()
                        .HasColumnType("decimal(18,4)");
                entity.Property(p => p.QuantityUnit)
                        .IsRequired()
                        .HasMaxLength(10);

                entity.Property(p => p.PurchasePrice)
                        .IsRequired()
                        .HasColumnType("decimal(18,2)");
                //entity.Property(p => p.SalePrice)
                //        .IsRequired()
                //        .HasColumnType("decimal(18,2)");

                entity.Property(p => p.Currency)
                      .IsRequired()
                      .HasMaxLength(3);

                entity.Property(p => p.PurchaseDate)
                      .IsRequired();
                //entity.Property(p => p.SaleDate)
                //      .IsRequired();

                entity.Property(p => p.IsSold)
                      .IsRequired();

                entity.Property(p => p.SaleOrdersPrice)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(p => p.CreatedAt)
                      .IsRequired();
                entity.Property(p => p.ModifiedAt)
                      .IsRequired();

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Warehouse)
                      .WithMany(w => w.Products)
                      .HasForeignKey(p => p.WarehouseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurazione di Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                        .IsRequired()
                        .HasMaxLength(100);
                entity.Property(c => c.Description)
                        .IsRequired();

                entity.Property(c => c.CreatedAt)
                      .IsRequired();
                entity.Property(c => c.ModifiedAt)
                      .IsRequired();

                entity.HasOne(c => c.ParentCategory)
                      .WithMany(c => c.SubCategories)
                      .HasForeignKey(c => c.ParentCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.Product)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(o => o.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(o => o.Quantity)
                      .HasColumnType("decimal(18,4)");

                entity.Property(o => o.UnitPrice)
                      .HasColumnType("decimal(18,2)");

                entity.Property(o => o.TotalPrice)
                      .HasColumnType("decimal(18,2)");
            });
        }
    }
}
