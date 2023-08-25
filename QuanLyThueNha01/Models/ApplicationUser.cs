using Microsoft.AspNetCore.Identity;

namespace QuanLyThueNha01.Models
{
    public class ApplicationUser: IdentityUser 
    {
        // Các thuộc tính tùy chỉnh khác của người dùng
        public string? FullName { get; set; }
        public bool IsVerified { get; set; }

        // Mối quan hệ với các thực thể khác
        public ICollection<UserHomeStay>? UserHomeStays { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public ICollection<Rental>? Rentals { get; set; }
    }
}
