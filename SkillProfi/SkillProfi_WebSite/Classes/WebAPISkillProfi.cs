using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SkillProfi_Shared;
using SkillProfi_WebSite.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkillProfi_WebSite.Classes
{
    public class WebAPISkillProfi: ISkillProfi
    {
        private readonly HttpClient httpClient;
        private readonly string url;
        public WebAPISkillProfi(IConfiguration configuration)
        {
            url = configuration?.GetSection("ConnectionServer")?.GetSection("DefaultConnection")?.Value;
            httpClient = new HttpClient();
        }

        #region названия меню, заголовков страниц, цитаты зоголовка меню
        /// <summary>
        /// записать название меню и заголовков страницы
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetMenuNameAndPageHeaderAsync(MenuNameAndPageHeader menuAndHeader)
        {
            //PUT api/MenuNameAndPageHeader
            if (menuAndHeader != null)
            {
                string send_url = url.TrimEnd('/') + "/api/MenuNameAndPageHeader";//HttpRequestMessage message
                _ = await httpClient.PutAsync(
                                                requestUri: send_url,
                                                content: new StringContent(JsonConvert.SerializeObject(menuAndHeader), Encoding.UTF8, mediaType: "application/json")
                                            );
                return true;
            }
            return false;
        }

        /// <summary>
        /// записать цитаты для заголовка
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetQuoteInTheHeaderAsync(List<HeaderDescription> headers)
        {
            //(PUT api/HeaderQuote/)
            if (headers != null)
            {
                string send_url = url.TrimEnd('/') + "/api/HeaderQuote";//HttpRequestMessage message
                _ = await httpClient.PutAsync(
                                                requestUri: send_url,
                                                content: new StringContent(JsonConvert.SerializeObject(headers), Encoding.UTF8, mediaType: "application/json")
                                            );
                return true;
            }
            return false;
        }

        /// <summary>
        /// Установить общие данные в ViewData
        /// </summary>
        /// <param name="viewData">ViewDataDictionary</param>
        /// <param name="isAdmin">пользователь админитсратор</param>
        /// <returns></returns>
        public async Task SetGeneralDataInViewDataDictionaryAsync(ViewDataDictionary viewData, bool isAdmin)
        {
            if (viewData == null) return;
            MenuNameAndPageHeader menu = await GetMenuNameAndPageHeaderAsync();
            if (!isAdmin)
                viewData["QuoteInTheHeader"] = await GetQuoteInTheHeaderAsync();
            if (menu != null)
            {
                Type type = typeof(MenuNameAndPageHeader);
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    if (!prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        viewData[prop.Name] = prop?.GetValue(menu);
                    }
                }
            }
        }


        /// <summary>
        /// получить все цитаты для заголовка
        /// </summary>
        /// <returns></returns>
        public async Task<List<HeaderDescription>> GetAllQuoteInTheHeaderAsync()
        {
            //GET api/HeaderQuote
            string send_url = url.TrimEnd('/') + "/api/HeaderQuote";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<List<HeaderDescription>>(json);
        }

        /// <summary>
        /// получение цитат для заголовка
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetQuoteInTheHeaderAsync()
        {
            //GET api/HeaderQuote/1
            string send_url = url.TrimEnd('/') + "/api/HeaderQuote/1";
            string json = await httpClient.GetStringAsync(send_url);
            return json;
        }

        /// <summary>
        /// Получить названия меню
        /// </summary>
        /// <returns></returns>
        public async Task<MenuNameAndPageHeader> GetMenuNameAndPageHeaderAsync()
        {
            //(GET api/MenuNameAndPageHeader/)
            string send_url = url.TrimEnd('/') + "/api/MenuNameAndPageHeader";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<MenuNameAndPageHeader>(json);
        }
        #endregion

        #region Заявки
        #region отображение формы заявки
        /// <summary>
        /// получить данные для формы заявок
        /// </summary>
        /// <returns></returns>
        public async Task<BidPageData> GetBidPageDataAsync()
        {
            //GET api/BidPage
            string send_url = url.TrimEnd('/') + "/api/BidPage";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<BidPageData>(json);
        }

        /// <summary>
        /// получить данные для формы заявок
        /// </summary>
        /// <returns></returns>
        public async Task<BidPageData> GetBidPageDataAsync(int? id)
        {
            //GET api/BidPage/id
            string send_url = url.TrimEnd('/') + $"/api/BidPage/{id}";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<BidPageData>(json);
        }

        /// <summary>
        /// записать данные формы заявок
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetBidPageDataAsync(BidPageData data)
        {
            //PUT api/BidPage/
            if (data != null) 
            {
                data.Image = SkillProfiHelper.GetImageToImageByte(data.ImageFormFile);
                data.ImageFormFile = null;
                string send_url = url.TrimEnd('/') + "/api/BidPage";//HttpRequestMessage message
                _ = await httpClient.PutAsync(
                                                requestUri: send_url,
                                                content: new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, mediaType: "application/json")
                                            );
                return true;
            }
            return false;
        }
        /// <summary>
        /// Установить данные в ViewData для формы заявки
        /// </summary>
        /// <param name="viewData"></param>
        /// <returns></returns>
        public async Task SetBidFormDataInViewDataDictionaryAsync(ViewDataDictionary viewData)
        {
            if (viewData == null) return;
            BidPageData data = await GetBidPageDataAsync();
            if (data != null)
            {
                viewData["ButtonTitle"] = data.HeaderButton;
                viewData["ImageHeader"] = data.HeaderImage;
                viewData["FormTitle"] = data.HeaderForm;
                viewData["Image"] = data.Image?.Length > 0 ? $"data:image/jpeg;base64,{(Convert.ToBase64String(data.Image))}" : "";
            }
        }
        #endregion

        /// <summary>
        /// добавление заявки
        /// </summary>
        /// <param name="newBid">дбавляемая заявка</param>
        /// <returns></returns>
        public async Task AddBidAsync(Bid newBid)
        {
            //POST api/BidData
            if (newBid?.Id == 0)
            {
                string send_url = url.TrimEnd('/') + "/api/BidData";//HttpRequestMessage message
                _ = await httpClient.PostAsync(
                                                requestUri: send_url,
                                                content: new StringContent(JsonConvert.SerializeObject(newBid), Encoding.UTF8, mediaType: "application/json")
                                            );
            }
        }

        /// <summary>
        /// получить записи за период
        /// </summary>
        /// <param name="from">дата от</param>
        /// <param name="up_to">дата до</param>
        /// <returns></returns>
        public async Task<SortedBidsData> GetBidForThePeriodAsync(DateTime from, DateTime up_to)
        {
            //GET api/BidData/from=2020-20-20&up_to=2020-20-20
            SortedBidsData bids = null;
            if (from.Date <= up_to.Date)
            {
                string send_url = url.TrimEnd('/') + $"/api/BidData/from={from:yyyy-MM-dd}&up_to={up_to:yyyy-MM-dd}";
                string json = await httpClient.GetStringAsync(send_url);
                bids = JsonConvert.DeserializeObject<SortedBidsData>(json);
                bids ??= new();
            }
            return bids;
        }

        /// <summary>
        /// получить запись по идентификатору записи 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<Bid> GetBidByIdAsync(int? Id)
        {
            //GET api/BidData/id
            if ((Id ?? 0) > 0)
            {
                string send_url = url.TrimEnd('/') + $"/api/BidData/{Id}";
                string json = await httpClient.GetStringAsync(send_url);
                return JsonConvert.DeserializeObject<Bid>(json);
            }
            return null;
        }

        /// <summary>
        /// изменение статуса заявки
        /// </summary>
        /// <param name="Id">Идентификатор заявки</param>
        /// <param name="SelectedBidStutus">измененный статус</param>
        /// <returns></returns>
        public async Task<bool> EditBidStatusAsync(int? Id, BidStatusEnum SelectedBidStutus)
        {
            //PUT api/BidData/id=id&SelectedBidStutus=SelectedBidStutus
            if (Id > 0)
            {
                string send_url = url.TrimEnd('/') + $"/api/BidData/id={Id}&SelectedBidStutus={SelectedBidStutus}";
                _ = await httpClient.PutAsync(
                                                requestUri: send_url,
                                                content: new StringContent("")
                                            );
                return true;
            }
            return false;
        }
        #endregion

        #region Услуги
        /// <summary>
        /// список услуг
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Service>> GetServiсesAsync()
        {
            //GET api/ServiceData
            string send_url = url.TrimEnd('/') + "/api/ServiceData";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<List<Service>>(json);
        }

        /// <summary>
        /// получить услугу по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор услуги</param>
        /// <returns></returns>
        public async Task<Service> GetServiсeByIdAsync(int? Id)
        {
            //GET api/ServiceData
            string send_url = url.TrimEnd('/') + $"/api/ServiceData/{Id}";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<Service>(json);
        }

        /// <summary>
        /// удалить услугу по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор услуги</param>
        /// <returns></returns>
        public async Task<bool> RemoveServiceByIdAsync(int? Id)
        {
            //DELETE api/ServiceData/Id
            if ((Id??0) > 0)
            {
                string send_url = url.TrimEnd('/') + $"/api/ServiceData/{Id}";
                _ = await httpClient.DeleteAsync(send_url);
                return true;
            }
            return false;
        }

        /// <summary>
        /// редактировать или добавить услугу
        /// </summary>
        /// <param name="service">отредактированная/добавляемая услуга</param>
        /// <returns></returns>
        public async Task<bool> EditOrCreateServiceAsync(Service service)
        {
            if (service != null)
            {
                string send_url = url.TrimEnd('/') + "/api/ServiceData";
                if (service.Id == 0)
                {
                    //POST api/ServiceData
                    _ = await httpClient.PostAsync(
                                requestUri: send_url,
                                content: new StringContent(JsonConvert.SerializeObject(service), Encoding.UTF8, mediaType: "application/json")
                            );
                }
                else
                {
                    //PUT api/ServiceData
                    _ = await httpClient.PutAsync(
                                requestUri: send_url,
                                content: new StringContent(JsonConvert.SerializeObject(service), Encoding.UTF8, mediaType: "application/json")
                            );
                }
                return true;
            }
            return false;
        }
        #endregion

        #region Проекты
        /// <summary>
        /// получить все проекты
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            //GET api/ProjectData
            string send_url = url.TrimEnd('/') + "/api/ProjectData";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<List<Project>>(json);
        }

        /// <summary>
        /// получить проект по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор проекта</param>
        /// <returns></returns>
        public async Task<Project> GetProjectByIdAsync(int? Id)
        {
            //GET api/ProjectData
            string send_url = url.TrimEnd('/') + $"/api/ProjectData/{Id}";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<Project>(json);
        }

        /// <summary>
        /// редактировать или добавить проект
        /// </summary>
        /// <param name="project">отредактированный/добавляемый проект</param>
        /// <returns></returns>
        public async Task<bool> EditOrCreateProjectAsync(Project project)
        {
            if (project != null)
            {
                project.Image = SkillProfiHelper.GetImageToImageByte(project.ImageFormFile);
                project.ImageFormFile = null;
                string send_url = url.TrimEnd('/') + "/api/ProjectData";
                if (project.Id == 0)
                {
                    //POST api/ProjectData
                    _ = await httpClient.PostAsync(
                                requestUri: send_url,
                                content: new StringContent(JsonConvert.SerializeObject(project), Encoding.UTF8, mediaType: "application/json")
                            );
                }
                else
                {
                    //PUT api/ProjectData
                    _ = await httpClient.PutAsync(
                                requestUri: send_url,
                                content: new StringContent(JsonConvert.SerializeObject(project), Encoding.UTF8, mediaType: "application/json")
                            );
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// удалить проект по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор проекта</param>
        /// <returns></returns>
        public async Task<bool> RemoveProjectByIdAsync(int? Id)
        {
            //DELETE api/ProjectData/Id
            if ((Id ?? 0) > 0)
            {
                string send_url = url.TrimEnd('/') + $"/api/ProjectData/{Id}";
                _ = await httpClient.DeleteAsync(send_url);
                return true;
            }
            return false;
        }
        #endregion

        #region Блог
        /// <summary>
        /// получить все блоги
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Blog>> GetBlogsAsync()
        {
            //GET api/BlogData
            string send_url = url.TrimEnd('/') + "/api/BlogData";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<List<Blog>>(json);
        }

        /// <summary>
        /// получить Блог по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор блога</param>
        /// <returns></returns>
        public async Task<Blog> GetBlogByIdAsync(int? Id)
        {
            //GET api/BlogData
            string send_url = url.TrimEnd('/') + $"/api/BlogData/{Id}";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<Blog>(json);
        }

        /// <summary>
        /// редактировать или добавить блог
        /// </summary>
        /// <param name="blog">отредактированный/добавляемый блог</param>
        /// <returns></returns>
        public async Task<bool> EditOrCreateBlogAsync(Blog blog)
        {
            if (blog != null)
            {
                blog.Image = SkillProfiHelper.GetImageToImageByte(blog.ImageFormFile);
                blog.ImageFormFile = null;
                string send_url = url.TrimEnd('/') + $"/api/BlogData";
                if (blog.Id ==0)
                {

                    //POST api/BlogData
                    _ = await httpClient.PostAsync(
                                requestUri: send_url,
                                content: new StringContent(JsonConvert.SerializeObject(blog), Encoding.UTF8, mediaType: "application/json")
                            );
                }
                else
                {
                    //PUT api/BlogData
                    _ = await httpClient.PutAsync(
                                requestUri: send_url,
                                content: new StringContent(JsonConvert.SerializeObject(blog), Encoding.UTF8, mediaType: "application/json")
                            );
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// удалить блог по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор блога</param>
        /// <returns></returns>
        public async Task<bool> RemoveBlogByIdAsync(int? Id)
        {
            //DELETE api/BlogData/Id
            if ((Id ?? 0) > 0)
            {
                string send_url = url.TrimEnd('/') + $"/api/BlogData/{Id}";
                _ = await httpClient.DeleteAsync(send_url);
                return true;
            }
            return false;
        }
        #endregion

        #region Контакты
        /// <summary>
        /// Получить контакты
        /// </summary>
        /// <returns></returns>
        public async Task<Contact> GetContact()
        {
            //GET api/ContactData
            string send_url = url.TrimEnd('/') + "/api/ContactData";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<Contact>(json);
        }

        /// <summary>
        /// получить контакт по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор контакта</param>
        /// <returns></returns>
        public async Task<Contact> GetContactByIdAsync(int? Id)
        {
            //GET api/ContactData
            string send_url = url.TrimEnd('/') + $"/api/ContactData/{Id}";
            string json = await httpClient.GetStringAsync(send_url);
            return JsonConvert.DeserializeObject<Contact>(json);
        }

        /// <summary>
        /// редактировать контакт
        /// </summary>
        /// <param name="contact">отредактированный контакт</param>
        /// <returns></returns>
        public async Task<bool> EditContactAsync(Contact contact)
        {
            //PUT api/ContactData
            if (contact != null)
            {
                contact.Image = SkillProfiHelper.GetImageToImageByte(contact.ImageFormFile);
                contact.ImageFormFile = null;
                if (contact.Links?.Count > 0)
                {
                    foreach (var link in contact.Links)
                    {
                        link.Image = SkillProfiHelper.GetImageToImageByte(link.ImageFormFile);
                        link.ImageFormFile = null;
                    }

                }
                string send_url = url.TrimEnd('/') + "/api/ContactData";//HttpRequestMessage message
                _ = await httpClient.PutAsync(
                                                requestUri: send_url,
                                                content: new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8, mediaType: "application/json")
                                            );
                return true;
            }
            return false;
        }
        #endregion
    }
}
