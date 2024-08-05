namespace OptionPatternImplimentations.Configuration
{
    public class Jwt
    {
        public const string Key = "Jwt";
        public string jwtKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
