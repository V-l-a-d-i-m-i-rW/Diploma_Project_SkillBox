using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillProfi_WebAPI.Controllers
{
    /// <summary>
    /// цитаты заголовка "крылатые фразы"
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HeaderQuoteController : ControllerBase
    {
        #region поля
        /// <summary>
        /// ссылка на контекст БД
        /// </summary>
        private readonly SkillProfiContext db;
        #endregion
        #region коннструкторы
        public HeaderQuoteController(SkillProfiContext context)
        {
            db = context;
        }
        #endregion

        /// <summary>
        /// получить все цитаты для заголовка (GET api/HeaderQuote)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<HeaderDescription>> GetAllQuoteInTheHeaderAsync()
        {
            return await Task.Run(() => db?.HeaderDescriptions?.ToList());
        }

        /// <summary>
        /// получение цитаты для заголовка (GET api/HeaderQuote/1)
        /// </summary>
        /// <returns></returns>
        [HttpGet("1")]
        public async Task<string> GetQuoteInTheHeaderAsync()
        {
            List<HeaderDescription> headers = await GetAllQuoteInTheHeaderAsync();
            if (headers?.Count > 0)
            {
                Random rnd = new();
                return headers[rnd.Next(0, headers.Count)].Description;
            }
            return "";
        }

        /// <summary>
        /// записать цитаты для заголовка (PUT api/HeaderQuote/)
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task SetQuoteInTheHeaderAsync([FromBody] List<HeaderDescription> headers)
        {
            if (headers != null)
            {
                List<HeaderDescription> cur_headers = await GetAllQuoteInTheHeaderAsync();
                if (headers.Count > 0)
                {
                    foreach (HeaderDescription header in headers)
                    {
                        if (header.Id == 0)
                        {                          
                            db?.HeaderDescriptions.Add(header);
                            cur_headers.Add(header); 
                            header.Saved = true;
                        }
                        else
                        {
                            HeaderDescription edit_Header = cur_headers?.FirstOrDefault(i => i.Id == header.Id);
                            if (edit_Header != null)
                            {
                                edit_Header.Description = header.Description;
                                edit_Header.Saved = true;
                            }
                        }
                    }
                }
                foreach (HeaderDescription head in cur_headers) if (!head.Saved) db.HeaderDescriptions.Remove(head);
                await db.SaveChangesAsync();
            }
        }
    }
}
