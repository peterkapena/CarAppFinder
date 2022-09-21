using CarAppFinder.Models;
using CarAppFinder.Services;
using CarAppFinder.Services.Bug;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CarAppFinder.Controllers
{
    public abstract class BaseAPI : ControllerBase
    {
        public UserManager<User> UserManager { get; set; }
        public RoleManager<IdentityRole> RoleManager { get; set; }
        public IErrorLogService ErrorLogService { get; }
        public SignInManager<User> SignInManager { get; set; }
        public DatabaseContext Context { get; set; }
        public Dictionary<string, object> ReturnValue;

        public IConfiguration Configuration { get; }
        public Services.Setting.Setting Setting { get; }

        public Task<User> AuthenticatedUser
        {
            get
            {
                return Task.Run(async () =>
                {
                    User user = null;
                    try
                    {
                        var claimsIdentity = this.User.Identity as ClaimsIdentity;
                        var userId = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                        user = await UserManager.FindByIdAsync(userId);
                    }
                    catch { }

                    return user;
                });
            }
        }
        public BaseAPI(UserManager<User> userMgr,
                       IErrorLogService errorLogService,
                       SignInManager<User> signinMgr = null,
                       DatabaseContext context = null,
                       IConfiguration configuration = null,
                       Services.Setting.Setting setting = null,
                       RoleManager<IdentityRole> roleManager = null)
        {
            UserManager = userMgr;
            SignInManager = signinMgr;
            Context = context;
            ReturnValue = new Dictionary<string, object>();
            Configuration = configuration;
            Setting = setting;
            RoleManager = roleManager;
            ErrorLogService = errorLogService;
        }

        [NonAction]
        protected virtual void SetReturnValue(string key, object value)
        {
            ReturnValue ??= new Dictionary<string, object>();

            if (ReturnValue.ContainsKey(key))
            {
                ReturnValue.Remove(key);
            }

            ReturnValue.Add(key, value);
        }

        [NonAction]
        public virtual void AuthenticateAnonymous(string token)
        {
            var JwtSettingtoken = Setting.JwtSetting.IssuerSigningKey.Split(".");
            if (JwtSettingtoken.Last() != token)
                throw new System.Security.Authentication.AuthenticationException("Invalid token received");
        }

        internal virtual ObjectResult GetErrorMessageResponse(Exception ex, XUser xUser = null, string userId = null)
        {
            SetReturnValue("error", "An error happened. Please contact support.");
            if (userId != null)
            {
                _ = ErrorLogService.RegisterError(ex, UserManager?.FindByIdAsync(userId)?.Result);
            }
            else if (xUser != null)
                _ = ErrorLogService.RegisterError(ex, xUser?.User);
            else _ = ErrorLogService.RegisterError(ex);

            return StatusCode(500, ReturnValue);
        }
    }
}
