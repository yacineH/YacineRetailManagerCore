using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRMApi.Data;
using TRMApi.Models;
using TRMDLL.DataAccess;
using TRMDLL.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;


        //core utilise Dependency injection donc ApplicationDbContext on va l'injecter
        public UserController(ApplicationDbContext context,UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            UserData data = new UserData(_config);

            return data.GetUserById(userId).First();
        }


        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public List<ApplicationUserModel> GetAllUsers()
        {

            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            var users =_context.Users.ToList();
            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };

            foreach (var user in users)
            {
                var u = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email
                };

                u.Roles = userRoles.Where(x => x.UserId == u.Id).ToDictionary(key=>key.RoleId,val=>val.Name);

                output.Add(u);
            }
            return output;
        }

        [HttpGet]
        [Route("api/User/Admin/GetAllRoles")]
        [Authorize(Roles = "Admin")]
        public Dictionary<string, string> GetAllRoles()
        {
                var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);
                return roles;
        }

        [HttpPost]
        [Route("api/User/Admin/GetAllRoles")]
        [Authorize(Roles = "Admin")]
        public async Task AddRoles(UserRolePairModel pairing)
        {
               var user = await _userManager.FindByIdAsync(pairing.UserId); 
               await  _userManager.AddToRoleAsync(user,pairing.RoleName);
        }


        [HttpPost]
        [Route("api/User/Admin/RemoveARole")]
        [Authorize(Roles = "Admin")]
        public async Task RemoveARole(UserRolePairModel pairing)
        {
            var user = await _userManager.FindByIdAsync(pairing.UserId);
                       await  _userManager.RemoveFromRoleAsync(user, pairing.RoleName);
        }
    }
}
