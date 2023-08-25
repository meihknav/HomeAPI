using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueNha01.Models
{
    public class HomeStay
    {
        [Key]
        public string? HomeId { get; set; }
        [Required]
        [StringLength(50)]
        public string? HomeName { get; set; }
        [Required]
        [StringLength(150)]
        public string? Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        [StringLength (50)]
        public string? Address { get; set; }
        public int LocationId { get; set; }
        [ForeignKey(nameof(LocationId))]
        public MapLocation? MapLocation { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<UserHomeStay>? UserHomeStays { get; set; }
        //public string? UserId { get; set; }
        //[ForeignKey("UserId")]
        //public ApplicationUser? ApplicationUser { get; set; }
    }
}
