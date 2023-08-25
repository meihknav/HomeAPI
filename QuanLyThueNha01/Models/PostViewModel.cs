namespace QuanLyThueNha01.Models
{
    public class PostViewModel
    {
        public string? PostTitle { get; set; }
        public string? PostContent { get; set; }
        public string? UserId { get; set; }
        public PostStatus PostStatus { get; set; } = 0;
    }
}
