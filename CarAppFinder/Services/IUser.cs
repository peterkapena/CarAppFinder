using CarAppFinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarAppFinder.Services
{
    public interface IUserService
    {
        public Task<IdentityResult> CreateAsync(XUser xUser);
        public Task<string> GetAuthToken(User xUser);
    }

    public class UserService : IUserService
    {
        public UserManager<User> UserManager { get; set; }
        public RoleManager<IdentityRole> RoleManager { get; set; }
        public Setting.Setting Setting { get; set; }
        public TokenValidationParameters TokenValidationParameters { get; }
        public DatabaseContext Context { get; set; }

        public enum UserRoles
        {
            User, Admin
        }

        public UserService(UserManager<User> userMgr,
                           DatabaseContext context,
                           Setting.Setting setting,
                           TokenValidationParameters tokenValidationParameters)
        {
            UserManager = userMgr;
            Context = context;
            Setting = setting;
            TokenValidationParameters = tokenValidationParameters;
        }

        public async Task<IdentityResult> CreateAsync(XUser xUser)
        {

            var user = new User
            {
                Email = xUser.Email,
                UserName = $"{UserManager.Users.ToListAsync().Result.Count + 1}"
            };
            IdentityResult result = await UserManager.CreateAsync(user, xUser.Password);

            return result;
        }

        public async Task<string> GetAuthToken(User user)
        {
            var userRoles = await UserManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, user.Id),
                        new Claim(ClaimTypes.Name, user.Email)
                    };

            //Add the user roles in the claim so that the the role will be used for authorisation
            foreach (var role in userRoles) authClaims.Add(new Claim(ClaimTypes.Role, role));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Setting.JwtSetting.IssuerSigningKey));

            var signingCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

            //If production we set the token lifetime to 8 hours, otherwise the lifetime is some seconds
            /*0.000555556 is two second in unit of hours*/
            var expires = DateTime.Now.AddHours(Setting.JwtSetting.TokenLifeTime);

            var securityToken = new JwtSecurityToken(Setting.JwtSetting.Issuer, Setting.JwtSetting.Audience, authClaims, null, expires, signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }

    public class XUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string TokenForAnonymous { get; set; }
    }
}
