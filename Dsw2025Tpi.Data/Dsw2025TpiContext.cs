using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data;

public class Dsw2025TpiContext: DbContext
{
    /*Clase para vincular el entityFrameWork*/
    /*Aca se usa la fluent api o el annotation*/
    public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options) : base(options)  
    {

    }
    public DbSet<Product> Products { get; set; } // Asumo que ya la tienes
    public DbSet<Order> Orders { get; set; }     // <--- ¡Probablemente te falta esta!
    public DbSet<OrderItem> OrderItems { get; set; } // 
    public DbSet<Customer> Customers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(eb =>
        {
            eb.ToTable("Products");
            eb.HasKey(p => p.Id);
            eb.Property(p => p.Sku).HasMaxLength(20).IsRequired();      
            eb.Property(p => p.Name).HasMaxLength(60).IsRequired();
            eb.Property(p => p.CurrentUnitPrice).HasPrecision(18, 2);
            eb.Property(p => p.InternalCode).HasMaxLength(30);
            eb.Property(p => p.Description).HasMaxLength(200);

            eb.HasMany(p => p.OrderItems)
              .WithOne(oi => oi.product)
              .HasForeignKey(oi => oi.productId);
             // .OnDelete(DeleteBehavior.Cascade); /*Si se elimina el producto, se eliminan los items de la orden que lo contienen*/
            /*El HasMaxLength se hace solo para strings*/
            /*Hay que configurar las relaciones aca */
        });

        modelBuilder.Entity<Order>(eb =>
        {

            eb.ToTable("Orders");
            eb.HasKey(o => o.Id);
            eb.Property(o => o.shippingAddress).HasMaxLength(60).IsRequired();
            eb.Property(o => o.billingAddress).HasMaxLength(60).IsRequired();
            eb.Property(o => o.notes).HasMaxLength(60);

               eb.HasMany(o => o.orderItems)
              .WithOne(oi => oi.order)
              .HasForeignKey(oi => oi.orderId);
              //.OnDelete(DeleteBehavior.Cascade); /*Si se elimina la orden, se eliminan los items de la orden que la contienen*/

        });

        modelBuilder.Entity<Customer>(eb =>
        {
            eb.ToTable("Customers");
            eb.HasKey(c => c.Id);
            eb.Property(c => c.Id).IsRequired();
            eb.Property(c => c.name).HasMaxLength(70).IsRequired();
            eb.Property(c => c.email).HasMaxLength(50).IsRequired();
            eb.Property(c => c.phoneNumber).HasMaxLength(20).IsRequired();

            eb.HasMany(c => c.orders)
              .WithOne(o => o.customer)
              .HasForeignKey(o => o.customerId);
              //.OnDelete(DeleteBehavior.Restrict); /*Si se elimina el cliente, se eliminan las ordenes que lo contienen*/
        });


       

    }
}
