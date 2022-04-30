using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebSite.Interfaces;
using System;
using System.Threading.Tasks;

namespace SkillProfi_WebSite.Controllers
{
    [Authorize(Roles = "administrator")]
    public class ServiceDataController : Controller
    {
        private readonly ISkillProfiData data;

        public ServiceDataController(ISkillProfiData data)//(,ILogger<HomeController> logger,
        {
            //_logger = logger;
            this.data = data;
        }
        #region Услуги
        /// <summary>
        /// get  запрос отображение услуг
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Services()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["ServicePage"]}";//"Главная * Услуги";
                ViewData["Header"] = $"{ViewData["ServicePageHeader"]}";
                return View(await data?.GetServiсesAsync());
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// удалить услугу
        /// </summary>
        /// <param name="id">идентификатор услуги</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveService(int? id)
        {
            try
            {
                if (ModelState.IsValid && await data?.RemoveServiceByIdAsync(id))
                {
                    return RedirectToAction("Services");
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// post запрос добавить/редактировать услугу
        /// </summary>
        /// <param name="service">сзданная/отредактированная структура услуги</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditOrCreateService(Service service)
        {
            try
            {
                if (ModelState.IsValid && await data?.EditOrCreateServiceAsync(service))
                {
                    return RedirectToAction("Services");
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// get запрос редактировать услугу
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditService(int? id)
        {
            try
            {

                Service cur_service = await data?.GetServiсeByIdAsync(id);
                if (cur_service != null)
                {
                    await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                    ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["ServicePage"]} * Редактировать";//"Главная * Услуги * Редактировать";
                    return View("ServiceEditorCreate", cur_service);
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }
        /// <summary>
        /// get запрос создать услугу
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CreateService()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["ServicePage"]} * Добавить";//"Главная * Услуги * Добавить";
                Service cur_service = new();
                return View("ServiceEditorCreate", cur_service);
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }
        #endregion
    }
}
