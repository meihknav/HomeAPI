using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueNha01.Data;
using QuanLyThueNha01.Models;
using System.Data;

namespace QuanLyThueNha01.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //ADMIN QUẢN LÝ BÀI POST 

        //Xem bai post
        [HttpGet("ViewPosts")]
        public ActionResult ViewAllPosts()
        {
            var posts = _context.Posts.ToList();
            return Ok(posts);
        }
        //Chức năng xem bài Post:
        //Xem các bài Post đang chờ duyệt(Pending = 0)
        [HttpGet("ViewPostsIsPending")]
        public async Task<ActionResult<IEnumerable<Post>>> ViewPostsIsPending()
        {
            var pendingPosts = await _context.Posts
                .Where(post => post.Status == PostStatus.Pending)
                .ToListAsync();

            return Ok(pendingPosts);
        }
        //Xem các bài Post đã duyệt 
        [HttpGet("ViewPostsIsApproved")]
        public async Task<ActionResult<IEnumerable<Post>>> ViewPostsIsApproved()
        {
            var approvedPosts = await _context.Posts
                .Where(post => post.Status == PostStatus.Approved)
                .ToListAsync();

            return Ok(approvedPosts);
        }
        //Xem các bài Post đã xóa 
        [HttpGet("ViewPostsIsRejected")]
        public async Task<ActionResult<IEnumerable<Post>>> ViewPostsIsRejected()
        {
            var rejectedPosts = await _context.Posts
                .Where(post => post.Status == PostStatus.Rejected)
                .ToListAsync();

            return Ok(rejectedPosts);
        }


        //Chức năng chỉnh trạng thái bài Post của Admin
        //Duyệt
        [HttpPut("ApprovedPost")]
        public async Task<ActionResult> ApprovedPost(int postId)
        {
            //lấy ra bài post cần duyệt theo Id 
            var postNeedApproved = await _context.Posts.FirstOrDefaultAsync(post => post.PostId == postId);
            if (postNeedApproved != null)
            {
                //thực hiện duyệt bằng cách chỉnh sửa Status của Post
                postNeedApproved.Status = PostStatus.Approved;
                await _context.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "Bạn đã duyệt bài viết thành công" });
            }
            else { return NotFound(); }
        }
        //Từ chối xóa bỏ post
        [HttpPut("RejectedPost")]
        public async Task<IActionResult> RejectedPost(int postId)
        {
            //lấy ra bài post cần duyệt theo Id 
            var postNeedRejected = await _context.Posts.FirstOrDefaultAsync(post => post.PostId == postId);
            if (postNeedRejected != null)
            {
                //thực hiện duyệt bằng cách chỉnh sửa Status của Post
                postNeedRejected.Status = PostStatus.Rejected;
                await _context.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "Bạn đã hủy bài viết thành công" });
            }
            else { return NotFound(); }
        }
        //Xóa một bài viết theo PostId mà Admin muốn xóa

        [HttpDelete("DeletePostById")]
        public async Task<IActionResult> DeleteAPost(int postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
            if (post != null)
            {
                _context.Remove(post);
                await _context.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "Bạn đã xóa bài viết thành công" });
            }
            else
            {
                return NotFound();
            }
        }

        //Xóa các bài viết có trạng thái hủy bỏ
        [HttpDelete("DeletePosts")]
        public async Task<IActionResult> DeletePosts()
        {
            var deletePosts = await _context.Posts.Where(post => post.Status == PostStatus.Rejected).ToListAsync();
            if (deletePosts == null || !deletePosts.Any())
            {
                return NotFound();
            }
            else
            {
                _context.RemoveRange(deletePosts);
                await _context.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "Bạn đã xóa bài viết thành công" });
            }
        }

        //Mã Admin : 2d7175b5-d98f-41cf-9a35-0970a73ec618
        //Mã Guest(Khách vãng lai) : 45318d62-c38c-4a17-baf6-4d3ee28aa4e6
        //Mã User(Khách hệ thống ) : edf16ebd-3156-4088-9ace-c1805a481899

        //QUẢN LÝ USER TRONG HỆ THỐNG --- QUẢN LÝ USER CÓ ROLE LÀ USER
        //CRUD - 

        //Create User ( User đăng ký hoặc do admin tự tạo ra 1 User :)) )
        [HttpPost("CreateNewUser")]
        public async Task<IActionResult> AddUser([FromBody] UserViewModel model, string role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {

                var newUser = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                    // Thêm các thông tin người dùng khác tùy ý
                };
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, role);
                    //return CreatedAtAction("AddUser", new { id = newUser.Id }, newUser);
                    return Ok(new Response { Status = "Success", Message = "Bạn đã thêm người dùng thành công" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                //return Ok(new Response { Status = "Success", Message = "Bạn đã thêm người dùng thành công"});
                return BadRequest(ModelState);
            }
        }

        //ReadUser ( GetUser From Database ) --- đọc ra các user có role là User
        [HttpGet("GetWithRoleUser")]
        public async Task<IActionResult> GetUsersWithRole()
        {
            var usersWithRole = await _userManager.GetUsersInRoleAsync("User");

            return Ok(usersWithRole);
        }

        //UpdateUser ( HttpPut sửa thông tin User )
        [HttpPut("updateuser/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userToUpdate = await _userManager.FindByIdAsync(id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(model.UserName))
            {
                userToUpdate.UserName = model.UserName;
            }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                userToUpdate.Email = model.Email;
            }

            // Cập nhật các thông tin người dùng khác tùy ý

            var result = await _userManager.UpdateAsync(userToUpdate);

            if (result.Succeeded)
            {
                return Ok(new Response { Status = "Success", Message = "Bạn đã sửa thông tin người dùng thành công" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        //DeleteUser (HttpDelete xóa 1 user trong Db )
        [HttpDelete("deleteuser/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var userToDelete = await _userManager.FindByIdAsync(userId);

            if (userToDelete == null)
            {
                return NotFound();
            }

            //Tạo một bảng liên quan tới HomeId---UserId ( nhà thuê và người thuê ) sau đó xóa dữ liệu trong bảng đó 

            //// Xóa các bản ghi liên quan trong bảng Posts
            var userPosts = await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
            _context.Posts.RemoveRange(userPosts);
            await _context.SaveChangesAsync();

            //// Xóa các bản ghi liên quan trong bảng UserHomeStay


            var userHomeStays = await _context.UserHomeStays.Where(p => p.UserId == userId).ToListAsync();
            _context.UserHomeStays.RemoveRange(userHomeStays);
            await _context.SaveChangesAsync();

            //// Xóa các bản ghi liên quan trong bảng Payments
            var userPayments = await _context.Payments.Where(p => p.UserId == userId).ToListAsync();
            _context.Payments.RemoveRange(userPayments);

            //// Xóa các bản ghi liên quan trong bảng Rentals
            var userRentals = await _context.Rentals.Where(r => r.UserId == userId).ToListAsync();
            _context.Rentals.RemoveRange(userRentals);
            await _context.SaveChangesAsync();



            //// Lưu thay đổi trước khi xóa người dùng
            await _context.SaveChangesAsync();

            // Xóa người dùng
            var result = await _userManager.DeleteAsync(userToDelete);

            if (result.Succeeded)
            {
                return Ok(new Response { Status = "Success", Message = "Bạn đã xóa người dùng thành công" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        //QUẢN LÝ DOANH THU


    }
}
