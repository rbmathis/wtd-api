using Microsoft.EntityFrameworkCore;
using WillTheyDie.Api.Models;

namespace WillTheyDie.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Show> Shows => Set<Show>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Episode> Episodes => Set<Episode>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<UserShow> UserShows => Set<UserShow>();
    public DbSet<Bet> Bets => Set<Bet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        // Show configuration
        modelBuilder.Entity<Show>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CurrencyName).HasMaxLength(50);
            entity.Property(e => e.CurrencySymbol).HasMaxLength(10);
        });

        // Season configuration
        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasOne(e => e.Show)
                .WithMany(s => s.Seasons)
                .HasForeignKey(e => e.ShowId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
        });

        // Episode configuration
        modelBuilder.Entity<Episode>(entity =>
        {
            entity.HasOne(e => e.Season)
                .WithMany(s => s.Episodes)
                .HasForeignKey(e => e.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
        });

        // Character configuration
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasOne(e => e.Show)
                .WithMany(s => s.Characters)
                .HasForeignKey(e => e.ShowId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => new { e.ShowId, e.Status });
            
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Actor).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        // UserShow configuration
        modelBuilder.Entity<UserShow>(entity =>
        {
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserShows)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Show)
                .WithMany(s => s.UserShows)
                .HasForeignKey(e => e.ShowId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => new { e.UserId, e.ShowId }).IsUnique();
            
            entity.Property(e => e.CurrencyBalance).HasPrecision(18, 2);
        });

        // Bet configuration
        modelBuilder.Entity<Bet>(entity =>
        {
            entity.HasOne(e => e.User)
                .WithMany(u => u.Bets)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Character)
                .WithMany(c => c.Bets)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Episode)
                .WithMany(ep => ep.Bets)
                .HasForeignKey(e => e.EpisodeId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasIndex(e => new { e.UserId, e.EpisodeId });
            entity.HasIndex(e => new { e.EpisodeId, e.Status });
            
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.Prediction).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
        });
    }
}
