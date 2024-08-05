using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OptionPatternImplimentations.Data;
using OptionPatternImplimentations.Models;
using OptionPatternImplimentations.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Authorization;
using OptionPatternImplimentations.Configuration;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OptionPatternImplimentations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly Jwt _jwt;
        private readonly IOptionsSnapshot<Jwt> _appSettingsSnapshot;
        private readonly IOptionsMonitor<Jwt> _appSettingsMonitor;
        public AuthController(ApplicationDbContext db,
                       UserManager<AppUser> userManager,
                       RoleManager<IdentityRole> roleManager,
                       SignInManager<AppUser> signInManager,
                       IConfiguration configuration,
                       IOptions<Jwt> jwtOption, IOptionsSnapshot<Jwt> appSettingsSnapshot,
                       IOptionsMonitor<Jwt> appSettingsMonitor)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwt = jwtOption.Value;
            _appSettingsMonitor = appSettingsMonitor;
            _appSettingsSnapshot = appSettingsSnapshot;

        }



        [HttpPost("loginWithJwt")]
        public async Task<ResponseModel> LoginWithJwt([FromBody] LoginVM model)
        {

            ResponseModel responseModel = new ResponseModel();

            var loggedUser = await _userManager.FindByEmailAsync(model.Email);
            var result = _signInManager.CheckPasswordSignInAsync(loggedUser, model.Password, false).Result;
            var userRoles = await _userManager.GetRolesAsync(loggedUser);
            if (result.Succeeded)
            {
                 
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.jwtKey));
                var signinCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var tokenOptions = new JwtSecurityToken(
                      issuer: _jwt.Issuer,
                    audience: _jwt.Audience,
                    claims: new List<Claim>() {
                     new Claim(JwtRegisteredClaimNames.Sub, loggedUser.Id),
                     new Claim(ClaimTypes.Name, loggedUser.Name ?? string.Empty),
                     new Claim(ClaimTypes.Role,userRoles.FirstOrDefault().ToString() )},
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                //return Ok(new { Token = tokenString });
                responseModel.Token = tokenString;
                return responseModel;
            }
            else
            {
                responseModel.Message = "Your credentials are Invalild. Try Again";
                return responseModel;
            }

        }

        [HttpPost("loginWithJwt2")]
        public async Task<ResponseModel> LoginWithJwt2([FromBody] LoginVM model)
        {
            ResponseModel responseModel = new ResponseModel();
            var loggedUser = await _userManager.FindByEmailAsync(model.Email);
            var result = _signInManager.CheckPasswordSignInAsync(loggedUser, model.Password, false).Result;
            var userRoles = await _userManager.GetRolesAsync(loggedUser);
            if (result.Succeeded)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bC9Shjao0rFLI9xZ+TX/WenMBCkY/N+v23uOA445Z4I="));
                var signinCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var tokenOptions = new JwtSecurityToken(
                    issuer: _appSettingsSnapshot.Value.Issuer,
                    audience: _appSettingsSnapshot.Value.Audience,
                  claims: new List<Claim>()
                  { new Claim(ClaimTypes.Name, loggedUser.Name ?? string.Empty),
                     new Claim(ClaimTypes.Role,userRoles.FirstOrDefault().ToString())
                  },
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                //return Ok(new { Token = tokenString });
                responseModel.Token = tokenString;
                return responseModel;
            }
            else
            {
                responseModel.Message = "Your credentials are Invalild. Try Again";
                return responseModel;
            }

          
        }


        [HttpPost("loginWithCookie")]
        public async Task<ResponseModel> LoginWithCookie(LoginVM model)
        {
            ResponseModel responseModel = new ResponseModel();
            var loggedUser = await _userManager.FindByEmailAsync(model.Email);
            var result = _signInManager.CheckPasswordSignInAsync(loggedUser, model.Password, false).Result;
            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loggedUser.Name )
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal));
                responseModel.Message = "you are logged In via cookie method";
            }
            return responseModel;
        }


        [HttpPost("loginReturnCookieandtoken")]
        public async Task<ResponseModel> loginReturnCookieandtoken([FromBody] LoginVM model)
        {

            ResponseModel responseModel = new ResponseModel();

            var loggedUser = await _userManager.FindByEmailAsync(model.Email);
            var result = _signInManager.CheckPasswordSignInAsync(loggedUser, model.Password, false).Result;
            var userRoles = await _userManager.GetRolesAsync(loggedUser);
            if (result.Succeeded)
            {

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettingsMonitor.CurrentValue.jwtKey));
                var signinCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var tokenOptions = new JwtSecurityToken(
                      issuer: _appSettingsMonitor.CurrentValue.Issuer,
                    audience: _appSettingsMonitor.CurrentValue.Audience,
                    claims: new List<Claim>() {
                     new Claim(JwtRegisteredClaimNames.Sub, loggedUser.Id),
                     new Claim(ClaimTypes.Name, loggedUser.Name ?? string.Empty),
                     new Claim(ClaimTypes.Role,userRoles.FirstOrDefault().ToString() )},
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                var claimsIdentity = new ClaimsIdentity(tokenOptions.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal));
                //return Ok(new { Token = tokenString });
                responseModel.Token = tokenString;
                return responseModel;
            }
            else
            {
                responseModel.Message = "Your credentials are Invalild. Try Again";
                return responseModel;
            }

        }

        [HttpPost("Register")]
        [Authorize(Policy = "OnlyAdmin", AuthenticationSchemes = "Bearer")]
        public async Task<ResponseModel> Register([FromBody] RegisterVM model)
        {
            ResponseModel responseModel = new ResponseModel();
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var RoleExists = await _roleManager.RoleExistsAsync("Admin");
                if (RoleExists)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    responseModel.Message = "Your are successfully Registered";
                }
            }
            else
            {
                responseModel.Message = "Email or password is Invalid";
            }
            return responseModel;
        }


    }
}
