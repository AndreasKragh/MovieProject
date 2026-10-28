using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Data;

public class MovieVaultContext : DbContext
{
    public MovieVaultContext(DbContextOptions<MovieVaultContext> options) : base(options)
    {
    }
    public DbSet<Director> Directors { get; set; }
    public DbSet<ProductionCompany> ProductionCompanies { get; set; }
    public DbSet<Studio> Studios { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<ProductionRecord> ProductionRecords { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<MovieCast> MovieCasts { get; set; }
    public DbSet<FilmedAt> FilmedAt { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<Screen> Screens { get; set; }
    public DbSet<Screening> Screenings { get; set; }
    public DbSet<TicketSale> TicketSales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---------- 1:N ----------

        // ProductionCompany 1 -> N Studio
        modelBuilder.Entity<Studio>()
            .HasOne(s => s.ProductionCompany)
            .WithMany(c => c.Studios)
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Director 1 -> N Movie
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.Director)
            .WithMany(d => d.Movies)
            .HasForeignKey(m => m.DirectorId)
            .OnDelete(DeleteBehavior.Restrict);

        // ProductionCompany 1 -> N Movie
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.ProductionCompany)
            .WithMany(c => c.Movies)
            .HasForeignKey(m => m.ProductionCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cinema 1 -> N Screen
        modelBuilder.Entity<Screen>()
            .HasOne(s => s.Cinema)
            .WithMany(c => c.Screens)
            .HasForeignKey(s => s.CinemaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Movie 1 -> N Screening (en film kan eksistere uden screenings)
        modelBuilder.Entity<Screening>()
            .HasOne(s => s.Movie)
            .WithMany(m => m.Screenings)
            .HasForeignKey(s => s.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        // Screen 1 -> N Screening
        modelBuilder.Entity<Screening>()
            .HasOne(s => s.Screen)
            .WithMany(sc => sc.Screenings)
            .HasForeignKey(s => s.ScreenId)
            .OnDelete(DeleteBehavior.Restrict);

        // Screening 1 -> N TicketSale
        modelBuilder.Entity<TicketSale>()
            .HasOne(t => t.Screening)
            .WithMany(s => s.TicketSales)
            .HasForeignKey(t => t.ScreeningId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------- 1:1 ----------

        // Movie 1 -> 0..1 ProductionRecord: FK ligger i ProductionRecord (unik)
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.ProductionRecord)
            .WithOne(p => p.Movie)
            .HasForeignKey<ProductionRecord>(p => p.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- N:M (med ekstra attributter) ----------

        // Movie <-> Actor via MovieCast
        modelBuilder.Entity<MovieCast>()
            .HasKey(mc => new { mc.MovieId, mc.ActorId });

        modelBuilder.Entity<MovieCast>()
            .HasOne(mc => mc.Movie)
            .WithMany(m => m.Cast)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieCast>()
            .HasOne(mc => mc.Actor)
            .WithMany(a => a.MovieCasts)
            .HasForeignKey(mc => mc.ActorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Movie <-> Location via FilmedAt
        modelBuilder.Entity<FilmedAt>()
            .HasKey(f => new { f.MovieId, f.LocationId });

        modelBuilder.Entity<FilmedAt>()
            .HasOne(f => f.Movie)
            .WithMany(m => m.FilmingLocations)
            .HasForeignKey(f => f.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FilmedAt>()
            .HasOne(f => f.Location)
            .WithMany(l => l.FilmedAt)
            .HasForeignKey(f => f.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- Præcision på decimaler ----------
        modelBuilder.Entity<ProductionRecord>()
            .Property(p => p.Budget).HasPrecision(15, 2);
        modelBuilder.Entity<Screening>()
            .Property(s => s.Price).HasPrecision(10, 2);
        modelBuilder.Entity<TicketSale>()
            .Property(t => t.TotalPrice).HasPrecision(10, 2);

        // ---------- Check constraints (matcher jeres SQL + opgavens regler) ----------
        modelBuilder.Entity<Location>().ToTable(t => t.HasCheckConstraint(
            "CK_Location_LocationType",
            "[LocationType] IN ('street', 'building', 'landscape', 'set')"));

        modelBuilder.Entity<Screen>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_Screen_ScreenType", "[ScreenType] IN ('2D', '3D', 'IMAX', '4DX')");
            t.HasCheckConstraint("CK_Screen_Capacity", "[SeatingCapacity] > 0");
        });

        modelBuilder.Entity<Movie>().ToTable(t => t.HasCheckConstraint(
            "CK_Movie_MovieStatus",
            "[MovieStatus] IN ('in production', 'released', 'archived', 'planned')"));

        modelBuilder.Entity<MovieCast>().ToTable(t => t.HasCheckConstraint(
            "CK_MovieCast_RoleType",
            "[RoleType] IN ('lead', 'supporting', 'cameo')"));

        modelBuilder.Entity<Screening>().ToTable(t => t.HasCheckConstraint(
            "CK_Screening_Price", "[Price] >= 0"));

        modelBuilder.Entity<TicketSale>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_TicketSale_Quantity", "[Quantity] > 0");
            t.HasCheckConstraint("CK_TicketSale_TotalPrice", "[TotalPrice] >= 0");
        });
    }
}