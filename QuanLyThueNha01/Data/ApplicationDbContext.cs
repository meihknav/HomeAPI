using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLyThueNha01.Models;
using System.Reflection.Emit;

namespace QuanLyThueNha01.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { 

        }
        public DbSet<ApplicationUser>? Users { get; set; }
        public DbSet<HomeStay> HomeStays { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<MapLocation> MapLocations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserHomeStay> UserHomeStays { get; set; }
        //dinh nghia anh xa vao csdl
        //phuong thuc khoi tao du lieu voi database 
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
            builder.Entity<Payment>()
            .Property(p => p.TotalAmount)
            .HasColumnType("decimal(18, 2)"); // Thay đổi kiểu dữ liệu tùy theo yêu cầu

            builder.Entity<UserHomeStay>()
              .HasKey(kch => new { kch.UserId, kch.HomeId }); // Xác định khóa chính ghép


            //updateRental
            //builder.Entity<Rental>()
            //.HasOne(r => r.LeastUser)
            //.WithMany()
            //.HasForeignKey(r => r.LeastUserId);

            //builder.Entity<Rental>()
            //    .HasOne(r => r.RenterUser)
            //    .WithMany()
            //    .HasForeignKey(r => r.RenterUserId);

            //builder.Entity<UserHomeStay>()
            //.HasKey(uhs => new { uhs.UserId, uhs.HomeId });

            //builder.Entity<UserHomeStay>()
            //    .HasOne(uhs => uhs.ApplicationUser)
            //    .WithMany(au => au.UserHomeStays)
            //    .HasForeignKey(uhs => uhs.UserId);
        }

        //
        private void SeedRoles(ModelBuilder builder )
        {
            builder.Entity<IdentityRole>().HasData
                (
                //tạo role mới trong bảng aspnetrole
                new IdentityRole() { Name = "Admin",ConcurrencyStamp = "1",NormalizedName = "Admin"},
                new IdentityRole() { Name = "User",ConcurrencyStamp = "2",NormalizedName="User"},
                new IdentityRole() { Name = "Guest", ConcurrencyStamp = "3",NormalizedName="Guest"}
                );
        }
    }
}
