using System.ComponentModel.DataAnnotations;

namespace QuanLyThueNha01.Models
{
    public class UserUpdateViewModel
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Email { get; set; }
    }
}
