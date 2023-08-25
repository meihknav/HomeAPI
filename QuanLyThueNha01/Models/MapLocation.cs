using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueNha01.Models
{
    public class MapLocation
    {
        [Key]
        public int LocationId { get; set; }
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;

        public DateTime LastUpdateTime { get; set; }
    }
}
