using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebSite.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkillProfi_WebSite.Controllers
{
    [Authorize(Roles = "administrator")]
    public class EditInterfaceController : Controller
    {
        private readonly ISkillProfiData data;
        public EditInterfaceController(ISkillProfiData data)//(,ILogger<HomeController> logger,
        {
            //_logger = logger;
            this.data = data;
        }

        /// <summary>
        /// get запрос редакторования фраз блока заголовка страницы
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditHeaderDescription()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                return View("EditHeaderDescription", await data?.GetAllQuoteInTheHeaderAsync());
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }
        /// <summary>
        /// post запрос сохранить фразы блока заголовка страницы
        /// </summary>
        /// <param name="headers">отредактированная запись фраз блока заголовка страницы</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveHeaderDescription(List<HeaderDescription> headers)
        {
            try
            {
                if (ModelState.IsValid && await data?.SetQuoteInTheHeaderAsync(headers))
                {
                    return RedirectToAction("Index", "BidData");
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }
        /// <summary>
        /// get запрос редакторования названий главного меню и заголовков странницы
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditMenuNameAndPageHeader()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                return View("EditMenuNameAndPageHeader", await data?.GetMenuNameAndPageHeaderAsync());
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// post запрос редакторования названий главного менюи заголовков странницы
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveMenuNameAndPageHeader(MenuNameAndPageHeader menu)
        {
            try
            {
                if (ModelState.IsValid && await data?.SetMenuNameAndPageHeaderAsync(menu))//
                {
                    return RedirectToAction("Index", "BidData");
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
