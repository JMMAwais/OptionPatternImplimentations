using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using OptionPatternImplimentations.Data;
using Microsoft.AspNetCore.Identity;
using OptionPatternImplimentations.Models;
using OptionPatternImplimentations.ViewModels;
using System.Security.Principal;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OptionPatternImplimentations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        public AccountController(ApplicationDbContext db,
                       UserManager<AppUser> userManager,
                       RoleManager<IdentityRole> roleManager,
                       SignInManager<AppUser> signInManager,
                       IConfiguration configuration)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;

        }


        //[HttpPost("Login")]
        //public async Task<ResponseModel> Login([FromBody] LoginVM model)
        //{
        //    ResponseModel responseModel = new ResponseModel();
        //    var loggedUser = await _userManager.FindByEmailAsync(model.Email);
        //    var result = _signInManager.CheckPasswordSignInAsync(loggedUser, model.Password, false).Result;
        //    var userRoles = await _userManager.GetRolesAsync(loggedUser);
        //    // var userRole =await _roleManager.Roles.Where(x=>x.Equals(user.Id)).ToListAsync();
        //    if (result.Succeeded)
        //    {
        //        var claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, loggedUser.Name )
        //        };

        //        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //        var principal = new ClaimsPrincipal(claimsIdentity);
        //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal));


        //        string token = IssueToken(loggedUser).Result;
        //        responseModel.Token = token;
        //        responseModel.Status = Ok(token).StatusCode.ToString();
        //        responseModel.Message = "User successfully loggedIn";
        //        //return Ok(token).StatusCode.ToString();

        //    }
        //    return responseModel;

        //}



        //[HttpPost("Register")]
        //public async Task<IActionResult> Register([FromBody] RegisterVM model)
        //{
        //    var user = new AppUser
        //    {
        //        UserName = model.Email,
        //        Email = model.Email,
        //        Name = model.Name
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (result.Succeeded)
        //    {
        //        var RoleExists = await _roleManager.RoleExistsAsync("Admin");
        //        if (RoleExists)
        //        {
        //            await _userManager.AddToRoleAsync(user, "Admin");
        //            return Ok("your are successfully registered");
        //        }
        //    }
        //    return Ok();
        //}


        //[HttpGet("GetString")]
       
        //[Authorize(Policy ="AllowUser")] 
        //public string GetString()
        //{
        //    return "you are successfuly loggedIn";
        //}



      private async Task<string> IssueToken(AppUser user)
      {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("gZisnS8SyuXPlwb1VN/t/2KC8uDimasq1Qqn0MdzSxk="));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
               {
                    new Claim("User_Id", user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role,userRoles.FirstOrDefault().ToString() )
               };
            var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }




    }
}
