using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebSite.Interfaces;
using System;
using System.Threading.Tasks;

namespace SkillProfi_WebSite.Controllers
{
    [Authorize(Roles = "administrator")]
    public class BlogDataController : Controller
    {
        private readonly ISkillProfiData data;
        public BlogDataController(ISkillProfiData data)//(,ILogger<HomeController> logger,
        {
            //_logger = logger;
            this.data = data;
        }

        #region Блог
        /// <summary>
        /// get запрос редактировать блог
        /// </summary>
        /// <param name="id">идентификатор блога</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditBlog(int? id)
        {
            try
            {


                Blog cur_blog = await data?.GetBlogByIdAsync(id);
                if (cur_blog != null)
                {                
                    await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                    ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["BlogPage"]} * Редактировать";//"Главная * Блог * Редактировать";
                    return View("BlogEditOrCreate", cur_blog);
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
        /// get запрос добавить Блог
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CreateBlog()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["BlogPage"]} * Добавить";//"Главная * Блог * Добавить";
                Blog cur_Blog = new();
                return View("BlogEditOrCreate", cur_Blog);
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// post запрос добавить/редактировать блог
        /// </summary>
        /// <param name="blog">сзданный/отредактированный блог</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditOrCreateBlog(Blog blog)
        {
            try
            {
                if (ModelState.IsValid && await data?.EditOrCreateBlogAsync(blog))
                {
                    return RedirectToAction("Blogs");
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// удалить блог
        /// </summary>
        /// <param name="id">идентификатор блога</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveBlog(int? id)
        {
            try
            {
                if (ModelState.IsValid && await data?.RemoveBlogByIdAsync(id))
                {
                    return RedirectToAction("Blogs");
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// get  запрос отображение блогов
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Blogs()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["BlogPage"]}";// "Главная * Блог";
                ViewData["Header"] = $"{ViewData["BlogPageHeader"]}";//"Блог";
                return View("Blogs", await data?.GetBlogsAsync());
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// get  запрос отображения блога по Id 
        /// </summary>
        /// <param name="id">идентификатор блога</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBlog(int? id)
        {
            try
            {                
                Blog cur_blog = await data?.GetBlogByIdAsync(id);
                if (cur_blog != null)
                {
                    await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                    ViewData["Path"] = $"{ViewData["MainPage"]} * {ViewData["BlogPage"]} * {cur_blog.Header}";//"Главная * Блог * " + ;
                    return View("Blog", cur_blog);
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }
        #endregion
    }
}
