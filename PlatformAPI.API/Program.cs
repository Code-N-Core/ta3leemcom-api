using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PlatformAPI.API.MiddleWares;
using PlatformAPI.Core.Helpers;
using PlatformAPI.Core.Hubs; // Add this for the NotificationHub
using PlatformAPI.EF.Data;
using System.Text;

namespace PlatformAPI.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var apiBaseUrl = builder.Configuration.GetValue<string>("AppSettings:ApiBaseUrl");

            // Configure HttpClient with the retrieved base URL
            builder.Services.AddHttpClient("QuizApiClient", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            // Configure the password constraints
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Add SignalR for real-time notifications
            builder.Services.AddSignalR();

            // Register services
            builder.Services.AddTransient<HttpClient, HttpClient>();
            builder.Services.AddTransient<QuestionService, QuestionService>();
            builder.Services.AddTransient<ChooseService, ChooseService>();
            builder.Services.AddTransient<QuizService, QuizService>();
            builder.Services.AddTransient<StudentService, StudentService>();
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IDayServices, DayServices>();
            builder.Services.AddTransient<IStudentMonthService, StudentMonthService>();
            builder.Services.AddTransient<IStudentAbsenceService, StudentAbsenceService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<AttachmentService, AttachmentService>();
            builder.Services.AddControllers();
            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
            builder.Services.Configure<AppSetteings>(builder.Configuration.GetSection("AppSetteings"));
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddTransient<IMailingService, MailingService>();

            // Add Quiz Notification Background Service
            builder.Services.AddHostedService<QuizNotificationService>();

            // Swagger/OpenAPI setup
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

            // JWT Authentication
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

            // Register IActionContextAccessor and IUrlHelper
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            builder.Services.AddTransient<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<TeacherIsSubsMiddleware>();

            // Add routing for SignalR
            app.MapHub<NotificationHub>("/notificationHub");

            app.MapControllers();

            app.Run();
        }
    }
}
