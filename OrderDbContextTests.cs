using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkIsCheap;
using Xunit;

namespace Tests
{
    public class orderdbcontext_should
        :IClassFixture<DbContextClassFixture>
    {
        OrderContext _context;

        public orderdbcontext_should(DbContextClassFixture fixture)
        {
            _context = fixture.Context;
        }

        [Fact]
        public void SomeTest1()
        {
            _context.Orders.Add(new Order() { Total = 10000 });
            _context.SaveChanges();

            var orders = _context.Orders.ToList();

            Assert.True(orders.Any());
        }


    }


    public class DbContextClassFixture
    {
        public OrderContext Context { get; }

        public DbContextClassFixture()
        {
            var builder = new DbContextOptionsBuilder<OrderContext>();
            builder.UseInMemoryDatabase(options =>
            {
               
            });



            Context = new OrderContext(builder.Options);
        }
    }
}
