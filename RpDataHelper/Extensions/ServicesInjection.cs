using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RpDataHelper.Middlewares;
using RpServices;
using RpServices.Services;
using RpServices.Services.Interfaces;

namespace RpDataHelper.Extensions;

public static class ServicesInjection
{
    public static void InjectDbContext(this IServiceCollection service, string connString)
    {
        service.AddDbContext<DatabaseContext>(
            opt => opt.UseSqlServer(connString));
    }

    public static void InjectIdentity(this IServiceCollection service)
    {
        service
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();
        
        service.Configure<IdentityOptions>(opt =>
        {
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequiredLength = 4;
            opt.User.RequireUniqueEmail = true;
        });
    }

    public static void InjectAuth(this IServiceCollection service, IConfiguration config)
    {
        service
            .AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = config["JWT:ValidIssuer"],
                    ValidAudience = config["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]))
                };
            });
    }

    public static void InjectServices(this IServiceCollection service)
    {
        service
            .AddSingleton<IJwt, JwtServices>()
            .AddTransient<RpExceptionHandlerMiddleware>()
            .AddScoped<ICardManagement, CardManagementServices>();
    }
}