using dsBot.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace dsBot.DataContext;

public class ApplicationDbContext : DbContext
{
    public DbSet<PlayList> PlayLists { get; set; }
    public DbSet<Tracks> Tracks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayList>()
            .HasMany(p => p.Tracks)
            .WithOne(t => t.PlayList)
            .HasForeignKey(t => t.PlayListId);
    }
}
