using AuthorsWebApi.DataTransferObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IDataProtector iDataProtector;

        public AccountsController(UserManager<IdentityUser> userManager,
                                  IConfiguration configuration,
                                  SignInManager<IdentityUser> signInManager,
                                  IDataProtectionProvider dataProtectionProvider)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.iDataProtector = dataProtectionProvider.CreateProtector("valor_único_y_quizas_secreato");
        }

        //[HttpGet("encript")]
        //public ActionResult Encript()
        //{
        //    var text = "Daniel Camilo";
        //    var cifredText = iDataProtector.Protect(text);
        //    var unCifredText = iDataProtector.Unprotect(cifredText);

        //    return Ok(new
        //    {
        //        text = text,
        //        cifredText = cifredText,
        //        unCifredText = unCifredText
        //    });
        //}

        [HttpPost("register", Name = "RegisterUser")] // api/account
        public async Task<ActionResult<UserAuthenticationResponse>> Register(UserCredentials credentials)
        {
            var NewUserResponse = await userManager
                                        .CreateAsync(new IdentityUser
                                        {
                                            UserName = credentials.Email,
                                            Email = credentials.Email
                                        }, credentials.Password);

            if (NewUserResponse.Succeeded)
            {
                return await BuildToken(credentials);        
            }
            
            return BadRequest(NewUserResponse.Errors);
        }

        [HttpGet("RenewToken", Name = "RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserAuthenticationResponse>> Renew()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim?.Value;
            var userCredentials = new UserCredentials()
            {
                Email = email
            };

            return await BuildToken(userCredentials);
        }

        [HttpPost("SetUserAdmin", Name = "SetUserAdmin")]
        public async Task<ActionResult> SetUserAdmin(UpdateUserPermissionsDto updateUserPermissionsDto)
        {
            var user = await userManager.FindByEmailAsync(updateUserPermissionsDto.Email);

            await userManager.AddClaimAsync(user, new Claim("IsAdmin", "1"));
            return NoContent(); 
        }

        [HttpPost("RemoveUserAdmin", Name = "RemoveUserAdmin")]
        public async Task<ActionResult> RemoveUserAdmin(UpdateUserPermissionsDto updateUserPermissionsDto)
        {
            var user = await userManager.FindByEmailAsync(updateUserPermissionsDto.Email);

            await userManager.RemoveClaimAsync(user, new Claim("IsAdmin", "1"));
            return NoContent();
        }

        [HttpPost("login", Name = "LoginUser")]
        public async Task<ActionResult<UserAuthenticationResponse>> Login (UserCredentials userCredentials)
        {
            var response = await 
                signInManager.PasswordSignInAsync(userCredentials.Email, 
                                                  userCredentials.Password, 
                                                  isPersistent: false,
                                                  lockoutOnFailure: false);

            if (response.Succeeded) return await BuildToken(userCredentials); 
            return BadRequest("Invalid Login.");
        }

        private async Task<UserAuthenticationResponse> BuildToken(UserCredentials userCredentials)
        {
            //Información confiable acerca de un usuario
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email)
            };

            var user = await userManager.FindByEmailAsync(userCredentials.Email);
            var userClaims = await userManager.GetClaimsAsync(user);

            claims.AddRange(userClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyJwt"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirationDate = DateTime.Now.AddHours(1);
            var securityToken = new JwtSecurityToken(issuer: null,
                                                     claims: claims,
                                                     expires: expirationDate,
                                                     signingCredentials: credential);

            return
            new UserAuthenticationResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                ExpirationDate = expirationDate
            };
        }
    }
}
