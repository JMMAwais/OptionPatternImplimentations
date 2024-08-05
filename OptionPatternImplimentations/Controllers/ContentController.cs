using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OptionPatternImplimentations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        [HttpGet("getWithCookie")]
        [Authorize(Policy = "OnlyCookieScheme")]
        public IActionResult GetWithCookie()
        {
            var userName = HttpContext.User.Claims
                    .Where(x => x.Type == ClaimTypes.Name)
                    .Select(x => x.Value)
                    .FirstOrDefault();
            return Content($"<p>Hello {userName}</p>");
        }



        
        [HttpGet("GetWithJwt2")]
        [Authorize(Policy = "OnlySecondJwtScheme")]
        public IActionResult GetWithSecondJwt()
        {
            var userName = HttpContext.User.Claims
                    .Where(x => x.Type == ClaimTypes.Name)
                    .Select(x => x.Value)
                    .FirstOrDefault();
            return Ok(new { Message = $"Welcome {userName} " });
        }

        [HttpGet("defaultJwt")]
        [Authorize(Policy = "defaultJwt")]
        public IActionResult GetWithDefaultJwt()
        {
            var userName = HttpContext.User.Claims
                    .Where(x => x.Type == ClaimTypes.Name)
                    .Select(x => x.Value)
                    .FirstOrDefault();
            return Ok(new { Message = $"Welcome with Defualt Scheme {userName} " });
        }


        [HttpGet("AllAuthenticationScheme")]
        [Authorize(AuthenticationSchemes = "MultiAuthSchemes")]
        public string GetAllSchemes()
        {
            return "This method can be acess by all Schemes";
        }

    }


}
