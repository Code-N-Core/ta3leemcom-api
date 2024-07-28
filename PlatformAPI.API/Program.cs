using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PlatformAPI.API.MiddleWares;
using PlatformAPI.Core.Helpers;
using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;
using PlatformAPI.Core.Services;
using PlatformAPI.EF;
using PlatformAPI.EF.Data;
using System.Configuration;
using System.Text;

namespace PlatformAPI.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddControllers();
            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddTransient<IMailingService,MailingService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Ta3leem Com",
                    Description = "Platform API",
                    Contact = new OpenApiContact
                    {
                        Name = "Sherif Ibrahim",
                        Email = "sherifebrahim2212@gmail.com",
                    },
                });
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"]
                    };
                });

            builder.Services.AddCors();
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddAutoMapper(typeof(PlatformAPI.Core.Helpers.MappingProfile));
            builder.Services.AddAutoMapper(typeof(PlatformAPI.API.Helpers.MappingProfile));


            // Registering the IActionContextAccessor service with a singleton lifetime.
            // This ensures that the same instance of IActionContextAccessor is used throughout the application's lifetime.
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Registering the IUrlHelper service with a transient lifetime.
            // This means a new instance of IUrlHelper will be created each time it is requested.
            builder.Services.AddTransient<IUrlHelper>(x =>
            {
                // Retrieve the ActionContext from the IActionContextAccessor service.
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;

                // Retrieve the IUrlHelperFactory service.
                var factory = x.GetRequiredService<IUrlHelperFactory>();

                // Use the factory to create a new IUrlHelper instance based on the retrieved ActionContext.
                return factory.GetUrlHelper(actionContext);
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<TeacherIsSubsMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}