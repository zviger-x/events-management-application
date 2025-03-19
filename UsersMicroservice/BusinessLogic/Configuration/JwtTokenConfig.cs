namespace BusinessLogic.Configuration
{
    #pragma warning disable CS8618
    internal class JwtTokenConfig
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenExpirationMinutes { get; set; }
    }
}
