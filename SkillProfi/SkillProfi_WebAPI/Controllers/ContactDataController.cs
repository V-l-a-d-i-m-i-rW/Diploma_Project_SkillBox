using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillProfi_Shared;
using SkillProfi_WebAPI.Models.DBData;
using System.Linq;
using System.Threading.Tasks;

namespace SkillProfi_WebAPI.Controllers
{
    /// <summary>
    /// данные страницы Контакты
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContactDataController : ControllerBase
    {
        #region поля
        /// <summary>
        /// ссылка на контекст БД
        /// </summary>
        private readonly SkillProfiContext db;
        #endregion
        #region коннструкторы
        public ContactDataController(SkillProfiContext context)
        {
            db = context;
        }
        #endregion

        /// <summary>
        /// Получить контакты  (GET api/ContactData)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Contact> GetContact()
        {
            return await Task.Run(() => db?.Contacts.Include(x => x.Links).Where(p => p.Id != 0).FirstOrDefault());
        }

        /// <summary>
        /// получить контакт по идентификатору (GET api/ContactData/Id)
        /// </summary>
        /// <param name="Id">идентификатор контакта</param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public async Task<Contact> GetContactByIdAsync(int? Id)
        {
            return await Task.Run(() => db?.Contacts?.Include(x => x.Links).Where(p => p.Id == Id).FirstOrDefault());
        }

        /// <summary>
        /// редактировать контакт (PUT api/ContactData)
        /// </summary>
        /// <param name="contact">отредактированный контакт</param>
        /// <returns></returns>
        [HttpPut]
        public async Task EditContactAsync([FromBody] Contact contact)
        {
            if (contact != null)
            {
                //contact.Image = SkillProfiHelper.GetImageToImageByte(contact.ImageFormFile);
                Contact cur_contact = await GetContactByIdAsync(contact.Id);
                if (cur_contact != null)
                {
                    cur_contact.Address = contact.Address;
                    if (contact.Image?.Length > 0)
                        cur_contact.Image = contact.Image;
                    if (contact?.Links?.Count > 0)
                    {
                        foreach (ContactLink link in contact.Links)
                        {
                            if (link.Id == 0)
                            {
                                link.Saved = true;
                                db?.Links.Add(link);
                                cur_contact.Links.Add(link);
                                link.Saved = true;
                            }
                            else
                            {
                                ContactLink cur_link = cur_contact?.Links?.FirstOrDefault(i => i.Id == link.Id);
                                //link.Image = SkillProfiHelper.GetImageToImageByte(link.ImageFormFile);
                                if (cur_link != null)
                                {
                                    cur_link.Description = link.Description;
                                    if (link.Image?.Length > 0)
                                        cur_link.Image = link.Image;
                                    cur_link.Saved = true;
                                }
                            }
                        }
                        //cur_contact.Links.RemoveAll(p => !p.Saved);
                        foreach (ContactLink ren_link in cur_contact.Links) if (!ren_link.Saved) db.Links.Remove(ren_link);
                    }
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
