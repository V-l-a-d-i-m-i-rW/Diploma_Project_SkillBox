using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebAPI.Models.DBData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillProfi_WebAPI.Controllers
{
    /// <summary>
    /// данные страницы Блог
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BlogDataController : ControllerBase
    {
        #region поля
        /// <summary>
        /// ссылка на контекст БД
        /// </summary>
        private readonly SkillProfiContext db;
        #endregion
        #region коннструкторы
        public BlogDataController(SkillProfiContext context)
        {
            db = context;
        }
        #endregion

        /// <summary>
        /// получить все блоги (GET api/BlogData)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Blog>> GetBlogsAsync()
        {
            return await Task.Run(() => db?.Blogs?.OrderBy(i => i.Date).ToList());
        }

        /// <summary>
        /// получить Блог по идентификатору (GET api/BlogData/Id)
        /// </summary>
        /// <param name="Id">идентификатор блога</param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public async Task<Blog> GetBlogByIdAsync(int? Id)
        {
            return await Task.Run(() => db?.Blogs?.FirstOrDefault(p => p.Id == Id));
        }

        /// <summary>
        /// редактировать блог (PUT api/BlogData)
        /// </summary>
        /// <param name="blog">отредактированный блог</param>
        /// <returns></returns>
        [HttpPut]
        public async Task EditOrCreateBlogAsync(Blog blog)
        {
            if (blog != null)
            {
                //blog.Image = SkillProfiHelper.GetImageToImageByte(blog.ImageFormFile);
                Blog cur_blog = await GetBlogByIdAsync(blog.Id);
                if (cur_blog != null)
                {
                    cur_blog.Header = blog.Header;
                    cur_blog.Description = blog.Description;
                    cur_blog.Date = blog.Date;
                    if (blog.Image?.Length > 0)
                        cur_blog.Image = blog.Image;
                    await db.SaveChangesAsync();
                }                
            }
        }

        /// <summary>
        /// добавить блог (POST api/BlogData)
        /// </summary>
        /// <param name="blog">добавляемый блог</param>
        /// <returns></returns>
        [HttpPost]
        public async Task CreateBlogAsync([FromBody] Blog blog)
        {
            if (blog?.Id == 0)
            {
                //blog.Image = SkillProfiHelper.GetImageToImageByte(blog.ImageFormFile);
                await db.Blogs.AddAsync(blog);
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// удалить блог по идентификатору (DELETE api/BlogData/Id)
        /// </summary>
        /// <param name="Id">идентификатор блога</param>
        /// <returns></returns>
        [HttpDelete("{Id}")]
        public async Task RemoveBlogByIdAsync(int? Id)
        {
            Blog removeBlog = await GetBlogByIdAsync(Id);
            if (removeBlog != null)
            {
                db.Blogs.Remove(removeBlog);
                await db.SaveChangesAsync();
            }
        }
    }
}
