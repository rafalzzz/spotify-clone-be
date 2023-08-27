using SpotifyAPI.Variables;

namespace SpotifyAPI.Configuration
{
    public class CorsConfiguration
    {
        public CorsConfiguration(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(Cors.CorsPolicy, builder =>
                    builder
                        .WithOrigins(Environment.GetEnvironmentVariable(EnvironmentVariables.FrontendDomain))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }
    }
}