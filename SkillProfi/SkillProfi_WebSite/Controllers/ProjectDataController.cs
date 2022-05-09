using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebSite.Interfaces;
using System;
using System.Threading.Tasks;
using SkillProfi_WebSite.Classes;

namespace SkillProfi_WebSite.Controllers
{
    [Authorize(Roles = "administrator")]
    public class ProjectDataController : Controller
    {
        private readonly ISkillProfi data;

        public ProjectDataController(ISkillProfi data)//(,ILogger<HomeController> logger,
        {
            //_logger = logger;
            this.data = data;
        }

        #region проекты
        /// <summary>
        /// get запрос редактировать проект
        /// </summary>
        /// <param name="id">идентификатор проекта</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditProject(int? id)
        {
            try
            {
                Project cur_project = await data?.GetProjectByIdAsync(id);
                if (cur_project != null)
                {
                    await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                    ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["ProjectPage"]} * Редактировать";//"Главная * Проекты * Редактировать";
                    return View("ProjectEditOrCreate", cur_project);
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return ExceptionView.View(ex, this);
            }
        }
        /// <summary>
        /// get запрос добавить проект
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CreateProject()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["ProjectPage"]} * Добавить";//"Главная * Проекты * Добавить";
                Project cur_project = new();
                return View("ProjectEditOrCreate", cur_project);
            }
            catch (Exception ex)
            {
                return ExceptionView.View(ex, this);
            }
        }

        /// <summary>
        /// post запрос добавить/редактировать проект
        /// </summary>
        /// <param name="project">сзданный/отредактированный проект</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditOrCreateProject(Project project)
        {
            try
            {
                if (ModelState.IsValid && await data?.EditOrCreateProjectAsync(project))
                {
                    return RedirectToAction("Projects");
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return ExceptionView.View(ex, this);
            }
        }

        /// <summary>
        /// удалить проект
        /// </summary>
        /// <param name="id">идентификатор проекта</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveProject(int? id)
        {
            try
            {
                if (ModelState.IsValid && await data?.RemoveProjectByIdAsync(id))
                {
                    return RedirectToAction("Projects");
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return ExceptionView.View(ex, this);
            }
        }

        /// <summary>
        /// get  запрос отображение проектов
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Projects()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["ProjectPage"]}"; //"Главная * Проекты";
                ViewData["Header"] = $"{ViewData["ProjectPageHeader"]}";//"Проекты";
                return View("Projects", await data?.GetProjectsAsync());
            }
            catch (Exception ex)
            {
                return ExceptionView.View(ex, this);
            }
        }

        /// <summary>
        /// get  запрос отображения проекта по Id 
        /// </summary>
        /// <param name="id">идентификатор проекта</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProject(int? id)
        {
            try
            {                
                Project cur_project = await data?.GetProjectByIdAsync(id);
                if (cur_project != null)
                {   
                    await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                    ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["ProjectPage"]} * {cur_project.Header}"; //"Главная * Проекты * "
                    return View("Project", cur_project);
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return ExceptionView.View(ex, this);
            }
        }
        #endregion
    }
}
