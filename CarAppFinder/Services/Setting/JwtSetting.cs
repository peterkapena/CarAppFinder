namespace CarAppFinder.Services.Setting
{
    public class JwtSetting
    {
        public string Issuer { get; set; }
        public string IssuerSigningKey { get; set; }
        public string Audience { get; set; }
        public int TokenLifeTime { get; set; }
    }
}
