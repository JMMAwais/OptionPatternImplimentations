using Microsoft.AspNetCore.Identity;

namespace OptionPatternImplimentations.Models
{
    public class AppUser: IdentityUser
    {
        public string Name { get; set; }
    }
}
