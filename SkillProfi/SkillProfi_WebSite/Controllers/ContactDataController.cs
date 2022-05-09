using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebSite.Classes;
using SkillProfi_WebSite.Interfaces;
using System;
using System.Threading.Tasks;

namespace SkillProfi_WebSite.Controllers
{
    [Authorize(Roles = "administrator")]
    public class ContactDataController : Controller
    {
        private readonly ISkillProfi data;
        public ContactDataController(ISkillProfi data)//(,ILogger<HomeController> logger,
        {
            //_logger = logger;
            this.data = data;
        }

        #region контакты
        /// <summary>
        /// get  запрос отображение формы контакты
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                ViewData["Path"] =  $"{ViewData["MainPage"]} * {ViewData["ContactPage"]}";//"Главная * Контакты"
                ViewData["Header"] = $"{ViewData["ContactPageHeader"]}" ;//"Контакты"
                Contact contact = await data?.GetContact();
                return View(contact);
            }
            catch (Exception ex)
            {
                return ExceptionView.View(ex, this);
            }
        }

        /// <summary>
        /// get запрос редактирование формы контакты
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditContact(int? id)
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["ContactPage"]} * Редактирвать";//"Главная * Контакты * Редактирвать";
                return View(await data?.GetContactByIdAsync(id));
            }
            catch (Exception ex)
            {
                return ExceptionView.View(ex, this);
            }
        }

        /// <summary>
        /// post запрос редактировать контакт
        /// </summary>
        /// <param name="contact">отредактированная запись контакта</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditContact(Contact contact)
        {
            try
            {
                if (ModelState.IsValid && await data?.EditContactAsync(contact)) //
                {
                    return RedirectToAction("Index", "ContactData");
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return ExceptionView.View(ex, this);
            }
        }
        #endregion
    }
}
