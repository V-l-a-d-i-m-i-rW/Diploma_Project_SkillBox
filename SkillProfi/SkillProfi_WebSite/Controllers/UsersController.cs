using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkillProfi_WebSite.Classes;
using SkillProfi_WebSite.Interfaces;
using SkillProfi_WebSite.UserAuthorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SkillProfi_WebSite.Controllers
{
    [Authorize(Roles = "administrator")]
    public class UsersController : Controller
    {
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ISkillProfi data;

        public UsersController( UserManager<User> userManager, ISkillProfi data)//RoleManager<IdentityRole> roleManager,
        {
            //_roleManager = roleManager;
            _userManager = userManager;
            this.data = data;
        }

        public async Task<IActionResult> Index() 
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                return View("UsersList", _userManager?.Users?.ToList());
            }
            catch (Exception ex)
            {
                return ExceptionView.View(ex, this);
            }
        }
           

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                //IdentityResult result = 
                _= await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

        //public async Task<IActionResult> EditRoles(string userId)
        //{
        //    // получаем пользователя
        //    User user = await _userManager.FindByIdAsync(userId);
        //    if (user != null)
        //    {
        //        // получем список ролей пользователя
        //        var userRoles = await _userManager.GetRolesAsync(user);
        //        var allRoles = _roleManager.Roles.ToList();
        //        ChangeRoleViewModel model = new ChangeRoleViewModel
        //        {
        //            UserId = user.Id,
        //            UserName = user.UserName,
        //            UserRoles = userRoles,
        //            AllRoles = allRoles
        //        };
        //        return View("EditRoles", model);
        //    }

        //    return NotFound();
        //}

        //[HttpPost]
        //public async Task<IActionResult> EditRoles(string userId, List<string> roles)
        //{
        //    // получаем пользователя
        //    User user = await _userManager.FindByIdAsync(userId);
        //    if (user != null)
        //    {
        //        // получем список ролей пользователя
        //        var userRoles = await _userManager.GetRolesAsync(user);
        //        // получаем все роли
        //        //var allRoles = _roleManager.Roles.ToList();
        //        // получаем список ролей, которые были добавлены
        //        var addedRoles = roles.Except(userRoles);
        //        // получаем роли, которые были удалены
        //        var removedRoles = userRoles.Except(roles);

        //        await _userManager.AddToRolesAsync(user, addedRoles);

        //        await _userManager.RemoveFromRolesAsync(user, removedRoles);

        //        return RedirectToAction("Index");
        //    }

        //    return NotFound();
        //}
    }
}
