using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueNha01.Models
{
    public class Rental
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RentalId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LongTime { get; set; }
        public string? HomeId { get; set; }
        [ForeignKey(nameof(HomeId))]
        public HomeStay? HomeStay { get; set; }
        public string? UserId { get; set; } // Khóa ngoại tham chiếu đến ApplicationUserId
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
