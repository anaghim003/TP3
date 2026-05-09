using Microsoft.EntityFrameworkCore;

namespace CinemaManager.Models.Cinema
{
    public class CinemaManagerContext : DbContext
    {
        public CinemaManagerContext(DbContextOptions<CinemaManagerContext> options)
            : base(options) { }

        public DbSet<Movie>    Movies    { get; set; }
        public DbSet<Producer> Producers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasOne(m => m.Producer)
                      .WithMany(p => p.Movies)
                      .HasForeignKey(m => m.ProducerId);
            });
        }
    }
}
