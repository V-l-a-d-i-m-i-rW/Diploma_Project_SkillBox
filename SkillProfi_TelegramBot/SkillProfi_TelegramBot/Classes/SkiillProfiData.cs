using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SkillProfi_TelegramBot.Classes
{
    class SkiillProfiData<T>
        where T : class, new()
    {

        public SkiillProfiData(string url, HttpClient client)
        {
            this.url = url;
            httpClient = client;
        }

        #region поля        
        private readonly HttpClient httpClient;
        private readonly string url;
        private List<T> collection;
        public event Action<string> ErrorMessage;
        #endregion

        #region свойства
        /// <summary>
        /// колекция объектов
        /// </summary>
        public List<T> DataCollection { get => collection; }       

        /// <summary>
        /// создаем объект
        /// </summary>
        public bool IsNewObject { get; set; }

        /// <summary>
        /// редактируемый объект
        /// </summary>
        public T EditObject { get; set; }

        #endregion

        #region Методы
        /// <summary>
        /// вывод сообщение об ощибке
        /// </summary>
        /// <param name="errorMessage"></param>
        private void OnErrorMessage(string errorMessage) => ErrorMessage?.Invoke(errorMessage);

        /// <summary>
        /// обновить данные коллекции
        /// </summary>
        public async Task<bool> UpdateDataCollectionAsync()
        {
            try
            {
                collection = await GetDataCollectionAsync();
                return true;
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message??ex.Message);
            }
            return false;
        }

        /// <summary>
        /// отмена создания/Редактирования нового объекта 
        /// </summary>
        public void CancelCreateOrEditObject()
        {
            EditObject = null;
            IsNewObject = false;
        }

        /// <summary>
        /// Установить объект для редактирования
        /// </summary>
        public void SetEditObject(T obj)
        {
            EditObject = obj;
            IsNewObject = false;
        }

        /// <summary>
        /// сохранить изменненный/новый объект
        /// </summary>
        public async Task<bool> SaveObjectAsync()
        {
            try
            {
                if (IsNewObject)
                {
                    await CreateDataAsync(EditObject);
                }
                else
                {
                    await EditDataAsync(EditObject);
                }
                CancelCreateOrEditObject();
                return true;
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            CancelCreateOrEditObject();
            return false;
        }

        #endregion

        #region получение/запись данных по WEBAPI
        /// <summary>
        /// список данных
        /// </summary>
        /// <returns></returns>
        private async Task<List<T>> GetDataCollectionAsync()
        {
            string json = await httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        /// <summary>
        /// Получить данные  для одного объекта по указанному URL
        /// </summary>
        /// <param name="url">URL по которому получаем данные</param>
        /// <returns></returns>
        public async Task<(T, bool)> GetOneObjectDataAsync(string url = "")
        {
            try
            {
                string send_url = string.IsNullOrEmpty(url?.Trim()) ? this.url : url;
                string json = await httpClient.GetStringAsync(send_url);
                return (JsonConvert.DeserializeObject<T>(json), true);
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            return (null, false);
        }

        /// <summary>
        /// Получить строку по указанному URL
        /// </summary>
        /// <param name="url">URL по которому получаем данные</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string url)
        {
            try
            {
                string json = await httpClient.GetStringAsync(url);
                return json;
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            return null;
        }

        /// <summary>
        /// получить данные по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор услуги</param>
        /// <returns></returns>
        public async Task<T> GetDataByIdAsync(int? Id)
        {
            try
            {
                string send_url = url.TrimEnd('/') + $"/{Id}";
                string json = await httpClient.GetStringAsync(send_url);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
                
            }
            return null;
        }

        /// <summary>
        /// удалить данные по идентификатору
        /// </summary>
        /// <param name="Id">идентификатор услуги</param>
        /// <returns></returns>
        public async Task<bool> RemoveDataByIdAsync(int? Id)
        {
            try
            {
                if ((Id ?? 0) > 0)
                {
                    string send_url = url.TrimEnd('/') + $"/{Id}";
                    _ = await httpClient.DeleteAsync(send_url);
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);                
            }
            return false;
        }

        /// <summary>
        /// редактировать объект
        /// </summary>
        /// <param name="data">отредактированный объект</param>
        /// <returns></returns>
        private async Task EditDataAsync(T data)
        {
            if (data != null)
            {
                //PUT api/ServiceData
                _ = await httpClient.PutAsync(
                            requestUri: url,
                            content: new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, mediaType: "application/json")
                        );
            }
        }

        /// <summary>
        /// создать объект
        /// </summary>
        /// <param name="data">новый объект</param>
        /// <returns></returns>
        private async Task CreateDataAsync(T data)
        {
            if (data != null)
            {
                //POST api/ServiceData
                _ = await httpClient.PostAsync(
                            requestUri: url,
                            content: new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, mediaType: "application/json")
                        );
            }
        }

        /// <summary>
        /// записать список данных
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetDataCollectionAsync(IEnumerable<T> dataCollection, string url = "")
        {
            try
            {
                 //(PUT api/HeaderQuote/)
                if (dataCollection != null)
                {
                    string send_url = string.IsNullOrEmpty(url?.Trim()) ? this.url : url; ;//HttpRequestMessage message
                    _ = await httpClient.PutAsync(
                                                    requestUri: send_url,
                                                    content: new StringContent(JsonConvert.SerializeObject(dataCollection), Encoding.UTF8, mediaType: "application/json")
                                                );
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            return false;
        }

        /// <summary>
        /// записать данные
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetUrlStringAsync(string send_url)
        {
            try
            {
                //(PUT api/HeaderQuote/)
                if (!string.IsNullOrWhiteSpace(send_url))
                {
                    _ = await httpClient.PutAsync(
                                                requestUri: send_url,
                                                content: new StringContent("")
                                            );
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            return false;
        }


        /// <summary>
        /// получить данные параметризованного класа
        /// </summary>
        /// <typeparam name="K">класс данные которого требуется получить</typeparam>
        /// <param name="url">адрес по которому получаем данные</param>
        /// <returns></returns>
        public async Task<K> GetParameterizedData<K>(string url = "") where K : class, new()
        {
            K dat = null;
            try
            {
                string send_url = string.IsNullOrEmpty(url?.Trim()) ? this.url : url; ;//HttpRequestMessage message
                string json = await httpClient.GetStringAsync(send_url);
                dat = JsonConvert.DeserializeObject<K>(json);
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            dat ??= new();
            return dat;
        }
        #endregion

    }
}
