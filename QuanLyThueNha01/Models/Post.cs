using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueNha01.Models
{
    public enum PostStatus
    {
        Pending,//Đang chờ duyệt,=>>> 1
        Approved, //Đã xác nhận,=>>> 2
        Rejected //Đã từ chối ,=>>>> 3
    }
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }
        [Required]
        [StringLength(50)]
        public string? PostTitle { get; set; }
        [Required]
        [StringLength(100)]
        public string? PostContent { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? ApplicationUser { get; set; }
        public PostStatus Status { get; set; }

    }
}
