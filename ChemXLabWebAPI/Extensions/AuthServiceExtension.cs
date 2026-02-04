using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace ChemXLabWebAPI.Extensions
{
    public static class AuthServiceExtension
    {
        public static void AuthenticationServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                };
            });
        }

        public static void SwaggerServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PRN232 API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Your token :"
                });

                // SỬA: Bọc toàn bộ trong lambda (document => ...)
                c.AddSecurityRequirement(document =>
                {
                    return new OpenApiSecurityRequirement
                    {
                        {
                            // Truyền "Bearer" VÀ biến "document" vào Constructor
                            new OpenApiSecuritySchemeReference("Bearer", document),
                            new List<string>()
                        }
                    };
                });
            });
        }
    }
}
