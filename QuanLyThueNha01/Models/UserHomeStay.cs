using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueNha01.Models
{
    public class UserHomeStay
    {
        [Key]
        public string? UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public string? HomeId { get; set; }
        public HomeStay? HomeStay { get; set; }
    }
}
