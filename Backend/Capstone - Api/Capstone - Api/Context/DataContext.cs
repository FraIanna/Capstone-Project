using Capstone___Api.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Capstone___Api.Context
{
    public class DataContext : DbContext
    {
        public virtual DbSet<Movie> Movies { get; set; }

        public virtual DbSet<Hall> Halls { get; set; }

        public virtual DbSet<HallSeat> HallSeats { get; set; }

        public virtual DbSet<MovieShowtime> MovieShowtimes { get; set; }

        public virtual DbSet<Reservation> Reservations { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public DataContext(DbContextOptions<DataContext> opt) : base(opt) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
               .HasMany(m => m.MovieShowtimes)
               .WithOne(ms => ms.Movie)
               .HasForeignKey(ms => ms.MovieId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Movie>()
               .HasOne(m => m.Hall)
               .WithMany(h => h.Movies)
               .HasForeignKey(m => m.HallId)
               .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Movie>()
               .HasMany(m => m.HallSeats)
               .WithOne()
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hall>()
                .HasMany(h => h.Movies)
                .WithOne(m => m.Hall)
                .HasForeignKey(m => m.HallId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hall>()
                .HasMany(h => h.Seats)
                .WithOne(s => s.Hall)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HallSeat>()
                .HasOne(h => h.Hall)
                .WithMany (h => h.Seats)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Hall)
                .WithMany(h => h.Reservations)
                .HasForeignKey(r => r.HallId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reservation>()
               .HasOne(r => r.Movie)
               .WithMany(m => m.Reservations)
               .HasForeignKey(r => r.MovieId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reservation>()
               .HasOne(r => r.Seat)
               .WithMany(hs => hs.Reservations)
               .HasForeignKey(r => r.HallSeatId)
               .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Reservation>()
               .HasOne(r => r.User)
               .WithMany()
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
