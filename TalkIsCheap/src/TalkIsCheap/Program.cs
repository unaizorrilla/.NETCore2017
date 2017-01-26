using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkIsCheap
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var context = new OrderContext())
            {
                var orders = context.Orders
                    .FromSql("SELECT o.Id,o.Total,o.OrderDate,o.UpdatedBy,o.UpdatedOn FROM dbo.Orders o")
                    .Where(o=>o.Total> 1000)
                     .ToList();
            }
        }

        static bool SomeClientEvaluationCode(Decimal total)
        {
            return total > 1000;
        }
    }

    public class Order
    {
        public int Id { get; set; }

        public decimal Total { get; set; }

        public DateTime _myNonConventionalNameField;

        public Order()
        {
            _myNonConventionalNameField = DateTime.UtcNow.AddYears(10);
        }
    }

    public class OrderContext
        :DbContext
    {
        public OrderContext() { }

        public OrderContext(DbContextOptions options) : base(options) { }

        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Initial Catalog=Demos;Integrated Security=true", options =>
                 {
                     //
                 });

                var loggerFactory = new LoggerFactory();
                loggerFactory.AddConsole();

                optionsBuilder.UseLoggerFactory(loggerFactory);

                optionsBuilder.ConfigureWarnings(c =>
                {
                    c.Throw(RelationalEventId.QueryClientEvaluationWarning);
                });
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
       
            modelBuilder.Entity<Order>()
                .ToTable("orders");

            modelBuilder.Entity<Order>()
                .Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo("Sample_HILO");

            modelBuilder.Entity<Order>()
                .Property<string>("UpdatedBy")
                .IsRequired();

            modelBuilder.Entity<Order>()
                .Property<DateTime>("UpdatedOn")
                .IsRequired();

            modelBuilder.Entity<Order>()
                .Property<DateTime>("OrderDate")
                .HasField("_myNonConventionalNameField")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

                
        }

        public override int SaveChanges()
        {
            var orders = this.ChangeTracker.Entries<Order>()
                .Where(o=>o.State == EntityState.Added );

            foreach (var item in orders)
            {
                item.Property<string>("UpdatedBy").CurrentValue = "Unai";
                item.Property<DateTime>("UpdatedOn").CurrentValue = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }

    }
}
