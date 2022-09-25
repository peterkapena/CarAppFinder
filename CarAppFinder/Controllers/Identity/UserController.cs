using CarAppFinder.Models;
using CarAppFinder.Services;
using CarAppFinder.Services.Bug;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CarAppFinder.Controllers.Identity
{
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
                              IErrorLogService errorLogService)
         : base(userMgr: userMgr,
                context: context,
                setting: setting,
                roleManager: roleManager,
                errorLogService: errorLogService)
        {
            UserService = userService;
        }

        public IUserService UserService { get; }

        #endregion.
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] XUser xUser)
        {
            var user = await UserManager.FindByEmailAsync(xUser.Email);

            if (user is null)
            {
                var result = await UserService.CreateAsync(xUser);
                if (result.Succeeded)
                {
                    user = await UserManager.FindByEmailAsync(xUser.Email);
                    //string token = await UserService.GetAuthToken(user);
                    ReturnValue = GetReturnForLogin(user);
                }
                else SetReturnValue("error", result.Errors.ToList());
            }
            else
            {
                if (await UserManager.CheckPasswordAsync(user, xUser.Password))
                {
                    //string token = await UserService.GetAuthToken(user);
                    ReturnValue = GetReturnForLogin(user);
                }
                else
                {
                    SetReturnValue("error", "Login failed");
                    return Unauthorized(ReturnValue);
                }
            }
            return Ok(ReturnValue);
        }
        private static Dictionary<string, object> GetReturnForLogin(User u)
        {
            return new Dictionary<string, object>
            {
                { "id", u.Id },
                { "email", u.Email },
             };
        }
    }
}
