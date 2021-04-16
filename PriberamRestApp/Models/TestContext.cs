using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PriberamRestApp.Models
{
    public class TestContext : DbContext
    {
        public DbSet<TestDocument> TestDocuments { get; set; }
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {
        }
    }
}
