using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebAPI.Models.DBData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillProfi_WebAPI.Controllers
{
    /// <summary>
    /// данные страницы Услуги
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceDataController : ControllerBase
    {
        #region поля
        /// <summary>
        /// ссылка на контекст БД
        /// </summary>
        private readonly SkillProfiContext db;
        #endregion
        #region коннструкторы
        public ServiceDataController(SkillProfiContext context)
        {
            db = context;
        }
        #endregion

        /// <summary>
        /// список услуг (GET api/ServiceData)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Service>> GetServiсesAsync()
        {
            return await Task.Run(() => db?.Services?.ToList());
        }

        /// <summary>
        /// получить услугу по идентификатору (GET api/ServiceData/Id)
        /// </summary>
        /// <param name="Id">идентификатор услуги</param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public async Task<Service> GetServiсeByIdAsync(int? Id)
        {
            return await Task.Run(() => db?.Services?.FirstOrDefault(s => s.Id == Id));
        }

        /// <summary>
        /// удалить услугу по идентификатору (DELETE api/ServiceData/Id)
        /// </summary>
        /// <param name="Id">идентификатор услуги</param>
        /// <returns></returns>
        [HttpDelete("{Id}")]
        public async Task RemoveServiceByIdAsync(int? Id)
        {
            Service removeService = await GetServiсeByIdAsync(Id);
            if (removeService != null)
            {
                db.Services.Remove(removeService);
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// редактировать или добавить услугу (PUT api/ServiceData)
        /// </summary>
        /// <param name="service">отредактированная услуга</param>
        /// <returns></returns>
        [HttpPut]
        public async Task EditServiceAsync([FromBody]Service service)
        {
            if (service != null)
            {
                Service cur_service = await GetServiсeByIdAsync(service.Id);
                if (cur_service != null)
                {
                    cur_service.Header = service.Header;
                    cur_service.Description = service.Description;
                    await db.SaveChangesAsync();
                }
                
            }
        }

        /// <summary>
        /// редактировать или добавить услугу (POST api/ServiceData)
        /// </summary>
        /// <param name="service">добавляемая услуга</param>
        /// <returns></returns>
        [HttpPost]
        public async Task CreateServiceAsync([FromBody]Service service)
        {
            if (service != null)
            {
                await db.Services.AddAsync(service);
                await db.SaveChangesAsync();           
            }
        }
    }
}
