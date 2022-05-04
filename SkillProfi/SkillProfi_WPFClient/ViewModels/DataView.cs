using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SkillProfi_WPFClient.ViewModels
{
    class DataView<T> : INotifyPropertyChanged
        where T : class, new()
    {

        public DataView(string url, HttpClient client)
        {
            this.url = url;
            httpClient = client;
            collection = new();
        }

        #region поля        
        private readonly HttpClient httpClient;
        private readonly string url;
        private ObservableCollection<T> collection;
        private T selectedItem;
        private bool isNew;
        private T editObject;

        public event Action<string> ErrorMessage;
        public event Action<bool> LoadData;
        #endregion

        #region свойства
        /// <summary>
        /// колекция объектов
        /// </summary>
        public ObservableCollection<T> DataCollection { get => collection; }       
        /// <summary>
        /// выбранный объект
        /// </summary>
        public T SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        /// <summary>
        /// создаем объект
        /// </summary>
        public bool IsNewObject
        {
            get => isNew;
            private set
            {
                isNew = value;
                OnPropertyChanged(nameof(IsNewObject));
            }
        }

        /// <summary>
        /// редактируемый объект
        /// </summary>
        public T EditObject
        {
            get => editObject;
            set
            {
                editObject = value;
                OnPropertyChanged(nameof(EditObject));
            }
        }

        #endregion

        #region Методы
        /// <summary>
        /// вывод сообщение об ощибке
        /// </summary>
        /// <param name="errorMessage"></param>
        private void OnErrorMessage(string errorMessage) => ErrorMessage?.Invoke(errorMessage);

        /// <summary>
        /// признак загрузки данных 
        /// </summary>
        /// <param name="isLoad"></param>
        private void OnLoadData(bool isLoad) => LoadData?.Invoke(isLoad);

        ///// <summary>
        ///// Вызвать обновление объекта в UI 
        ///// </summary>
        //public void RefreshEditObjectProperty()
        //{
        //    OnPropertyChanged(nameof(EditObject));
        //}

        /// <summary>
        /// обновить данные коллекции
        /// </summary>
        public async Task<bool> UpdateDataCollectionAsync()
        {
            try
            {
                OnLoadData(true);
                collection = await GetDataCollectionAsync();
                OnPropertyChanged(nameof(DataCollection));
                OnLoadData(false);
                return true;
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message??ex.Message);
            }
            OnLoadData(false);
            return false;
        }

        /// <summary>
        /// Создать новый объект
        /// </summary>
        public void CreateNewObject()
        {
            EditObject = new();
            IsNewObject = true;
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
                OnLoadData(true);
                if (isNew)
                {
                    await CreateDataAsync(editObject);
                }
                else
                {
                    await EditDataAsync(editObject);
                }
                CancelCreateOrEditObject();
                OnLoadData(false);
                return true;
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            CancelCreateOrEditObject();
            OnLoadData(false);
            return false;
        }

        #endregion

        #region получение/запись данных по WEBAPI
        /// <summary>
        /// список данных
        /// </summary>
        /// <returns></returns>
        private async Task<ObservableCollection<T>> GetDataCollectionAsync()
        {
            string json = await httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<ObservableCollection<T>>(json);
        }

        /// <summary>
        /// Получить данные  для одного объекта по указанному URL
        /// </summary>
        /// <param name="url">URL по которому получаем данные</param>
        /// <returns></returns>
        public async Task<T> GetOneObjectDataAsync(string url = "")
        {
            try
            {
                OnLoadData(true);
                string send_url = string.IsNullOrEmpty(url?.Trim()) ? this.url : url;
                string json = await httpClient.GetStringAsync(send_url);
                OnLoadData(false);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            OnLoadData(false);
            return null;
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
                OnLoadData(true);
                string json = await httpClient.GetStringAsync(url);
                OnLoadData(false);
                return json;
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            OnLoadData(false);
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
                OnLoadData(true);
                string send_url = url.TrimEnd('/') + $"/{Id}";
                string json = await httpClient.GetStringAsync(send_url);
                OnLoadData(false);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
                
            }
            OnLoadData(false);
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
                OnLoadData(false);
                if ((Id ?? 0) > 0)
                {
                    string send_url = url.TrimEnd('/') + $"/{Id}";
                    _ = await httpClient.DeleteAsync(send_url);
                    OnLoadData(false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);                
            }
            OnLoadData(false);
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
                OnLoadData(false);
                //(PUT api/HeaderQuote/)
                if (dataCollection != null)
                {
                    string send_url = string.IsNullOrEmpty(url?.Trim()) ? this.url : url; ;//HttpRequestMessage message
                    _ = await httpClient.PutAsync(
                                                    requestUri: send_url,
                                                    content: new StringContent(JsonConvert.SerializeObject(dataCollection), Encoding.UTF8, mediaType: "application/json")
                                                );
                    OnLoadData(true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            OnLoadData(false);
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
                OnLoadData(false);
                //(PUT api/HeaderQuote/)
                if (!string.IsNullOrWhiteSpace(send_url))
                {
                    _ = await httpClient.PutAsync(
                                                requestUri: send_url,
                                                content: new StringContent("")
                                            );
                    OnLoadData(true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            OnLoadData(false);
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
                OnLoadData(false);
                string send_url = string.IsNullOrEmpty(url?.Trim()) ? this.url : url; ;//HttpRequestMessage message
                string json = await httpClient.GetStringAsync(send_url);
                dat = JsonConvert.DeserializeObject<K>(json);
            }
            catch (Exception ex)
            {
                OnErrorMessage(ex.InnerException?.Message ?? ex.Message);
            }
            OnLoadData(false);
            dat ??= new();
            return dat;
        }
        #endregion

        #region реализация интерфеса INotifyPropertyChanged
        /// <summary>
        /// событие PropertyChangedEventHandler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// метод изменения свойства
        /// </summary>
        /// <param name="propertyName">имя свойства</param>
        public void OnPropertyChanged(string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
