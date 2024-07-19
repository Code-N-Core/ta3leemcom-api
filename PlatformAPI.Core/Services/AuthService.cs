using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using PlatformAPI.Core.Const;
using PlatformAPI.Core.DTOs;
using PlatformAPI.Core.Helpers;
using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlatformAPI.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailingService _mailingService;
        private readonly IUnitOfWork _unitOfWork;
        public AuthService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IMapper mapper, IOptions<JWT> jwt, IUrlHelperFactory urlHelperFactory,IHttpContextAccessor httpContextAccessor, IMailingService mailingService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _jwt = jwt.Value;
            _urlHelperFactory = urlHelperFactory;
            _httpContextAccessor = httpContextAccessor;
            _mailingService = mailingService;
            _unitOfWork = unitOfWork;
        }
        public async Task<AuthDTO> RegisterAsync(RegisterDTO model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthDTO { Message = "Email is already registerd!" };
            var user = _mapper.Map<ApplicationUser>(model);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}, ";
                }
                return new AuthDTO { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, model.Role);
            var jwtSecurityToken = await CreateJwtToken(user);

            //Verification Code
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext(
                _httpContextAccessor.HttpContext,
                _httpContextAccessor.HttpContext.GetRouteData(),
                new ActionDescriptor()));


            var verificationUrl = _httpContextAccessor.HttpContext.Request.Scheme+"://"+_httpContextAccessor.HttpContext.Request.Host
                + urlHelper.Action("ConfirmEmail", "Authentication", new {userId=userId,code=code});


            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\PlatformAPI.Core\\Templates\\EmailTemplate.html");
            var str = new StreamReader(filePath);

            var mailText = str.ReadToEnd();
            str.Close();

            mailText = mailText.Replace("[name]", user.Name).Replace("[email]", user.Email).Replace("[link]",verificationUrl);
            await _mailingService.SendEmailAsync(user.Email, "Verificstion Code",mailText);
            
            ////////////////////////////////////////

            return new AuthDTO
            {
                Email = user.Email,
                IsAuthenticated = true,
                ExpiresOn = jwtSecurityToken.ValidTo,
                Roles = new List<string> { Roles.Teacher.ToString() },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };

        }
        public async Task<AuthDTO> LoginAsync(LoginDTO model)
        {
            var authModel = new AuthDTO();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            var roles = await _userManager.GetRolesAsync(user);
            var jwtSecurityToken = await CreateJwtToken(user);
            authModel.Email = user.Email;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.IsAuthenticated = true;
            authModel.Roles = roles.ToList();
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return authModel;
        }
        
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            foreach (var role in userRoles)
            {
                roleClaims.Add(new Claim("roles", role));
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                signingCredentials: signingCredentials,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays)
                );
            return jwtSecurityToken;
        }
    }
}
