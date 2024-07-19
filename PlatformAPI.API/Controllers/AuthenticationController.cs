using Microsoft.AspNetCore.Identity;
using PlatformAPI.Core.Models;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AuthenticationController(IAuthService authService,UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork,IMapper mapper)
        {
            _authService = authService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!EmailValidatorService.IsValidEmailProvider(model.Email))
                return BadRequest("Invalid Email");

            model.UserName = model.Email;
            var result = await _authService.RegisterAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            if (model.Role == Roles.Teacher.ToString())
            {
                var teacher = new Teacher
                {
                    ApplicationUserId = _userManager.FindByEmailAsync(model.Email).Result.Id,
                    IsActive = true,
                    IsSubscribed = false
                };
                try
                {
                    await _unitOfWork.Teacher.AddAsync(teacher);
                    _unitOfWork.Complete();
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                
            }
            else
            {
                var parent = new Parent
                {
                    ApplicationUserId = _userManager.FindByEmailAsync(model.Email).Result.Id
                };
                try
                {
                    await _unitOfWork.Parent.AddAsync(parent);
                    _unitOfWork.Complete();
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return Ok(new { token = result.Token, expiresOn = result.ExpiresOn });
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest("UserId and token must be supplied for email confirmation.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest($"Unable to load user with ID '{userId}'.");
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Email is already confirmed");

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully.");
            }

            return BadRequest("Error confirming email.");
        }
        [HttpPost("login")]// Login for teacher or parent or admin
        public async Task<IActionResult> LoginAsync(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result=await _authService.LoginAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            var user=await _userManager.FindByEmailAsync(model.Email);

            if (!user.EmailConfirmed)
                return BadRequest("Email not confirmed");

            if(await _userManager.IsInRoleAsync(user, Roles.Teacher.ToString()))
            {
                var teacher = await _unitOfWork.Teacher.GetByAppUserIdAsync(user.Id);
                return Ok(new
                {
                    token = result.Token,
                    expiresOn = result.ExpiresOn,
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    IsActive=teacher.IsActive,
                    IsSubscribed=teacher.IsSubscribed,
                    Role=Roles.Teacher.ToString()
                });
            }
            else if(await _userManager.IsInRoleAsync(user, Roles.Parent.ToString()))
            {
                return Ok(new
                {
                    token = result.Token,
                    expiresOn = result.ExpiresOn,
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = Roles.Parent.ToString()
                });
            }
            else
            {
                return Ok(new
                {
                    token = result.Token,
                    expiresOn = result.ExpiresOn,
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role=Roles.Admin.ToString()
                });

            }

        }
        [HttpPost("student-login")]
        public async Task<IActionResult> StudentLoginAsync(StudentLoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (await _unitOfWork.Student.FindByCodeAsync(model.Code) is null)
                return BadRequest("Code is invalid!");
            var loginModel = new LoginDTO
            {
                Email = _userManager.FindByEmailAsync(model.Code + StudentConst.EmailComplete).Result.Email,
                Password = StudentConst.Password
            }; 
            var result = await _authService.LoginAsync(loginModel);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            var user = await _userManager.FindByEmailAsync(model.Code + StudentConst.EmailComplete);
            return Ok(new
            {
                token = result.Token,
                expiresOn = result.ExpiresOn,
                Name = user.Name,
                Code=model.Code,
                Role = Roles.Student.ToString()
            });
        }

    }
}