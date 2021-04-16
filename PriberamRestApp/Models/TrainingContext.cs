using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PriberamRestApp.Models
{
    public class TrainingContext : DbContext
    {
        public DbSet<TrainingDocument> TrainingDocuments { get; set; }
        public TrainingContext(DbContextOptions<TrainingContext> options)
            : base(options)
        {
        }
    }
}
