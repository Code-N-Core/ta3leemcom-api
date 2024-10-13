using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using PlatformAPI.Core.DTOs.Auth;
using PlatformAPI.Core.DTOs.Student;
using PlatformAPI.Core.Models;
using Microsoft.AspNetCore.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Authorization;


namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailingService _mailingService;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _webHostEnvironment; // Inject IWebHostEnvironment

        public AuthenticationController(IHostingEnvironment webHostEnvironment, IAuthService authService,UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork,IMapper mapper,IMailingService mailingService)
        {
            _authService = authService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mailingService= mailingService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!EmailValidatorService.IsValidEmailProvider(model.Email))
                return BadRequest("Invalid Email");

            model.UserName = model.Email;
            try
            {
                var result= await _authService.RegisterAsync(model);
                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);
                return Ok(new { token = result.Token, expiresOn = result.ExpiresOn });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
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
                    Role=Roles.Teacher.ToString(),
                    TeacherId=teacher.Id
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
                    Role = result.Roles,
                    parentId=_unitOfWork.Parent.FindTWithExpression<Parent>(p=>p.ApplicationUserId==user.Id).Result.Id
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
                Role = Roles.Student.ToString(),
                StudentId= _unitOfWork.Parent.FindTWithExpression<Student>(s => s.ApplicationUserId == user.Id).Result.Id,
                LevelId=_unitOfWork.Level.GetByIdAsync(_unitOfWork.LevelYear.
                GetByIdAsync(_unitOfWork.Group.GetByIdAsync(_unitOfWork.Student
                .GetByIdAsync(_unitOfWork.Parent.FindTWithExpression<Student>(s => s.ApplicationUserId == user.Id).Result.Id).Result.GroupId).Result.LevelYearId).Result.LevelId).Result.Id,
                LevelName= _unitOfWork.Level.GetByIdAsync(_unitOfWork.LevelYear.
                GetByIdAsync(_unitOfWork.Group.GetByIdAsync(_unitOfWork.Student
                .GetByIdAsync(_unitOfWork.Parent.FindTWithExpression<Student>(s => s.ApplicationUserId == user.Id).Result.Id).Result.GroupId).Result.LevelYearId).Result.LevelId).Result.Name,
                LevelYearId= _unitOfWork.LevelYear.
                GetByIdAsync(_unitOfWork.Group.GetByIdAsync(_unitOfWork.Student
                .GetByIdAsync(_unitOfWork.Parent.FindTWithExpression<Student>(s => s.ApplicationUserId == user.Id).Result.Id).Result.GroupId).Result.LevelYearId).Result.Id,
                LevelYearName= _unitOfWork.LevelYear.
                GetByIdAsync(_unitOfWork.Group.GetByIdAsync(_unitOfWork.Student
                .GetByIdAsync(_unitOfWork.Parent.FindTWithExpression<Student>(s => s.ApplicationUserId == user.Id).Result.Id).Result.GroupId).Result.LevelYearId).Result.Name,
                GroupId= _unitOfWork.Group.GetByIdAsync(_unitOfWork.Student
                .GetByIdAsync(_unitOfWork.Parent.FindTWithExpression<Student>(s => s.ApplicationUserId == user.Id).Result.Id).Result.GroupId).Result.Id,
                GroupName= _unitOfWork.Group.GetByIdAsync(_unitOfWork.Student
                .GetByIdAsync(_unitOfWork.Parent.FindTWithExpression<Student>(s => s.ApplicationUserId == user.Id).Result.Id).Result.GroupId).Result.Name

            });
        }
        [Authorize(Roles = "Parent,Teacher")]
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (await _unitOfWork.Parent.GetByIdAsync(model.Id)!=null)
            {
                var parent = await _unitOfWork.Parent.GetByIdAsync(model.Id);
                var user = await _userManager.FindByIdAsync(parent.ApplicationUserId);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                // Check if the current password matches
                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    return BadRequest("الباسورد الحالي غير صحييح");
                }

                // Check if the new password is the same as the old one
                if (model.CurrentPassword == model.NewPassword)
                {
                    return BadRequest("مينفعش الباسورد القديم يكون مشابه للجديد");
                }
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

                if (!result.Succeeded)
                {
                    // Return errors if password reset failed
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest($"Failed to update password: {errors}");
                }
                return Ok("تم تغير كلمة السر بنجاح");
            }
            else if(await _unitOfWork.Teacher.GetByIdAsync(model.Id) != null)
            {
                var parent = await _unitOfWork.Teacher.GetByIdAsync(model.Id);
                var user = await _userManager.FindByIdAsync(parent.ApplicationUserId);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                // Check if the current password matches
                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    return BadRequest("الباسورد الحالي غير صحييح");
                }

                // Check if the new password is the same as the old one
                if (model.CurrentPassword == model.NewPassword)
                {
                    return BadRequest("مينفعش الباسورد القديم يكون مشابه للجديد");
                }
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

                if (!result.Succeeded)
                {
                    // Return errors if password reset failed
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest($"Failed to update password: {errors}");
                }
                return Ok("تم تغير كلمة السر بنجاح");
            }
            else
                return BadRequest("Invalid id");
        }
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("لم يتم العثور علي الإيميل");

            // Generate the reset code
            string resetCode = _mailingService.GenerateCode();

            // Set the reset code and expiration time
            user.ResetPasswordCode = resetCode;
            user.ResetCodeExpiry = DateTime.UtcNow.AddMinutes(30); // Expiry time of 30 minutes

            // Save the reset code and expiration time in the database
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return StatusCode(500, "An error occurred while updating the user record.");

            // Load the email template
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "ResetPassword.html");
            string emailBody = await System.IO.File.ReadAllTextAsync(filePath);

            // Customize email body with reset code if needed
            emailBody = emailBody.Replace("{ResetCode}", resetCode);

            // Send the reset code via email
            await _mailingService.SendEmailAsync(
                model.Email,
                "Code For Reset Password",
                emailBody // Using the modified template with the reset code
            );

            return Ok("تم إرسال رمز التأكيد الي ايميلك");
        }
        [HttpPost("check-reset-code")]
        public async Task<IActionResult> CheckResetCode(CheckResetCodeDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("لم يتم العثور علي الإيميل");

            // Check if the reset code matches and has not expired
            if (user.ResetPasswordCode != model.ResetCode || user.ResetCodeExpiry < DateTime.UtcNow)
                return BadRequest("The reset code is invalid or has expired.");

            // Code is valid
            return Ok("Reset code is valid.");
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("لم يتم العثور علي الإيميل");


            // Reset the password
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Clear the reset code after successful password reset
            user.ResetPasswordCode = null;
            user.ResetCodeExpiry = null;
            await _userManager.UpdateAsync(user);

            return Ok("تم تغير كلمة السر بنجاح.");
        }
        [HttpPost("verify-parent-code")]
        public async Task<IActionResult> VerifyParentCode(VerifyParentCodeDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("لم يتم العثور علي الإيميل");

            // Check if the user is a parent
            if (!await _userManager.IsInRoleAsync(user, Roles.Parent.ToString()))
                return BadRequest("The user is not a parent.");

            // Verify the code
            if (user.VerificationCode != model.VerificationCode)
                return BadRequest("The verification code is invalid.");

            // Code matches, mark IsConfirmed as true
            user.EmailConfirmed = true;

            // Clear the verification code to prevent reuse
            user.VerificationCode = null;

            // Update the user in the database
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest("Failed to update confirmation status.");

            var loginModelDto = new LoginDTO
            {
                Email=model.Email,
                Password=model.Password
            };
            var result2 = await _authService.LoginAsync(loginModelDto);

            if (!result2.IsAuthenticated)
                return BadRequest(result2.Message);

            var parent = await _unitOfWork.Parent.GetByAppUserIdAsync(user.Id);
            return Ok(new
            {
                token = result2.Token,
                expiresOn = result2.ExpiresOn,
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = result2.Roles,
                ParentId = parent.Id
            });
        }
        [HttpPost("resend-verification-code")]
        public async Task<IActionResult> ResendVerificationCode(ResendVerificationCodeDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("لم يتم العثور علي الإيميل");

            // Check if the user is already confirmed
            if (user.EmailConfirmed)
                return BadRequest("The account is already confirmed.");

            // Generate a new 6-digit verification code
            var newVerificationCode = new Random().Next(100000, 999999).ToString();

            // Store the new verification code
            user.VerificationCode = newVerificationCode;

            // Update the user in the database
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return StatusCode(500, "An error occurred while updating the user record.");

            // Load email template
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "ParentVerificationCodeTemplate.html");

            var mailText = await System.IO.File.ReadAllTextAsync(filePath);
            mailText = mailText.Replace("[name]", user.Name)
                               .Replace("[email]", user.Email)
                               .Replace("[code]", newVerificationCode); // Replace link with the new verification code

            // Send the email with the new verification code
            await _mailingService.SendEmailAsync(user.Email, "Resend Verification Code", mailText);

            return Ok("A new verification code has been sent to your email.");
        }

    }
}