using CarAppFinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CarAppFinder.Services
{
    public interface IUserService
    {
        public Task<IdentityResult> CreateAsync(XUser xUser);
        public Task<string> GetAuthToken(XUser xUser);
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

        public UserService(UserManager<User> userMgr, DatabaseContext context, Setting.Setting setting, TokenValidationParameters tokenValidationParameters)
        {
            UserManager = userMgr;
            Context = context;
            Setting = setting;
            TokenValidationParameters = tokenValidationParameters;
        }

        public async Task<IdentityResult> CreateAsync(XUser xUser)
        {
            xUser.User.UserName = (xUser.User.Name + (UserManager.Users.ToListAsync().Result.Count + 1))
                .Trim().Replace(" ", "");

            IdentityResult result = await UserManager.CreateAsync(xUser.User, xUser.User.PassWord);

            return result;
        }

        public async Task<string> GetAuthToken(XUser xUser)
        {
            var userRoles = await UserManager.GetRolesAsync(xUser.User);
            var authClaims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, xUser.User.Id),
                        new Claim(ClaimTypes.Name, xUser.User.Name)
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
        public User User { get; set; }
        public string TokenForAnonymous { get; set; }
    }
}
