using CarAppFinder.Models;
using CarAppFinder.Services;
using CarAppFinder.Services.Bug;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CarAppFinder.Controllers.Identity
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseAPI
    {
        #region 'Construcors'
        public UserController(UserManager<User> userMgr,
                              DatabaseContext context,
                              Services.Setting.Setting setting,
                              RoleManager<IdentityRole> roleManager,
                              IUserService userService,
                              IErrorLogService errorLogService,
                              TokenValidationParameters tokenValidationParameters)
         : base(userMgr: userMgr, context: context, setting: setting, roleManager: roleManager, errorLogService: errorLogService)
        {
            UserService = userService;
            TokenValidationParameters = tokenValidationParameters;
        }

        public IUserService UserService { get; }
        public TokenValidationParameters TokenValidationParameters { get; }

        [AllowAnonymous]
        #endregion.
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] XUser xUser)
        {
            try
            {
                AuthenticateAnonymous(xUser.TokenForAnonymous);

                var user = await UserManager.FindByEmailAsync(xUser.Email);

                if (user is null)
                {
                    var result = await UserService.CreateAsync(xUser);
                    if (result.Succeeded)
                    {
                        user = await UserManager.FindByEmailAsync(xUser.Email);
                        string token = await UserService.GetAuthToken(user);
                        ReturnValue = GetReturnForLogin(user, token);
                    }
                    else SetReturnValue("error", result.Errors.ToList());
                }
                else
                {
                    if (await UserManager.CheckPasswordAsync(user, xUser.Password))
                    {
                        string token = await UserService.GetAuthToken(user);
                        ReturnValue = GetReturnForLogin(user, token);
                    }
                    else
                    {
                        SetReturnValue("error", "Login failed");
                        return Unauthorized(ReturnValue);
                    }
                }
                return Ok(ReturnValue);
            }
            catch (Exception ex)
            {
                return await GetErrorMessageResponse(ex, xUser.Email);
            }
        }
        private static Dictionary<string, object> GetReturnForLogin(User u, string token)
        {
            var user = new Dictionary<string, object>
            {
                { "id", u.Id },
                { "email", u.Email },
            };

            return new Dictionary<string, object> {
                { "token",token},
                { "user", user },
            };
        }
    }
}
