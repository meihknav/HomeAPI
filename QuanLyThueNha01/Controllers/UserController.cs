using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueNha01.Data;
using QuanLyThueNha01.Models;
using System.Linq.Expressions;

namespace QuanLyThueNha01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //Depedency Injection
        private ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        //QUẢN LÝ KHÁCH HÀNG 
        //GUEST = "PUBLIC 
        //USER = authorize role User thực hiện đc các chức năng, còn Guest ko có quyền, chỉ xài đc những chức năng ko có role là User

        //USER + GUEST
        //Xem danh sach cac can ho dang cho thue ( isAvailable == true): có sẵn thì đang cho thuê ==> ai đó thuê homeId đó thì IsAvailable = false
        //Mot user co the cho thue nhieu can ho 
        //public
        [HttpGet("getHomeStays")]
        public async Task<ActionResult> GetHomeStays()
        {
            var homeIsRentals = await _context.HomeStays.Where(h => h.IsAvailable == true).ToListAsync();
            if (homeIsRentals == null)
            {
                return BadRequest();
            }
            return Ok(homeIsRentals);
        }
        //Xem chi tiet can ho
        //public
        [HttpGet("Detail/{homeId}")]
        public async Task<ActionResult> GetHomeStayDetails(string homeId)
        {
            var homeDetail = await _context.HomeStays.Where(h => h.HomeId == homeId).ToListAsync();
            if (homeDetail == null)
            {
                return BadRequest();
            }
            return Ok(homeDetail);
        }
        //Tim Kiem Can Ho cho Thue theo gia 200 , 250 -- tim kiem theo khoang gia (nhap vao gia 1, gia 2) xuat ra gia trong khoang
        //public
        [HttpGet("SearchByPrice")]
        public async Task<ActionResult> SearchHomeByPrice(int price1, int price2)
        {
            var homeListPrices = await _context.HomeStays.Where(home => home.Price >= price1 && home.Price <= price2).ToListAsync();
            if (homeListPrices == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new Response { Status = "Error", Message = "Khong tim thay can ho" });
            }
            return Ok(homeListPrices);
        }
        //Sap xep can ho theo gia tang dan 
        //public
        [HttpGet("SortIncreaseByPrice")]
        public async Task<ActionResult> SortIncrease()
        {
            var sortHomeLists = await _context.HomeStays.OrderBy(home => home.Price).ToListAsync();
            return Ok(sortHomeLists);
        }
        //Sap xep can ho theo gia giam dan 
        //public
        [HttpGet("SortDescendingByPrice")]
        public async Task<ActionResult> SortDesceding()
        {
            var sortHomeLists = await _context.HomeStays.OrderByDescending(home => home.Price).ToListAsync();
            return Ok(sortHomeLists);
        }
        //Tim Kiem Can Ho theo dia chi cho thue : vd : Quan12, BinhThanh,Quan3,TanBinh (nhập quận, hoặc nhập quận + thành phố đều ra)
        //public
        [HttpGet("SearchHomeByAddress")]
        public async Task<ActionResult> SearchByAddress(string District)
        {
            var homeByAddress = await _context.HomeStays
                .Where(home =>
                    (home.Address!.IndexOf(",") != -1) ?
                        home.Address.Substring(0, home.Address.IndexOf(",")) == District || home.Address.Substring(0) == District :
                        false
                )
                .ToListAsync();

            if (homeByAddress.Count == 0)
            {
                return NotFound(new Response { Status = "Error", Message = "Không tìm thấy căn hộ" });
            }
            else
            {
                return Ok(homeByAddress);
            }
        }

        //TimDuong=> Location hien kinh do va vi do : FAKE CHI DUONG
        //public
        [HttpGet("MapToHome/{homeId}")]
        public async Task<ActionResult<MapLocation>> SearchMapToHome(string homeId)
        {
            var maptoHome = await _context.HomeStays
                           .Where(home => home.HomeId == homeId)
                           .Join(_context.MapLocations,
                            home => home.LocationId,
                            location => location.LocationId,
                            (home, location) => new
                            {
                                HomeId = home.HomeId,
                                HomeName = home.HomeName,
                                Latitude = location.Latitude,
                                Longitude = location.Longitude,
                                LastTimeMap = location.LastUpdateTime
                            }
                            ).ToListAsync();
            return Ok(maptoHome);
        }

        //Cac chuc nang cua Khach hang Thanh Vien == Khach vang lai se ko thuc hien duoc nhung chuc nang nay 
        //POST 
        //Khach hang dang bai Post moi cho thue can ho 
        //USER 
        [Authorize(Roles = "User")]
        [HttpPost("NewPostToRental")]
        public async Task<ActionResult> AddNewPost(PostViewModel newpost)
        {
            var post = new Post
            {
                PostTitle = newpost.PostTitle,
                PostContent = newpost.PostContent,
                UserId = newpost.UserId,
                Status = PostStatus.Pending,
            };
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created,
            new Response { Status = "Success", Message = "Ban da dang bai Post thanh cong" });
        }
        //Xem danh sach cac can ho ma minh dang cho thue
        //trong bang UserHomeStay
        //USER
        [Authorize(Roles = "User")]
        [HttpGet("MyHomeStayForRental/{userId}")]
        public async Task<ActionResult> MyHomeStay(string userId)
        {
            var homeStayLists = await _context.UserHomeStays
                                .Where(home => home.UserId == userId)
                                .Join(_context.HomeStays,
                                h => h.HomeId,
                                s => s.HomeId,
                                (h, s) => new
                                {
                                    HomeId = s.HomeId,
                                    HomeName = s.HomeName,
                                    Price = s.Price,
                                    Address = s.Address
                                }).ToListAsync();
            return Ok(homeStayLists);
        }
        //Đăng ký thuê căn Hộ 
        //USER
        [Authorize(Roles = "User")]
        [HttpPost("AcceptRental")]
        public async Task<ActionResult> AcceptRental(RentalModel rentalModel,string userid,string homeId) {
            var homeRental = await _context.HomeStays.FirstOrDefaultAsync(h => h.HomeId == homeId);
            if(homeRental!.IsAvailable == false)
            {
                return Ok(new Response { Status = "Error", Message = "Can ho nay da duoc thue" });
            }else
            {
                var newRental = new Rental
                {
                    UserId = userid,
                    HomeId = homeId,
                    StartDate = rentalModel.StartDate!.Value,
                    EndDate = rentalModel.EndDate!.Value,
                    LongTime = rentalModel.EndDate.Value.Day - rentalModel.StartDate.Value.Day + 1,
                };
                await _context.Rentals.AddAsync(newRental);
                await _context.SaveChangesAsync();
                return Ok(newRental);
            }
        }
        //Thanh toán onl => khi truy cập api này sẽ tạo ra hóa đơn payment tương ứng với userId,homeId,số tiền,date thì lấy date.Now(ngày hiện tại )
        //Thanh toán dứt điểm ( thuê xong là trả tiền)
        //Thanh toán dồn (cộng các Price lại )
        //[HttpPost("PaymentAllHomeOnline")]
        //public async Task<ActionResult> PaymentAll(string userId)
        //{
        //    var homeNeedPays = await _context.Rentals
        //                      .Where(home => home.UserId == userId)
        //                      .Join(_context.HomeStays,
        //                      r => r.HomeId,
        //                      h => h.HomeId,
        //                      (r, h) => new
        //                      {
        //                          HomeId = r.HomeId,
        //                          Price = h.Price,
        //                      }).ToListAsync();
        //    double TotalAmount = 0;
        //    foreach (var home in homeNeedPays)
        //    {
        //        TotalAmount += home.Price;
        //    }
        //    var payment = new Payment
        //    {
        //        UserId = userId,
        //        TotalAmount = (decimal)TotalAmount,
        //        PaymentDate = DateTime.Now,
        //    };
        //    await _context.Payments.AddAsync(payment);
        //    await _context.SaveChangesAsync();

        //    return Ok(new Response { Status = "Success", Message = $"Ban da thanh toan thanh cong voi tong so tien la {TotalAmount}" });
        //}

        //Thanh toán từng căn 
        //USER
        [Authorize(Roles = "User")]
        [HttpPost("PaymentHomeOnline")]
        public async Task<ActionResult> PaymentAHome()
        {
            //vừa đăng ký xong thì thanh toán 
            var lastRental = await _context.Rentals.OrderByDescending(r => r.RentalId).FirstOrDefaultAsync();
            var homeStayPrice = await _context.HomeStays
            .Where(h => h.HomeId == lastRental!.HomeId)
            .Select(h => h.Price)
            .FirstOrDefaultAsync();
            var TotalAmount = (decimal)homeStayPrice;
            var payment = new Payment
            {
                UserId = lastRental!.UserId,
                HomeId = lastRental!.HomeId,
                TotalAmount = TotalAmount,
                PaymentDate = DateTime.Now,
            };
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return Ok(new Response { Status = "Success", Message = $"Ban da thanh toan thanh cong voi tong so tien la {TotalAmount}" });
        }
    }
}

