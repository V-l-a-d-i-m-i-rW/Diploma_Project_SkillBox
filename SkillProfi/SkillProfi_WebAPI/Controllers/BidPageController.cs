using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebAPI.Classes;
using System.Linq;
using System.Threading.Tasks;

namespace SkillProfi_WebAPI.Controllers
{
    /// <summary>
    /// Данные для отображнения страницы заявок (нстройка интерфейса пользователя)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BidPageController : ControllerBase
    {
        #region поля
        /// <summary>
        /// ссылка на контекст БД
        /// </summary>
        private readonly SkillProfiContext db;
        #endregion
        #region коннструкторы
        public BidPageController(SkillProfiContext context)
        {
            db = context;
        }
        #endregion

        #region отображение формы заявки
        /// <summary>
        /// получить данные для формы заявок (GET api/BidPage)
        /// </summary>
        /// <returns></returns>
        [HttpGet] 
        public async Task<BidPageData> GetBidPageDataAsync()
        {
            return await Task.Run(() => db?.DataBidPage.FirstOrDefault(i => i.Id > 0));
        }

        /// <summary>
        /// получить данные для формы заявок (GET api/BidPage/id)
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<BidPageData> GetBidPageDataAsync(int? id)
        {
            return await Task.Run(() => db?.DataBidPage.FirstOrDefault(i => i.Id == id));
        }

        /// <summary>
        /// установить данные для формы заявок (PUT api/BidPage/)
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task SetBidPageDataAsync([FromBody] BidPageData data)
        {
            if (data != null)
            {
                //data.Image = SkillProfiHelper.GetImageToImageByte(data.ImageFormFile);
                BidPageData cur_page = await GetBidPageDataAsync(data.Id);
                if (cur_page != null)
                {
                    cur_page.HeaderButton = data.HeaderButton;
                    cur_page.HeaderForm = data.HeaderForm;
                    cur_page.HeaderImage = data.HeaderImage;
                    if (data?.Image?.Length > 0)
                        cur_page.Image = data.Image;
                    await db?.SaveChangesAsync();
                }
            }
        }
        #endregion
    }
}
