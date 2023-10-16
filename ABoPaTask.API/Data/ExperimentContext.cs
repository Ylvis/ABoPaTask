using ABoPaTask.API.Classes;
using Microsoft.EntityFrameworkCore;
using ABoPaTask.API.Data.Configuration;

namespace ABoPaTask.API.Data
{
    public class ExperimentContext : DbContext
    {

        public ExperimentContext(DbContextOptions<ExperimentContext> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Result> Results { get; set; }
        public DbSet<Experiment> Experiments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ExperimentConfigur());
        }
    }
}
