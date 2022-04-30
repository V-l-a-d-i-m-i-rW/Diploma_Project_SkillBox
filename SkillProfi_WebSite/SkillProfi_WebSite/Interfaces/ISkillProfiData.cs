using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SkillProfi_Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkillProfi_WebSite.Interfaces
{
    /// <summary>
    /// данные для сайта
    /// </summary>
    public interface ISkillProfiData
    {

        /// <summary>
        /// получение цитат для заголовка
        /// </summary>
        /// <returns></returns>
        public Task<string> GetQuoteInTheHeaderAsync();

        /// <summary>
        /// получить все цитаты для заголовка
        /// </summary>
        /// <returns></returns>
        public Task<List<HeaderDescription>> GetAllQuoteInTheHeaderAsync();

        /// <summary>
        /// записать цитаты для заголовка
        /// </summary>
        /// <returns></returns>
        public Task<bool> SetQuoteInTheHeaderAsync(List<HeaderDescription> headers);

        /// <summary>
        /// записать название меню и заголовков страницы
        /// </summary>
        /// <returns></returns>
        public Task<bool> SetMenuNameAndPageHeaderAsync(MenuNameAndPageHeader menuAndHeader);

        /// <summary>
        /// Получить названия меню
        /// </summary>
        /// <returns></returns>
        public Task<MenuNameAndPageHeader> GetMenuNameAndPageHeaderAsync();

        /// <summary>
        /// Установить общие данные в ViewData
        /// </summary>
        /// <param name="viewData">ViewDataDictionary</param>
        /// <param name="isAdmin">пользователь админитсратор</param>
        /// <returns></returns>
        public Task SetGeneralDataInViewDataDictionaryAsync(ViewDataDictionary viewData, bool isAdmin);

        #region Заявки
        /// <summary>
        /// добавление заявки
        /// </summary>
        /// <param name="newBid">дбавляемая заявка</param>
        /// <returns></returns>
        public Task AddBidAsync(Bid newBid);

        /// <summary>
        /// получить записи за период
        /// </summary>
        /// <param name="from">дата от</param>
        /// <param name="up_to">дата до</param>
        /// <returns></returns>
        public Task<SortedBidsData> GetBidForThePeriodAsync(DateTime from, DateTime up_to);

        /// <summary>
        /// получить запись по идентификатору записи 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<Bid> GetBidByIdAsync(int? Id);

        /// <summary>
        /// изменение статуса заявки
        /// </summary>
        /// <param name="Id">Идентификатор заявки</param>
        /// <param name="SelectedBidStutus">измененный статус</param>
        /// <returns></returns>
        public Task<bool> EditBidStatusAsync(int? Id, BidStatusEnum SelectedBidStutus);

        /// <summary>
        /// Установить данные в ViewData для формы заявки
        /// </summary>
        /// <param name="viewData"></param>
        /// <returns></returns>
        public Task SetBidFormDataInViewDataDictionaryAsync(ViewDataDictionary viewData);

        #region отображение формы заявки
        /// <summary>
        /// получить данные для формы заявок
        /// </summary>
        /// <returns></returns>
        public Task<BidPageData> GetBidPageDataAsync();

        /// <summary>
        /// получить данные для формы заявок
        /// </summary>
        /// <returns></returns>
        public Task<BidPageData> GetBidPageDataAsync(int? id);

        /// <summary>
        /// получить данные для формы заявок
        /// </summary>
        /// <returns></returns>
        public Task<bool> SetBidPageDataAsync(BidPageData data);
        #endregion


        #endregion

        #region Услуги
        /// <summary>
        /// список услуг
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Service>> GetServiсesAsync();

        /// <summary>
        /// получить услугу по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор услуги</param>
        /// <returns></returns>
        public Task<Service> GetServiсeByIdAsync(int? Id);

        /// <summary>
        /// удалить услугу по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор услуги</param>
        /// <returns></returns>
        public Task<bool> RemoveServiceByIdAsync(int? Id);

        /// <summary>
        /// редактировать или добавить услугу
        /// </summary>
        /// <param name="service">отредактированная/добавляемая услуга</param>
        /// <returns></returns>
        public Task<bool> EditOrCreateServiceAsync(Service service);
        #endregion

        #region Проекты
        /// <summary>
        /// получить проект по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор проекта</param>
        /// <returns></returns>
        public Task<Project> GetProjectByIdAsync(int? Id);

        /// <summary>
        /// редактировать или добавить проект
        /// </summary>
        /// <param name="project">отредактированный/добавляемый проект</param>
        /// <returns></returns>
        public Task<bool> EditOrCreateProjectAsync(Project project);

        /// <summary>
        /// удалить проект по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор проекта</param>
        /// <returns></returns>
        public Task<bool> RemoveProjectByIdAsync(int? Id);

        /// <summary>
        /// получить все проекты
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Project>> GetProjectsAsync();
        #endregion

        #region Блог
        /// <summary>
        /// получить Блог по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор блога</param>
        /// <returns></returns>
        public Task<Blog> GetBlogByIdAsync(int? Id);

        /// <summary>
        /// редактировать или добавить блог
        /// </summary>
        /// <param name="blog">отредактированный/добавляемый блог</param>
        /// <returns></returns>
        public Task<bool> EditOrCreateBlogAsync(Blog blog);

        /// <summary>
        /// удалить блог по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор блога</param>
        /// <returns></returns>
        public Task<bool> RemoveBlogByIdAsync(int? Id);

        /// <summary>
        /// получить все блоги
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Blog>> GetBlogsAsync();
        #endregion

        #region Контакты
        /// <summary>
        /// Получить контакты
        /// </summary>
        /// <returns></returns>
        public Task<Contact> GetContact();

        /// <summary>
        /// получить контакт по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор контакта</param>
        /// <returns></returns>
        public Task<Contact> GetContactByIdAsync(int? Id);

        /// <summary>
        /// редактировать контакт
        /// </summary>
        /// <param name="contact">отредактированный контакт</param>
        /// <returns></returns>
        public Task<bool> EditContactAsync(Contact contact);
        #endregion
    }
}
