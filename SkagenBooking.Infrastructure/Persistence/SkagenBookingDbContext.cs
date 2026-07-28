using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkagenBooking.Core.Entities;
using SkagenBooking.Infrastructure.Identity;

namespace SkagenBooking.Infrastructure.Persistence;

public sealed class SkagenBookingDbContext : IdentityDbContext<ApplicationUser>
{
    public SkagenBookingDbContext(DbContextOptions<SkagenBookingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<ParkingAllocation> ParkingAllocations => Set<ParkingAllocation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.PropertyId).IsRequired();
            entity.Property(x => x.RoomId).IsRequired();
            entity.Property(x => x.GuestCount).IsRequired();
            entity.Property(x => x.NeedsParking).IsRequired();
            entity.Property(x => x.EstimatedArrivalTime);
            entity.Property(x => x.Status).IsRequired();

            // Optional owner link for Phase 1: null UserId = unowned / legacy booking.
            entity.Property(x => x.UserId)
                .HasMaxLength(450)
                .IsRequired(false);

            entity.HasIndex(x => x.UserId);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            entity.OwnsOne(x => x.DateRange, dateRange =>
            {
                dateRange.Property(x => x.CheckIn).HasColumnName("CheckIn").IsRequired();
                dateRange.Property(x => x.CheckOut).HasColumnName("CheckOut").IsRequired();
            });
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.PropertyId).IsRequired();
            entity.Property(x => x.Type).IsRequired();
            entity.Property(x => x.Capacity).IsRequired();

            entity.ComplexProperty(x => x.NightlyRate, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("NightlyRateAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                money.Property(x => x.Currency)
                    .HasColumnName("NightlyRateCurrency")
                    .HasMaxLength(8)
                    .IsRequired();
            });
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.City).HasMaxLength(200).IsRequired();
            entity.Property(x => x.ParkingCapacity).IsRequired();
        });

        modelBuilder.Entity<ParkingAllocation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.PropertyId).IsRequired();
            entity.Property(x => x.BookingId).IsRequired();

            entity.OwnsOne(x => x.DateRange, dateRange =>
            {
                dateRange.Property(x => x.CheckIn).HasColumnName("CheckIn").IsRequired();
                dateRange.Property(x => x.CheckOut).HasColumnName("CheckOut").IsRequired();
            });
        });
    }
}

