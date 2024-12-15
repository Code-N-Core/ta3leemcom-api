using Hangfire;
using Hangfire.Dashboard;
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
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
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
                                 QueuePollInterval = TimeSpan.FromSeconds(15),
                                 SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                 CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
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
            builder.Services.AddTransient<IImageService, ImageService>();

            // Swagger/OpenAPI setup
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

                // Support file uploads
                c.OperationFilter<MultipartFormDataOperationFilter>();
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
                        var token = context.Request.Cookies["token"] ?? context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
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

            // Configure Hangfire dashboard with authorization
            app.UseStaticFiles();
            app.UseHangfireDashboard("/Hangfire", new DashboardOptions
            {
                Authorization = new[] { new AllowAllAuthorizationFilter() }
            });

            // Schedule the recurring Hangfire job for the vacation escalation process
            RecurringJob.AddOrUpdate<QuizNotificationService>(
                "check-ended-quizzes",
                job => job.CheckEndedQuizzesAsync(),
                Cron.MinuteInterval(5)
            );

            // Map controllers
            app.MapControllers();

            // Run the app
            app.Run();
        }
    }
    public class JwtAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var token = httpContext.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return false; // No token, unauthorized
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("3/FPNVEzE1vgKWNZB/nx+Sw+i994jElHCKT5U9kj4fM=")),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "SecureApi",
                ValidAudience = "SecureApiUser",
                ValidateLifetime = true
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false; // Invalid token
            }
        }
    }
    public class AllowAllAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true; // Allow all access. Replace with your logic for restricted access.
        }
    }
    // Custom Operation Filter to Handle Multipart/Form-Data Requests
    public class MultipartFormDataOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formParameters = context.MethodInfo.GetParameters()
                .Where(p => p.GetCustomAttributes(typeof(FromFormAttribute), false).Any());

            if (formParameters.Any())
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(
                                formParameters.First().ParameterType, context.SchemaRepository)
                        }
                    }
                };
            }
        }
    }
}

