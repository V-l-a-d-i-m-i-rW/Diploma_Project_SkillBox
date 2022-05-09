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
    /// данные страницы Заявки ("Рабочий стол")
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BidDataController : ControllerBase
    {
        #region поля
        /// <summary>
        /// ссылка на контекст БД
        /// </summary>
        private readonly SkillProfiContext db;
        #endregion
        #region коннструкторы
        public BidDataController(SkillProfiContext context)
        {
            db = context;
        }
        #endregion

        /// <summary>
        /// добавление заявки (POST api/BidData)
        /// </summary>
        /// <param name="newBid">дбавляемая заявка</param>
        /// <returns></returns>        
        [HttpPost]
        public async Task AddBidAsync([FromBody] Bid newBid)
        {
            if (newBid != null && db?.Bids != null)
            {
                newBid.Date = DateTime.Now;
                newBid.BidStatus = BidStatusEnum.received;
                await db.Bids.AddAsync(newBid);
                await db.SaveChangesAsync(); 
            }           
        }

        /// <summary>
        /// получить записи за период (GET api/BidData/from=2020-20-20&up_to=2020-20-20)
        /// </summary>
        /// <param name="from">дата от</param>
        /// <param name="up_to">дата до</param>
        /// <returns></returns>
        [HttpGet("from={from}&up_to={up_to}")]
        public async Task<SortedBidsData> GetBidForThePeriodAsync(DateTime from, DateTime up_to)
        {
            SortedBidsData bids = new();
            if (db?.Bids?.Count() > 0 && from.Date <= up_to.Date)
            {
                bids.AllBidsCount = db.Bids.Count();
                List<Bid> bids_sort = db.Bids.AsParallel().Where(b => b.Date.Date >= from.Date && b.Date.Date <= up_to.Date).OrderBy(b => b.Id).ToList();
                if (bids_sort?.Count > 0)
                {
                    await Task.Run(() =>
                    {
                        bids.Bids = bids_sort;
                    });
                }
            }
            return bids;
        }

        /// <summary>
        /// получить запись по идентификатору записи (GET api/BidData/id)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public async Task<Bid> GetBidByIdAsync(int? Id)
        {
            if ((Id ?? 0) > 0 && db?.Bids != null)
            {
                return await Task.Run(() => db.Bids.FirstOrDefault(b => b.Id == Id));
            }
            return null;
        }

        /// <summary>
        /// post запрос изменение статуса заявки (PUT api/BidData/id=id&SelectedBidStutus=SelectedBidStutus)
        /// </summary>
        /// <param name="Id">Идентификатор заявки</param>
        /// <param name="SelectedBidStutus">измененный статус</param>
        /// <returns></returns>
        [HttpPut("Id={Id}&SelectedBidStutus={SelectedBidStutus}")]
        public async Task EditBidStatusAsync(int? Id, BidStatusEnum SelectedBidStutus)
        {
            Bid current_bid = await GetBidByIdAsync(Id);
            if (current_bid != null)
            {
                current_bid.BidStatus = SelectedBidStutus;
                await db.SaveChangesAsync();
            }
        }


    }
}
