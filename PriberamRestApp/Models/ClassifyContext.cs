using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PriberamRestApp.Models
{
    public class ClassifyContext : DbContext
    {
        public DbSet<ClassifyItem> Items { get; set; }
        public ClassifyContext(DbContextOptions<ClassifyContext> options)
            : base(options){
        }
    }
}
