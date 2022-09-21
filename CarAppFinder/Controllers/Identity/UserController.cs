using CarAppFinder.Models;
using CarAppFinder.Services;
using CarAppFinder.Services.Bug;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                //AuthenticateAnonymous(xUser.TokenForAnonymous);

                var result = await UserService.CreateAsync(xUser);
                //If succeded we set the JSON return value from the XUser internal SetReturnForPost method
                //Otherwise we set the JSON with an error list
                if (result.Succeeded)
                {
                    ReturnValue = GetReturnForPost(xUser);
                }
                else SetReturnValue("error", result.Errors.ToList());

                return Ok(ReturnValue);
            }
            catch (Exception ex)
            {
                return GetErrorMessageResponse(ex, xUser);
            }
        }

        [AllowAnonymous]
        [HttpPatch]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody] XUser xUser)
        {
            try
            {
                //AuthenticateAnonymous(xUser.TokenForAnonymous);

                var user =
                    xUser.User.UserName.Contains('@') ?
                    await UserManager.FindByEmailAsync(xUser.User.UserName) :
                await UserManager.FindByNameAsync(xUser.User.UserName);

                if (xUser.User != null && await UserManager.CheckPasswordAsync(user, xUser.User.PassWord))
                {
                    xUser.User = user;

                    string token = await UserService.GetAuthToken(xUser);

                    ReturnValue = GetReturnForLogin(xUser, token);
                }
                else
                {
                    SetReturnValue("error", "Login failed");
                    return Unauthorized(ReturnValue);
                }

                return Ok(ReturnValue);
            }
            catch (Exception ex)
            {
                return GetErrorMessageResponse(ex, xUser);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmail([FromBody] XUser xUser, string t)
        {
            try
            {
                //AuthenticateAnonymous(xUser.TokenForAnonymous);

                var user = await UserManager.FindByNameAsync(xUser.User.UserName);

                if (xUser.User != null)
                {
                    var decodedToken = WebEncoders.Base64UrlDecode(t);
                    var tokenDecoded = Encoding.UTF8.GetString(decodedToken);

                    xUser.User = user;
                    var confirmEmailResult = await UserManager.ConfirmEmailAsync(xUser.User, tokenDecoded);
                    if (!confirmEmailResult.Succeeded)
                    {
                        SetReturnValue("error", confirmEmailResult.Errors.ToList());
                    }
                    else SetReturnValue("userId", xUser.User.Id);
                }
                else
                {
                    SetReturnValue("error", "Unauthorized");
                    return Unauthorized(ReturnValue);
                }

                return Ok(ReturnValue);
            }
            catch (Exception ex)
            {
                SetReturnValue("error", "An error happened. Please contact support.");
                await ErrorLogService.RegisterError(ex, xUser.User);
                return StatusCode(500, ReturnValue);
            }
        }

        private Dictionary<string, object> GetReturnForLogin(XUser xUser, string token)
        {
            var user = new Dictionary<string, object>
            {
                { "id", xUser.User.Id },
                { "name", xUser.User.Name },
                { "userName", xUser.User.UserName },
                { "surname", xUser.User.Surname },
                { "email", xUser.User.Email },
                { "phoneNumber", xUser.User.PhoneNumber }
            };

            return new Dictionary<string, object> {
                { "token",token},
                { "user", user },
            };
        }
        private Dictionary<string, object> GetReturnForPost(XUser xUser)
        {
            var add = new Dictionary<string, object>
            {
                { "id", xUser.User.Id },
                { "name", xUser.User.Name },
                { "userName", xUser.User.UserName },
                { "surname", xUser.User.Surname },
                { "email", xUser.User.Email },
                { "phoneNumber", xUser.User.PhoneNumber },
            };
            var user = new Dictionary<string, object>
            {
                { "id", xUser.User.Id },
                { "name", xUser.User.Name },
                { "userName", xUser.User.UserName },
                { "surname", xUser.User.Surname },
                { "email", xUser.User.Email },
                { "phoneNumber", xUser.User.PhoneNumber },
            };

            return new Dictionary<string, object> { { "user", user }, { "address", add } };
        }
    }
}
