using Microsoft.AspNetCore.Identity;

namespace PlatformAPI.API.MiddleWares
{
    public class TeacherIsSubsMiddleware
    {
        private readonly RequestDelegate _next;


        public TeacherIsSubsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                // If the user is not authenticated, continue to the next middleware
                await _next(context);
                return;
            }
                using (var scope = context.RequestServices.CreateScope())
                {
                    var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    var user = await _userManager.GetUserAsync(context.User);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains(Roles.Teacher.ToString()))
                        {
                            var teacher = await _unitOfWork.Teacher.GetByAppUserIdAsync(user.Id);
        
                            if (teacher != null && teacher.IsActive && teacher.IsSubscribed)
                            {
                                await _next(context);
                                return;
                            }
                        }
                    }
                }
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

        }


    }
    }
