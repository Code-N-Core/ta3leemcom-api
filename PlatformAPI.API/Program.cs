using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PlatformAPI.API.MiddleWares;
using PlatformAPI.Core.Helpers;
using PlatformAPI.EF.Data;
using PlatformAPI.EF.Hubs;
using System.Text;

namespace PlatformAPI.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Register Hangfire services
            builder.Services.AddHangfire(configuration =>
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                             .UseSimpleAssemblyNameTypeSerializer()
                             .UseRecommendedSerializerSettings()
                             .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                             {
                                 CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                 SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                 QueuePollInterval = TimeSpan.Zero,
                                 UseRecommendedIsolationLevel = true,
                                 DisableGlobalLocks = true
                             }));

            // Add the Hangfire server
            builder.Services.AddHangfireServer();

            var apiBaseUrl = builder.Configuration.GetValue<string>("AppSettings:ApiBaseUrl");

            // Configure HttpClient with the retrieved base URL
            builder.Services.AddHttpClient("QuizApiClient", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            // Configure the password constraints for Identity
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

            // Configure DbContext
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
            builder.Services.AddTransient<NotificationService, NotificationService>();
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

            // Swagger/OpenAPI setup
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Ta3lem Com For Testing",
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

                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Check if the token is available in cookies or query string (for SignalR WebSocket)
                        var token = context.Request.Cookies["token"] ?? context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token; // Assign the token
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            // Enable CORS
            builder.Services.AddCors();

            // AutoMapper configuration
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

            // Enable Swagger for API documentation
            app.UseSwagger();
            app.UseSwaggerUI();

            // Enable HTTPS redirection
            app.UseHttpsRedirection();

            // CORS configuration
            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(origin => true));

            // Enable authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Middleware (commented out for now)
            //app.UseMiddleware<TeacherIsSubsMiddleware>();

            // Add routing for SignalR
            app.MapHub<NotificationHub>("/notificationHub");

            // Configure Hangfire dashboard for monitoring background jobs
            app.UseHangfireDashboard("/dashboard");

            // Schedule the recurring Hangfire job for the vacation escalation process
            RecurringJob.AddOrUpdate<QuizNotificationService>(
                "escalate-vacation-requests",
                job => job.CheckEndedQuizzesAsync(),
                Cron.MinuteInterval(5) // This schedules the job to run every 6 hours
                );

            // Map controllers
            app.MapControllers();

            // Run the app
            app.Run();
        }
    }
}
