using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkillProfi_WebSite.Interfaces;
using SkillProfi_WebSite.UserAuthorization;

namespace SkillProfi_WebSite.Controllers
{
    public class AccountController : Controller
    {
        //private readonly ILogger log;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ISkillProfi data;

        public AccountController(UserManager<User> userManager, 
                                SignInManager<User> signInManager, ISkillProfi data)
                               // ,ILoggerFactory Log)
        {
            //this.log = Log.CreateLogger(">>> Мой Logger ");
            _userManager = userManager;
            _signInManager = signInManager;
            this.data = data;
        }

        /// <summary>
        /// регистрация нового пользователя GET запрос
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Register()
        {
            await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
            return View("Register", new UserRegistration());
        }

        /// <summary>
        /// регистрация нового пользователя подтверждение POST запрос
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Register(UserRegistration model)
        {
            if (ModelState.IsValid)
            {

                var user = new User { UserName = model.LoginUser };
                // добавляем пользователя
                var createResult = await _userManager.CreateAsync(user, model.Password);
                //если пользователь удачно добавлен
                if (createResult.Succeeded)
                {
                    //установкароли пользователь
                    await _userManager.AddToRoleAsync(user, RoleInitializer.RoleAdmin);
                    // установка куки
                    //await _signInManager.SignInAsync(user, false);
                   // _ = await data.SetGeneralDataAndGetMenuAsync(ViewData);
                    return RedirectToAction("Index", "Users");
                }
                else//иначе
                {
                    foreach (var identityError in createResult.Errors) 
                    {
                        ModelState.AddModelError("", identityError.Description);
                    }
                }
            }
            await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "BidData");
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
            //log.LogWarning($" ------- \n >> Login(string returnUrl) сработал, returnUrl = {returnUrl}\n ------- \n ");
            return View(new UserLogin()
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLogin model)
        {
           
            if (ModelState.IsValid)
            {
                var loginResult = await _signInManager.PasswordSignInAsync(model.LoginUser,
                    model.Password,
                    false,
                    lockoutOnFailure: false);

                if (loginResult.Succeeded)
                {
                    
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "BidData");
                }

            }
            await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
            ModelState.AddModelError("", "Неправильный логин и (или) пароль");
            return View(model);
        }

    }
}