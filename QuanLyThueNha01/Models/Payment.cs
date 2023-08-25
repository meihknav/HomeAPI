
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueNha01.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PayId{ get; set; }
        public string? UserId { get; set; } // Thành viên thực hiện thanh toán
        [ForeignKey("UserId")]
        public ApplicationUser? ApplicationUser { get; set; }
        public string? HomeId { get; set; } // Căn hộ thanh toán
        [ForeignKey("HomeId")]
        public HomeStay? HomeStay { get; set; }
        public decimal TotalAmount { get; set; } // Số tiền thanh toán
        public DateTime PaymentDate { get; set; } // Ngày và giờ thanh toán
    }
}
