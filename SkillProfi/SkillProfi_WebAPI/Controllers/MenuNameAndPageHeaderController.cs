using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebAPI.Classes;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SkillProfi_WebAPI.Controllers
{
    /// <summary>
    /// Название страниц главного меню и заголовков страниц
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MenuNameAndPageHeaderController : ControllerBase
    {
        #region поля
        /// <summary>
        /// ссылка на контекст БД
        /// </summary>
        private readonly SkillProfiContext db;
        #endregion
        #region коннструкторы
        public MenuNameAndPageHeaderController(SkillProfiContext context)
        {
            db = context;
        }
        #endregion

        /// <summary>
        /// записать название меню и заголовков страницы (PUT api/MenuNameAndPageHeader/)
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task SetMenuNameAndPageHeaderAsync([FromBody]MenuNameAndPageHeader menuAndHeader)
        {
            if (menuAndHeader != null)
            {
                MenuNameAndPageHeader cur_menuAndHeader = await GetMenuNameAndPageHeaderAsync(menuAndHeader.Id);
                if (cur_menuAndHeader != null)
                {
                    Type type = typeof(MenuNameAndPageHeader);
                    foreach (PropertyInfo prop in type.GetProperties())
                    {
                        if (!prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                        {
                            prop?.SetValue(cur_menuAndHeader, prop?.GetValue(menuAndHeader));
                        }
                    }
                    await db?.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Получить названия меню (GET api/MenuNameAndPageHeader/)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<MenuNameAndPageHeader> GetMenuNameAndPageHeaderAsync()
        {
            return await Task.Run(() => db?.MenuNamesAndPageHeaders?.FirstOrDefault(i => i.Id > 0));
        }

        /// <summary>
        /// Получить названия меню (GET api/MenuNameAndPageHeader/id)
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<MenuNameAndPageHeader> GetMenuNameAndPageHeaderAsync(int? id)
        {
            return await Task.Run(() => db?.MenuNamesAndPageHeaders?.FirstOrDefault(i => i.Id == id));
        }
    }
}
