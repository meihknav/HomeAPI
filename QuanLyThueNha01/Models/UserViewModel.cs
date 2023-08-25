using System.ComponentModel.DataAnnotations;

namespace QuanLyThueNha01.Models
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Phải nhập username")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Phải nhập Email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phải nhập pass")]
        public string? Password { get; set; }
    }
}
