namespace EventsAPI.Configuration
{
    public class JwtTokenConfig
    {
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int TokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationMinutes { get; set; }
    }
}
