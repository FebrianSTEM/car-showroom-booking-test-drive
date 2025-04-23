using car_booking_service.Application.Services.Interfaces;
using car_booking_service.Domain.Entities;
using car_booking_service.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static car_booking_service.Domain.Constants.ValidationConstants;


namespace car_booking_service.Infrastructure.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserContext _userContext;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                                    ICurrentUserContext userContext)
            : base(options) 
        {
            _userContext = userContext;
        }

        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CarModelConfiguration());
            modelBuilder.ApplyConfiguration(new BookingTestDriveConfiguration());
        }

        public override int SaveChanges()
        {
            // Get all entries in the ChangeTracker that are of type BaseEntity
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.Now;
                    entry.Entity.CreatedBy = _userContext.UserId;
                    entry.Entity.UpdatedAt = entry.Entity.CreatedAt;
                    entry.Entity.UpdatedBy = _userContext.UserId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                    entry.Entity.UpdatedBy = _userContext.UserId;
                }
            }
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.Now;
                    entry.Entity.CreatedBy = _userContext.UserId;
                    entry.Entity.UpdatedAt = entry.Entity.CreatedAt;
                    entry.Entity.UpdatedBy = _userContext.UserId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                    entry.Entity.UpdatedBy = _userContext.UserId;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}