using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuanLyThueNha01.Models;
using QuanLyThueNha01.Models.Authenticate.Login;
using QuanLyThueNha01.Models.Authenticate.SignUp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuanLyThueNha01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        //Đăng ký
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {
            //kiem tra user da ton tai trong db hay chua 
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                  new Response { Status = "Error", Message = "Nguoi dung da ton tai !" }
                );
            }

            //tao 1 user moi 
            IdentityUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.UserName,
            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "Dang ky that bai" });
                }

                //Assign a role : gan một Quyen cho user đó
                await _userManager.AddToRoleAsync(user, role);
                ////gan claims cho user nay ( claims là thong tin cua user khi authenticate)
                //var claims = new List<Claim>
                //{
                //    new Claim("FullName",user.UserName!),
                //    new Claim("Email",user.Email!),

                //};
                //await _userManager.AddClaimsAsync(user, claims);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = " Success", Message = "User Created Successfully!" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "This Role Doesn't Exist." });
            }
        }

        //Đăng nhập Login ==> JWT TOKEN
        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        {
            //checking user...
            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if(user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var authClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                var jwtToken = GetToken(authClaims);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                });
            }
            return Unauthorized();
        }

        //hàm tạo token 
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey,SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
