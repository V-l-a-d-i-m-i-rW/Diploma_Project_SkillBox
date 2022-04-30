using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebSite.Interfaces;
using SkillProfi_WebSite.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SkillProfi_WebSite.Controllers
{
    [Authorize(Roles = "administrator")]
    public class BidDataController : Controller
    {
        private readonly ISkillProfiData data;
        public BidDataController(ISkillProfiData data)//(,ILogger<HomeController> logger,
        {
            //_logger = logger;
            this.data = data;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region заявки
        #region страница заявок редактирвование
        /// <summary>
        /// post запрос на сохранение данных для отображения формы заявки
        /// </summary>
        /// <param name="bidPageData">измененные данные для отображения формы заявки</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveBibPageData(BidPageData bidPageData)
        {
            try
            {
                if (ModelState.IsValid && await data?.SetBidPageDataAsync(bidPageData))
                {
                    return RedirectToAction("Index", "BidData");
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// get запрос на изменение данных для отображения формы заявки
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditBidPageData()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                return View("BidPageEdit", await data?.GetBidPageDataAsync());
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }
        #endregion
        /// <summary>
        /// get  запрос отображение формы заявки
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                Bid bid = new();
                await data?.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                ViewData["Header"] = $"{ViewData["MainPageHeader"]}";//"Главная";
                await data?.SetBidFormDataInViewDataDictionaryAsync(ViewData);
                return View(bid);
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }
        /// <summary>
        /// Post запрос добавление заявки в БД 
        /// </summary>
        /// <param name="newBid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendBid(Bid newBid)
        {
            try
            {
                if (ModelState.IsValid && newBid != null)
                {
                    await data?.AddBidAsync(newBid);
                }
                else
                    return NotFound();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        #region выбор заявок по периодам
        /// <summary>
        /// Получить список заявок за сегодня
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBidsToday()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                AddFilterData("Today", new DateTime(0), new DateTime(0));
                return View("Bids", await data.GetBidForThePeriodAsync(DateTime.Now, DateTime.Now));
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Получить список заявок за вчерашний день
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBidsYesterday()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                AddFilterData("Yesterday", new DateTime(0), new DateTime(0));
                DateTime date = DateTime.Now.AddDays(-1);
                return View("Bids", await data.GetBidForThePeriodAsync(date, date));
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Получить список заявок за неделю
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBidsWeek()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                AddFilterData("Week", new DateTime(0), new DateTime(0));
                DateTime date = DateTime.Now.AddDays(-7);
                return View("Bids", await data.GetBidForThePeriodAsync(date, DateTime.Now));
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Получить список заявок за месяц
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBidsMonth()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                AddFilterData("Month", new DateTime(0), new DateTime(0));
                DateTime date = DateTime.Now.AddMonths(-1);
                return View("Bids", await data.GetBidForThePeriodAsync(date, DateTime.Now));
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }
        /// <summary>
        /// Получить страницу заявок
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBids()
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                DateTime date = new(0);
                AddFilterData("", date, date);
                return View("Bids", await data.GetBidForThePeriodAsync(date, date));
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Получить список заявок период
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetBidsPeriod(DateTime fromDate, DateTime up_toDate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                    AddFilterData("Period", fromDate, up_toDate);
                    return View("Bids", await data.GetBidForThePeriodAsync(fromDate, up_toDate));
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }
        #endregion

        #region изменение статуса заявки
        /// <summary>
        /// get запрос изменение статуса заявки
        /// </summary>
        /// <param name="Id">Идентификатор заявки</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditBidStatus(int? Id)
        {
            try
            {
                await data.SetGeneralDataInViewDataDictionaryAsync(ViewData, User.IsInRole("administrator"));
                Bid current_bid = await data?.GetBidByIdAsync(Id);
                if (current_bid != null)
                {
                    return View("BidStateEdit", current_bid);
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
        /// post запрос изменение статуса заявки
        /// </summary>
        /// <param name="Id">Идентификатор заявки</param>
        /// <param name="SelectedBidStutus">измененный статус</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditBidStatus(int? Id, BidStatusEnum SelectedBidStutus)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await data?.EditBidStatusAsync(Id, SelectedBidStutus))
                    {
                        string filter = Request.Cookies.ContainsKey("PressedButton") ? Request.Cookies["PressedButton"] : string.Empty;
                        switch (filter)
                        {
                            case "Today": return await GetBidsToday();
                            case "Yesterday": return await GetBidsYesterday();
                            case "Week": return await GetBidsWeek();
                            case "Month": return await GetBidsMonth();
                            case "Period":
                                DateTime fromPeriod, Up_toPeriod;
                                string fromPeriod_str, Up_toPeriod_str;
                                Request.Cookies.TryGetValue("FromPeriod", out fromPeriod_str);
                                Request.Cookies.TryGetValue("Up_toPeriod", out Up_toPeriod_str);
                                _ = DateTime.TryParse(fromPeriod_str, out fromPeriod);
                                _ = DateTime.TryParse(Up_toPeriod_str, out Up_toPeriod);
                                return await GetBidsPeriod(fromPeriod, Up_toPeriod);
                            default: return RedirectToAction("GetBids", "BidData");
                        }

                    }
                    else
                        return NotFound();
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return View("Error", ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Добавление данных в ViewData и Cookies для фильтрации заявок
        /// </summary>
        /// <param name="key">текстовый ключ</param>
        /// <param name="fromDate">дата для фильтрации начало периода</param>
        /// <param name="up_toDate">дата для фильтрации конец периода</param>
        private void AddFilterData(string key, DateTime fromDate, DateTime up_toDate)
        {
            if (!string.IsNullOrEmpty(key))
            {
                ViewData["PressedButton"] = key;
                Response.Cookies.Append("PressedButton", key);
            }
            else
            {
                Response.Cookies.Delete("PressedButton");
            }
            DateTime nullDate = new(0);
            if (fromDate != nullDate && up_toDate != nullDate)
            {
                ViewData["FromPeriod"] = fromDate.ToString("yyyy-MM-dd");
                ViewData["Up_toPeriod"] = up_toDate.ToString("yyyy-MM-dd");
                Response.Cookies.Append("FromPeriod", fromDate.ToString("yyyy-MM-dd"));
                Response.Cookies.Append("Up_toPeriod", up_toDate.ToString("yyyy-MM-dd"));
            }
            else
            {
                Response.Cookies.Delete("FromPeriod");
                Response.Cookies.Delete("Up_toPeriod");
            }
        }
        #endregion

        #endregion
    }
}
