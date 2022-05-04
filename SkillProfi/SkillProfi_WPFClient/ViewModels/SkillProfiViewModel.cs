using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkillProfi_Shared;
using SkillProfi_WPFClient.Classes;
using SkillProfi_WPFClient.Interfaces;
using SkillProfi_WPFClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace SkillProfi_WPFClient.ViewModels
{
    /* Вкладки TabControl
        0 Вкладка Добаввить заявку            
        1 Вкладка Редактировать форму заявки
        2 Вкладка Рабочий стол
        3 Вкладка Рабочий стол редактировать заявку            
        4 Вкладка проекты
        5 Вкладка Добавить/редактировать проект 
        6 Вкладка Услуги
        7 Вкладка Добавить/редактировать услуги
        8 Вкладка Блог 
        9 Вкладка Добавить/редактировать Блог
        10 Контакты
        11 Контакты Редактировать
        12 Авторизация
        13 Регистрация пользователя
        14 Список пользователей
        15 Вкладка Редактирование названий меню
        16 Вкладка Редактирование списка цитат заголовка
     */
    class SkillProfiViewModel : INotifyPropertyChanged
    {
        #region поля
        private string header;
        private readonly DataView<Bid> bidData;
        private readonly string url;
        private readonly HttpClient http_client;
        private string editOrCreateTextButton, editOrCreateTextHeader;
        private SortedBidsData sortedBidData;

        private readonly List<BidStatus> bidStatus;    

        /// <summary>
        /// признак загрузки данных для отображения окна
        /// </summary>
        private bool isLoadData;
        /// <summary>
        /// Объект услуги
        /// </summary>
        private readonly DataView<Service> serviceData;
        /// <summary>
        /// Объект проекты
        /// </summary>
        private readonly DataView<Project> projectData;
        /// <summary>
        /// Объект блог
        /// </summary>
        private readonly DataView<Blog> blogData;

        /// <summary>
        /// Объект заголовки меню и страниц
        /// </summary>
        private readonly DataView<MenuNameAndPageHeader> menuNameAndPageHeaderData;

        /// <summary>
        /// Объект заголовки
        /// </summary>
        private readonly DataView<HeaderDescription> headerDescriptionData;

        /// <summary>
        /// Объект контакты
        /// </summary>
        private readonly DataView<Contact> contactData;

        /// <summary>
        /// данные для отображения формы заявки
        /// </summary>
        private readonly DataView<BidPageData> bidPageData;

        /// <summary>
        /// сервис вывода сообщений
        /// </summary>
        private readonly IDialogService dialogService;


        /// <summary>
        /// индекс выбранной вкладки tabControl 
        /// </summary>
        private int selectedTabItem;

        private ObservableCollection<User> userList;
        private User authorizedUser;
        private User selectedUser;

        private string currentAuthorizationLogin, addUserLogin;
        private string currentAuthorizationPassword, addUserPassword, addUserConfirmPassword;

        /// <summary>
        /// массив фильтров для вывода заявок
        /// </summary>
        private readonly List<BidFilter> bidFilter;

        private const string pathUsers = @"\DB\users.json";
        #endregion

        #region свойсва

        #region авторизация
        private User AuthorizedUser
        {
            get => authorizedUser;
            set
            {
                authorizedUser = value;
                OnPropertyChanged(nameof(IsAuthorized));
                OnPropertyChanged(nameof(AuthorizedName));
            }
        }
        /// <summary>
        /// выбранный пользователь в списке пользователей
        /// </summary>
        public User SelectedUser
        {
            get => selectedUser;
            set
            {
                selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }
        /// <summary>
        /// авторицация пройдена
        /// </summary>
        public bool IsAuthorized { get => AuthorizedUser != null; }

        public string AuthorizedName
        {
            get
            {
                return IsAuthorized ? $"{AuthorizedUser.Login}" : "";
            }
        }
        /// <summary>
        /// список пользователей
        /// </summary>
        public ObservableCollection<User> UserList { get => userList; }

        /// <summary>
        /// Логин пользователя для авторизации
        /// </summary>
        public string CurrentAuthorizationLogin
        {
            get => currentAuthorizationLogin;
            set
            {
                currentAuthorizationLogin = value;
                OnPropertyChanged(nameof(CurrentAuthorizationLogin));
            }
        }

        /// <summary>
        /// Пароль пользователя для авторизации
        /// </summary>
        public string CurrentAuthorizationPassword
        {
            get => currentAuthorizationPassword;
            set
            {
                currentAuthorizationPassword = value;
                OnPropertyChanged(nameof(CurrentAuthorizationPassword));
            }
        }

        /// <summary>
        /// Логин добавляемого пользователя
        /// </summary>
        public string AddUserLogin
        {
            get => addUserLogin;
            set
            {
                addUserLogin = value;
                OnPropertyChanged(nameof(AddUserLogin));
            }
        }

        /// <summary>
        /// пароль добавляемого пользователя
        /// </summary>
        public string AddUserPassword
        {
            get => addUserPassword;
            set
            {
                addUserPassword = value;
                OnPropertyChanged(nameof(AddUserPassword));
            }
        }

        /// <summary>
        /// подтверждение пароля добавляемого пользователя
        /// </summary>
        public string AddUserConfirmPassword
        {
            get => addUserConfirmPassword;
            set
            {
                addUserConfirmPassword = value;
                OnPropertyChanged(nameof(AddUserConfirmPassword));
            }
        }
        #endregion

        public List<BidStatus> StatusBid { get => bidStatus; }
        /// <summary>
        /// заявка на отправку
        /// </summary>
        public DataView<Bid> BidData{ get => bidData;}

        /// <summary>
        /// отсортированные заявки с общим количеством заявок в БД
        /// </summary>
        public SortedBidsData BidDataSorted{ get => sortedBidData; }
        /// <summary>
        /// признак загрузки данных для отображения окна
        /// </summary>
        public bool IsLoadData
        {
            get => isLoadData;
            set 
            {
                isLoadData = value;
                OnPropertyChanged(nameof(IsLoadData));
            }
        }

        /// <summary>
        /// Услуги
        /// </summary>
        public DataView<Service> ServiceData { get => serviceData; }

        /// <summary>
        /// Проекты
        /// </summary>
        public DataView<Project> ProjectData { get => projectData; }

        /// <summary>
        /// Блог
        /// </summary>
        public DataView<Blog> BlogData { get => blogData; }

        /// <summary>
        /// Объект заголовки меню и страниц
        /// </summary>
        public DataView<MenuNameAndPageHeader> MenuNameAndPageHeaderData { get => menuNameAndPageHeaderData; }

        /// <summary>
        /// Объект заголовки "Крылатые фразы"
        /// </summary>
        public DataView<HeaderDescription> HeaderDescriptionData { get => headerDescriptionData; }

        /// <summary>
        /// Объект контакты
        /// </summary>
        public DataView<Contact> ContactData { get => contactData; }

        /// <summary>
        /// данные для отображения формы заявки
        /// </summary>
        public DataView<BidPageData> BidPageData { get => bidPageData; }

        /// <summary>
        /// индекс выбранной вкладки
        /// </summary>
        public int SelectedTabItem
        {
            get => selectedTabItem;
            set
            {
                selectedTabItem = value < 0 ? 0 : value;
                OnPropertyChanged(nameof(SelectedTabItem));
            }
        }
        /// <summary>
        /// крылатая фраза заголовка
        /// </summary>
        public string QuoteInTheHeader
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged(nameof(QuoteInTheHeader));
            }
        }
        /// <summary>
        /// название кнопки при редактировании/создании
        /// </summary>
        public string EditOrCreateTextButton
        {
            get => editOrCreateTextButton;
            private set
            {
                editOrCreateTextButton = value;
                OnPropertyChanged(nameof(EditOrCreateTextButton));
            }
        }

        /// <summary>
        /// название заголовка при редактировании/создании
        /// </summary>
        public string EditOrCreateTextHeader
        {
            get => editOrCreateTextHeader;
            private set
            {
                editOrCreateTextHeader = value;
                OnPropertyChanged(nameof(EditOrCreateTextHeader));
            }
        }

        /// <summary>
        /// фильтр для вывода заявок
        /// </summary>
        public List<BidFilter> FilterBid { get => bidFilter; }
        #endregion

        #region Методы
        /// <summary>
        /// выаод диалогового окна с ошибкой
        /// </summary>
        /// <param name="errorText">текстовое сообшение ошибки</param>
        private void ShowErrorMessage(string errorText) => dialogService?.ShowErrorMessage(errorText);

        /// <summary>
        /// выаод диалогового окна с вопросом
        /// </summary>
        /// <param name="errorText">текстовое сообшение ошибки</param>
        private bool ShowQuestionMessage(string message) => dialogService?.SwowQuestionMessage(message) ?? false;

        /// <summary>
        /// выаод диалогового окна с выбором файла
        /// </summary>

        private bool ShowOpenDialog() => dialogService?.OpenFileDialog("Файлы изображений | *.jpg;*.jpeg;*.png") ?? false;

        /// <summary>
        /// загрузка данных (для отображения окна пользователю)
        /// </summary>
        /// <param name="isLoad"></param>
        private void OnLoadData(bool isLoad)
        {
            App.Current?.Dispatcher?.Invoke(() => { IsLoadData = isLoad; });
        }


        /// <summary>
        /// выаод диалогового окна с ообщением
        /// </summary>
        /// <param name="text">текстовое сообшение</param>
        private void ShowMessage(string text) => dialogService?.ShowMessage(text);

        /// <summary>
        /// выаод диалогового окна с вопросом
        /// </summary>
        /// <param name="text">текстовое сообшение</param>
        private bool ShowDialogMessage(string text) => dialogService?.SwowQuestionMessage(text) ?? true;

        /// <summary>
        /// загрузить данные пользователей для авторизации
        /// </summary>
        private void LoadUserData()
        {
            string filePath = SkillProfiHelper.GetLocalFilePath(pathUsers);
            if (File.Exists(filePath))
            {
                Newtonsoft.Json.JsonSerializerSettings jsettings = new ()
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                };
                JsonSerializer jsonSerializer = JsonSerializer.Create(jsettings);
                using TextReader text_reader = File.OpenText(filePath);

                userList = (ObservableCollection<User>)jsonSerializer.Deserialize(text_reader, typeof(ObservableCollection<User>));
            }
            userList ??= new ObservableCollection<User>();
            if (userList.Count == 0)
            {
                userList.Add(new User() { Login = "admin", Password = SkillProfiHelper.CalculateMD5Checksum("!Aa123")});
                SaveUserData();
            }
        }
        /// <summary>
        /// сохранение данных пользователя 
        /// </summary>
        private void SaveUserData()
        {
            if (userList?.Count > 0)
            {
                string filePath = SkillProfiHelper.GetLocalFilePath(pathUsers);
                JsonSerializerSettings jsettings = new ()
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    // , ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                JsonSerializer jsonSerializer = JsonSerializer.Create(jsettings);
                using TextWriter text_writer = File.CreateText(filePath);
                jsonSerializer.Serialize(text_writer, userList);
            }
        }

        /// <summary>
        /// получить данные названий меню и страниц
        /// </summary>
        /// <returns></returns>
        private async Task GetMenuName()
        {
            menuNameAndPageHeaderData.SetEditObject(await menuNameAndPageHeaderData?.GetOneObjectDataAsync());
            QuoteInTheHeader = await headerDescriptionData.GetStringAsync(url.TrimEnd('/') + "/api/HeaderQuote/1");
        }

        /// <summary>
        /// Показать страницу услуги
        /// </summary>
        /// <returns></returns>
        private async Task ShowServeces()
        {
            await GetMenuName ();
            if (await serviceData?.UpdateDataCollectionAsync())
            {
                SelectedTabItem = 6;
            }
        }

        /// <summary>
        /// Показать страницу Блог
        /// </summary>
        /// <returns></returns>
        private async Task ShowBlogs()
        {
            await GetMenuName();
            if (await blogData?.UpdateDataCollectionAsync())
            {
                SelectedTabItem = 8;
            }
        }

        /// <summary>
        /// Показать страницу Проекты
        /// </summary>
        /// <returns></returns>
        private async Task ShowProjects()
        {
            await GetMenuName ();
            if (await projectData?.UpdateDataCollectionAsync())
            {
                SelectedTabItem = 4;
            }
        }

        /// <summary>
        /// Показать страницу Проекты
        /// </summary>
        /// <returns></returns>
        private async Task ShowContact()
        {
            await GetMenuName();
            contactData.SetEditObject(await contactData?.GetOneObjectDataAsync());
            SelectedTabItem = 10;
        }

        /// <summary>
        /// получить данные для отображения формы заявки
        /// </summary>
        /// <returns></returns>
        private async void GetBidPageData()
        {
            await GetMenuName();
            bidPageData.SetEditObject(await bidPageData?.GetOneObjectDataAsync());
            bidData.CreateNewObject();
            SelectedTabItem = 0;
        }

        /// <summary>
        /// сброс данных фильтра
        /// </summary>
        private void RefreshBidFilterData()
        {
            foreach (BidFilter filter in bidFilter)
            {
                filter.IsChecked = false;
                switch (filter.Name)
                {
                    case "today":
                        filter.DateFrom = DateTime.Now;
                        filter.DateUpTo = DateTime.Now;
                        break;
                    case "yesterday":
                        filter.DateFrom = DateTime.Now.AddDays(-1);
                        filter.DateUpTo = DateTime.Now.AddDays(-1);
                        break;
                    case "week":
                        filter.DateFrom = DateTime.Now.AddDays(-7);
                        filter.DateUpTo = DateTime.Now;
                        break;
                    case "month":
                        filter.DateFrom = DateTime.Now.AddMonths(-1);
                        filter.DateUpTo =  DateTime.Now;
                        break;
                    case "period":
                        filter.DateFrom = DateTime.Now;
                        filter.DateUpTo = DateTime.Now;
                        break;
                }
            }
        }
        /// <summary>
        /// получить данные о заявках
        /// </summary>
        /// <param name="usedFilter"></param>
        /// <returns></returns>
        private async Task GetSortedBidData(bool usedFilter = false)
        {
            DateTime from = new(0), up_to = new(0);
            if (usedFilter && bidFilter?.Count > 0)
            {
                foreach (BidFilter filter in bidFilter)
                {
                    if (filter.IsChecked)
                    {
                        from = filter.DateFrom;
                        up_to = filter.DateUpTo;
                        break;
                    }
                }
            }
            sortedBidData = await bidData?.GetParameterizedData<SortedBidsData>(url.TrimEnd('/') + $"/api/BidData/from={from:yyyy-MM-dd}&up_to={up_to:yyyy-MM-dd}");
            OnPropertyChanged(nameof(BidDataSorted));
        }
        #endregion

        #region команды для связи с View

        private RelayCommand selectImageCommand;

        #region Авторизация
        private RelayCommand authorizationCommand;
        private RelayCommand cancelAuthorizationCommand;
        private RelayCommand exitAuthorizationCommand;
        private RelayCommand showAuthorizationCommand;
        private RelayCommand showAddUserCommand;
        private RelayCommand showUserSettingsCommand;
        private RelayCommand addUserCommand;
        private RelayCommand cancelUserCommand;
        private RelayCommand removeUserCommand;
        private RelayCommand saveUserCommand;
        #endregion

        #region Услуги
        private RelayCommand showServicesCommand;
        private RelayCommand editServiceCommand;
        private RelayCommand removeServiceCommand;
        private RelayCommand createServiceCommand;
        private RelayCommand saveChangeServiceCommand;
        #endregion

        #region Проекты
        private RelayCommand showProjectsCommand;
        private RelayCommand editProjectCommand;
        private RelayCommand removeProjectCommand;
        private RelayCommand createProjectCommand;
        private RelayCommand saveChangeProjectCommand;
        #endregion

        #region Блог
        private RelayCommand showBlogsCommand;
        private RelayCommand editBlogCommand;
        private RelayCommand removeBlogCommand;
        private RelayCommand createBlogCommand;
        private RelayCommand saveChangeBlogCommand;
        #endregion

        #region Контакты
        private RelayCommand showContactCommand;
        private RelayCommand startLinkCommand;
        private RelayCommand editContactCommand;
        private RelayCommand addLinkCommand;
        private RelayCommand removeLinkCommand;
        private RelayCommand saveChangeContactCommand;
        #endregion

        #region Названия меню и заголовки страниц
        private RelayCommand editMenuNameAndPageHeaderDataCommand;
        private RelayCommand saveMenuNameAndPageHeaderDataCommand;
        #endregion

        #region Цитаты Заголовка
        private RelayCommand editHeaderDescriptionCommand;
        private RelayCommand addHeaderDescriptionCommand;
        private RelayCommand removeHeaderDescriptionCommand;
        private RelayCommand saveChangeHeaderDescriptionCommand;
        #endregion

        #region Форма заявки
        private RelayCommand showBidPageCommand;
        private RelayCommand editBidPageCommand;
        private RelayCommand saveChangeBidPageCommand;
        private RelayCommand sendBidCommand;
        private RelayCommand showDesctopCommand;
        private RelayCommand refreshDesctopDataCommand;
        private RelayCommand editBidStatusCommand;
        private RelayCommand saveBidStatusCommand;
        #endregion

        #endregion

        #region Реализация команд для связи с View

        #region Авторизация и добавление пользователя
        /// <summary>
        /// авторизация
        /// </summary>
        public RelayCommand AuthorizationCommand
        {
            get
            {
                return authorizationCommand ??= new RelayCommand(obj =>
                {
                    User user = userList?.FirstOrDefault(i => i.Login?.Equals(CurrentAuthorizationLogin?.Trim(), StringComparison.OrdinalIgnoreCase)??false);
                    if (user != null && user.Password.Equals(SkillProfiHelper.CalculateMD5Checksum(CurrentAuthorizationPassword)))
                    {
                        AuthorizedUser = user;
                        CurrentAuthorizationPassword = string.Empty;
                        CurrentAuthorizationLogin = string.Empty;
                        GetBidPageData();
                        ShowMessage($"В системе авторизовался пользователь \"{user.Login}\"");
                    }
                    else
                    {
                        AuthorizedUser = null;
                        CurrentAuthorizationPassword = string.Empty;
                        ShowErrorMessage("Не верный логин/пароль");
                    }

                },
                    (obj) => !IsAuthorized
                             && !string.IsNullOrWhiteSpace(CurrentAuthorizationLogin)
                             && !string.IsNullOrWhiteSpace(CurrentAuthorizationPassword)
                 );
            }
        }

        /// <summary>
        /// выход из авторизации
        /// </summary>
        public RelayCommand ExitAuthorizationCommand
        {
            get
            {
                return exitAuthorizationCommand ??= new RelayCommand(obj =>
                {
                    //if (SelectedTabItem == 14 && (authorizedUser!= null))
                    //    SaveUserData();
                    AuthorizedUser = null;
                    CurrentAuthorizationPassword = string.Empty;
                    CurrentAuthorizationLogin = string.Empty;
                    GetBidPageData();
                },
                    (obj) => IsAuthorized
                 );
            }
        }

        /// <summary>
        /// показать форму авторизации
        /// </summary>
        public RelayCommand ShowAuthorizationCommand
        {
            get
            {
                return showAuthorizationCommand ??= new RelayCommand(obj =>
                {
                    SelectedTabItem = 12;
                },
                    (obj) => !IsAuthorized
                 );
            }
        }


        /// <summary>
        /// добавить пользователя показать форму
        /// </summary>
        public RelayCommand ShowAddUserCommand
        {
            get
            {
                return showAddUserCommand ??= new RelayCommand(obj =>
                {
                    SelectedTabItem = 13;
                },
                    (obj) => IsAuthorized && SelectedTabItem == 14 && (AuthorizedUser != null)
                 );
            }
        }

        /// <summary>
        /// список пользователей
        /// </summary>
        public RelayCommand ShowUserSettingsCommand
        {
            get
            {
                return showUserSettingsCommand ??= new RelayCommand(obj =>
                {
                    SelectedTabItem = 14;
                },
                    (obj) => IsAuthorized
                 );
            }
        }
        /// <summary>
        /// добавить пользователя в коллекцию
        /// </summary>
        public RelayCommand AddUserCommand
        {
            get
            {
                return addUserCommand ??= new RelayCommand(obj =>
                {
                    try
                    {
                        if (userList?.FirstOrDefault(i => i.Login.Equals(AddUserLogin.Trim(), StringComparison.OrdinalIgnoreCase)) != null)
                            throw new ArgumentException($"Пользователь с логином \"{AddUserLogin}\" уже зарегистрирован");
                        if (AddUserLogin.Contains(" "))
                            throw new ArgumentException("Логин не должен содержать символов пробел");
                        if (!SkillProfiHelper.CheckStringByRegexPattern(AddUserPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9])\S{6,30}$"))
                            throw new ArgumentException("Пароль не соответсвет требованию безопасости!\n" +
                                "Пароль должен содержать минимум 1 латинскую букву в нижнем регистре\n" +
                                "Пароль должен содержать минимум 1 латинскую букву в верхнем регистре\n" +
                                "Пароль должен содержать минимум 1 цифру\n" +
                                "Пароль должен содержать минимум 1 знак отличный от буквы и цифры\n" +
                                "Пароль должен содержать минимум 6 символов"
                                );
                        userList.Add(new User() { Login = AddUserLogin.Trim(), Password = SkillProfiHelper.CalculateMD5Checksum(AddUserPassword)});
                        SaveUserData();
                        AddUserLogin = string.Empty;
                        AddUserPassword = string.Empty;
                        AddUserConfirmPassword = string.Empty;
                        SelectedTabItem = 14;
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                },
                    (obj) => IsAuthorized && SelectedTabItem == 13 &&
                            (AuthorizedUser != null) &&
                            !string.IsNullOrEmpty(AddUserLogin?.Trim()) &&
                            !string.IsNullOrEmpty(AddUserPassword) &&
                            !string.IsNullOrEmpty(AddUserConfirmPassword) &&
                            AddUserPassword.Equals(AddUserConfirmPassword)
                 );
            }
        }

        /// <summary>
        /// удалить пользователя из коллекции
        /// </summary>
        public RelayCommand RemoveUserCommand
        {
            get
            {
                return removeUserCommand ??= new RelayCommand(obj =>
                {
                    if (SelectedUser != null)
                    {
                        if (ShowDialogMessage($"Удалить пользователя\"{SelectedUser.Login}\""))
                        {
                            userList.Remove(SelectedUser);
                            SaveUserData();
                        }
                    }
                },
                    (obj) => IsAuthorized && SelectedTabItem == 14 &&
                            (AuthorizedUser!= null) && SelectedUser != null
                 );
            }
        }

        /// <summary>
        /// сохранить коллекцию пльзователей
        /// </summary>
        public RelayCommand SaveUserCommand
        {
            get
            {
                return saveUserCommand ??= new RelayCommand(obj =>
                {
                    SaveUserData();
                    GetBidPageData();
                },
                    (obj) => IsAuthorized && SelectedTabItem == 14 && (AuthorizedUser!=null)
                 );
            }
        }
        /// <summary>
        /// отмена авторизации пользователя
        /// </summary>
        public RelayCommand CancelAuthorizationCommand
        {
            get
            {
                return cancelAuthorizationCommand ??= new RelayCommand(obj =>
                {
                    AuthorizedUser = null;
                    CurrentAuthorizationPassword = string.Empty;
                    CurrentAuthorizationLogin = string.Empty;
                    GetBidPageData();
                }
                ,(obj) => SelectedTabItem == 12
                 );
            }
        }
        /// <summary>
        /// отмена действий пользователя
        /// </summary>
        public RelayCommand CancelUserCommand
        {
            get
            {
                return cancelUserCommand ??= new RelayCommand(obj =>
                {
                    AddUserLogin = string.Empty;
                    AddUserPassword = string.Empty;
                    AddUserConfirmPassword = string.Empty;
                    SelectedTabItem = 14;
                }
                , (obj) => IsAuthorized && SelectedTabItem == 13 && (AuthorizedUser!= null)
                 );
            }
        }
        #endregion

        /// <summary>
        /// Показать вкладку Проекты
        /// </summary>
        public RelayCommand SelectImageCommand
        {
            get
            {
                return selectImageCommand ??= new RelayCommand(obj =>
                {
                    if (obj != null && ShowOpenDialog())
                    {
                        Type type = obj.GetType();
                        PropertyInfo property_image = type.GetProperty("Image");
                        if (property_image != null)
                        {
                            byte[] imgdata = File.ReadAllBytes(dialogService.FilePath);
                            //byte[] imgdata = System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath(path));
                            property_image.SetValue(obj, imgdata);

                            //if (type == typeof(Project))
                            //    projectData.RefreshEditObjectProperty();
                            //else if (type == typeof(Blog))
                            //    blogData.RefreshEditObjectProperty();
                            //else if (type == typeof(Contact) || typeof(ContactLink) == type)
                            //    contactData.RefreshEditObjectProperty();
                        }
                    }
                }
                 , (obj) => ProjectData != null
                 );
            }
        }

        #region Услуги
        /// <summary>
        /// Показать вкладку услуги
        /// </summary>
        public RelayCommand ShowServecesCommand
        {
            get
            {
                return showServicesCommand ??= new RelayCommand(async obj =>
                {
                    await ShowServeces();
                }
                ,(obj) => serviceData != null
                 );
            }
        }

        /// <summary>
        /// Редактировать услугу
        /// </summary>
        public RelayCommand EditServiceCommand
        {
            get
            {
                return editServiceCommand ??= new RelayCommand(async obj =>
                {
                    if (obj != null)
                    {
                        Service tmp = await serviceData.GetDataByIdAsync(obj as int?);
                        if (tmp != null)
                        {
                            serviceData.SetEditObject(tmp);
                            EditOrCreateTextButton = "Сохранить";
                            SelectedTabItem = 7;
                            return;
                        }                        
                    }
                    ShowErrorMessage($"Объект не найден!!!");
                    await ShowServeces();
                }
                , (obj) => IsAuthorized && SelectedTabItem == 6
                 );
            }
        }

        /// <summary>
        /// Удалить услугу
        /// </summary>
        public RelayCommand RemoveServiceCommand
        {
            get
            {
                return removeServiceCommand ??= new RelayCommand(async obj =>
                {
                    if (obj != null && obj is Service)
                    {
                        if(ShowQuestionMessage($"Вы действительно хотите удалить услугу \"{(obj as Service)?.Header}\""))
                            await serviceData.RemoveDataByIdAsync((obj as Service)?.Id);
                    }
                    else
                        ShowErrorMessage($"Объект не найден!!!");
                    await ShowServeces();
                }
                , (obj) => IsAuthorized && SelectedTabItem == 6
                 );
            }
        }

        /// <summary>
        /// Добавить услугу
        /// </summary>
        public RelayCommand СreateServiceCommand
        {
            get
            {
                return createServiceCommand ??= new RelayCommand(obj =>
                {
                    EditOrCreateTextButton = "Добавить";
                    serviceData.CreateNewObject();
                    SelectedTabItem = 7;
                }
                , (obj) => IsAuthorized && SelectedTabItem == 6
                 );
            }
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public RelayCommand SaveChangeServiceCommand
        {
            get
            {
                return saveChangeServiceCommand ??= new RelayCommand(async obj =>
                {
                    await serviceData.SaveObjectAsync();
                    await ShowServeces();
                }
                , (obj) => IsAuthorized && SelectedTabItem == 7 &&
                          !string.IsNullOrWhiteSpace(serviceData?.EditObject?.Header) && 
                          !string.IsNullOrWhiteSpace(serviceData?.EditObject?.Description) 
                 );
            }
        }
        #endregion

        #region Проекты
        /// <summary>
        /// Показать вкладку Проекты
        /// </summary>
        public RelayCommand ShowProjectsCommand
        {
            get
            {
                return showProjectsCommand ??= new RelayCommand(async obj =>
                {
                    await ShowProjects();
                }
                 ,(obj) => ProjectData!=null
                 );
            }
        }

        /// <summary>
        /// Редактировать проект
        /// </summary>
        public RelayCommand EditProjectCommand
        {
            get
            {
                return editProjectCommand ??= new RelayCommand(async obj =>
                {
                    if (obj != null)
                    {
                        Project tmp = await projectData.GetDataByIdAsync(obj as int?);
                        if (tmp != null)
                        {
                            projectData.SetEditObject(tmp);
                            EditOrCreateTextButton = "Сохранить";
                            SelectedTabItem = 5;
                            return;
                        }
                    }
                    ShowErrorMessage($"Объект не найден!!!");
                    await ShowProjects();
                }
                , (obj) => IsAuthorized && SelectedTabItem == 4
                 );
            }
        }

        /// <summary>
        /// Удалить проект
        /// </summary>
        public RelayCommand RemoveProjectCommand
        {
            get
            {
                return removeProjectCommand ??= new RelayCommand(async obj =>
                {
                    if (obj != null && obj is Project)
                    {
                        if (ShowQuestionMessage($"Вы действительно хотите удалить проект \"{(obj as Project)?.Header}\""))
                            await projectData.RemoveDataByIdAsync((obj as Project)?.Id);
                    }
                    else
                        ShowErrorMessage($"Объект не найден!!!");
                    await ShowProjects();
                }
                , (obj) => IsAuthorized && SelectedTabItem == 4
                 );
            }
        }

        /// <summary>
        /// Добавить проект
        /// </summary>
        public RelayCommand СreateProjectCommand
        {
            get
            {
                return createProjectCommand ??= new RelayCommand(obj =>
                {
                    EditOrCreateTextButton = "Добавить";
                    projectData.CreateNewObject();
                    SelectedTabItem = 5;
                }
                , (obj) => IsAuthorized && SelectedTabItem == 4
                 );
            }
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public RelayCommand SaveChangesProjectCommand
        {
            get
            {
                return saveChangeProjectCommand ??= new RelayCommand(async obj =>
                {
                    await projectData.SaveObjectAsync();
                    await ShowProjects();
                }
                , (obj) => IsAuthorized && SelectedTabItem == 5 
                           && !string.IsNullOrWhiteSpace(projectData?.EditObject?.Header)
                           && !string.IsNullOrWhiteSpace(projectData?.EditObject?.Description)
                           && projectData?.EditObject?.Image?.Length >0
                 );
            }
        }
        #endregion

        #region Блог
        /// <summary>
        /// Показать вкладку Проекты
        /// </summary>
        public RelayCommand ShowBlogsCommand
        {
            get
            {
                return showBlogsCommand ??= new RelayCommand(async obj =>
                {
                    await ShowBlogs();
                }
                    ,(obj) => BlogData!=null
                 );
            }
        }
        /// <summary>
        /// Редактировать проект
        /// </summary>
        public RelayCommand EditBlogCommand
        {
            get
            {
                return editBlogCommand ??= new RelayCommand(async obj =>
                {
                    if (obj != null)
                    {
                        Blog tmp = await blogData.GetDataByIdAsync(obj as int?);
                        if (tmp != null)
                        {
                            blogData.SetEditObject(tmp);
                            EditOrCreateTextButton = "Сохранить";
                            SelectedTabItem = 9;
                            return;
                        }
                    }
                    ShowErrorMessage($"Объект не найден!!!");
                    await ShowBlogs();
                }
                , (obj) => blogData != null && IsAuthorized && SelectedTabItem == 8
                 );
            }
        }

        /// <summary>
        /// Удалить проект
        /// </summary>
        public RelayCommand RemoveBlogCommand
        {
            get
            {
                return removeBlogCommand ??= new RelayCommand(async obj =>
                {
                    if (obj != null && obj is Blog)
                    {
                        if (ShowQuestionMessage($"Вы действительно хотите удалить блог \"{(obj as Blog)?.Header}\""))
                            await blogData.RemoveDataByIdAsync((obj as Blog)?.Id);
                    }
                    else
                        ShowErrorMessage($"Объект не найден!!!");
                    await ShowBlogs();
                }
                , (obj) => blogData != null && IsAuthorized && SelectedTabItem == 8
                 );
            }
        }

        /// <summary>
        /// Добавить проект
        /// </summary>
        public RelayCommand СreateBlogCommand
        {
            get
            {
                return createBlogCommand ??= new RelayCommand(obj =>
                {
                    EditOrCreateTextButton = "Добавить";
                    blogData.CreateNewObject();
                    SelectedTabItem = 9;
                }
                , (obj) => blogData != null && IsAuthorized && SelectedTabItem == 8
                 );
            }
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public RelayCommand SaveChangeBlogCommand
        {
            get
            {
                return saveChangeBlogCommand ??= new RelayCommand(async obj =>
                {
                    await blogData.SaveObjectAsync();
                    await ShowBlogs();
                }
                , (obj) => blogData != null && IsAuthorized && SelectedTabItem == 9 && blogData?.EditObject != null
                           && !string.IsNullOrWhiteSpace(blogData?.EditObject?.Header)
                           && !string.IsNullOrWhiteSpace(blogData?.EditObject?.Description)
                           && blogData?.EditObject?.Image?.Length > 0
                           && blogData?.EditObject?.Date != null
                 );;
            }
        }
        #endregion

        #region Контакты
        /// <summary>
        /// Показать вкладку Проекты
        /// </summary>
        public RelayCommand ShowContactCommand
        {
            get
            {
                return showContactCommand ??= new RelayCommand(async obj =>
                {
                    await ShowContact();
                }
                    ,(obj) => contactData!=null
                 );
            }
        }    

        /// <summary>
        /// Запуск гиперссылок из иконок контактов
        /// </summary>
        public RelayCommand StartLinkCommand
        {
            get
            {
                return startLinkCommand ??= new RelayCommand(obj =>
                {
                    if (obj != null)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(obj as string) { UseShellExecute = true });
                        }
                        catch //(Exception ex)
                        {
                            //ShowErrorMessage(ex.InnerException?.Message ?? ex.Message);
                        }
                    }
                }
                    ,(obj) => contactData!=null
                 );
            }
        }

        /// <summary>
        /// Редактировать страницу Контактов
        /// </summary>
        public RelayCommand EditContactCommand
        {
            get
            {
                return editContactCommand ??= new RelayCommand(obj =>
                {
                    SelectedTabItem = 11;
                }
                    ,(obj) => contactData!=null && IsAuthorized && SelectedTabItem == 10
                 );
            }
        }

        /// <summary>
        /// добавить ссылку в контакты
        /// </summary>
        public RelayCommand AddContactLinkCommand
        {
            get
            {
                return addLinkCommand ??= new RelayCommand(obj =>
                {
                    contactData.EditObject.Links.Add(new ContactLink());
                }
                    , (obj) => contactData?.EditObject?.Links != null && IsAuthorized && SelectedTabItem == 11
                 );
            }
        }

        /// <summary>
        /// удалить ссылку из контактов
        /// </summary>
        public RelayCommand RemoveContactLinkCommand
        {
            get
            {
                return removeLinkCommand ??= new RelayCommand(obj =>
                {
                    if (obj!= null && obj is ContactLink)
                        contactData.EditObject.Links.Remove(obj as ContactLink);
                }
                    , (obj) => contactData?.EditObject?.Links?.Count>0 && IsAuthorized && SelectedTabItem == 11
                 );
            }
        }

        /// <summary>
        /// Сохранить контакты
        /// </summary>
        public RelayCommand SaveChangeContactCommand
        {
            get
            {
                return saveChangeContactCommand ??= new RelayCommand(async obj =>
                {
                    await contactData.SaveObjectAsync();
                    await ShowContact();
                }
                , (obj) => IsAuthorized && SelectedTabItem == 11 && contactData?.EditObject != null
                 ); ;
            }
        }
        #endregion       

        #region Названия меню и заголовки страниц
        /// <summary>
        /// Показать вкладку Редактирование Названия меню и заголовки страниц
        /// </summary>
        public RelayCommand EditMenuNameAndPageHeaderDataCommand
        {
            get
            {
                return editMenuNameAndPageHeaderDataCommand ??= new RelayCommand(async obj =>
                {
                    await GetMenuName();
                    SelectedTabItem = 15;
                }
                    ,(obj) => menuNameAndPageHeaderData != null && IsAuthorized
                 );
            }
        }    


        /// <summary>
        /// Сохранить контакты
        /// </summary>
        public RelayCommand SaveChangeMenuNameAndPageHeaderDataCommand
        {
            get
            {
                return saveMenuNameAndPageHeaderDataCommand ??= new RelayCommand(async obj =>
                {
                    await menuNameAndPageHeaderData.SaveObjectAsync();
                    GetBidPageData();
                }
                , (obj) => IsAuthorized && SelectedTabItem == 15 && menuNameAndPageHeaderData?.EditObject != null
                 ); ;
            }
        }
        #endregion

        #region Цитаты Заголовка
        /// <summary>
        /// Редактировать список цитат заголовка
        /// </summary>
        public RelayCommand EditHeaderDescriptionCommand
        {
            get
            {
                return editHeaderDescriptionCommand ??= new RelayCommand(async obj =>
                {
                    if (await headerDescriptionData.UpdateDataCollectionAsync())
                    {
                        SelectedTabItem = 16;
                    }                    
                }
                    ,(obj) => headerDescriptionData!=null && IsAuthorized 
                 );
            }
        }

        /// <summary>
        /// добавить цитату
        /// </summary>
        public RelayCommand AddHeaderDescriptionCommand
        {
            get
            {
                return addHeaderDescriptionCommand ??= new RelayCommand(obj =>
                {
                    headerDescriptionData.DataCollection.Add(new HeaderDescription());
                }
                    , (obj) => headerDescriptionData?.DataCollection != null && IsAuthorized && SelectedTabItem == 16
                 );
            }
        }

        /// <summary>
        /// удалить цитату
        /// </summary>
        public RelayCommand RemoveHeaderDescriptionCommand
        {
            get
            {
                return removeHeaderDescriptionCommand??= new RelayCommand(obj =>
                {
                    if (obj!= null && obj is HeaderDescription)
                        headerDescriptionData.DataCollection.Remove(obj as HeaderDescription);
                }
                    , (obj) => headerDescriptionData?.DataCollection?.Count>0 && IsAuthorized && SelectedTabItem == 16
                 );
            }
        }

        /// <summary>
        /// Сохранить контакты
        /// </summary>
        public RelayCommand SaveHeaderDescriptionCommand
        {
            get
            {
                return saveChangeHeaderDescriptionCommand ??= new RelayCommand(async obj =>
                {
                    //сохранение списка Цитат
                    if (await headerDescriptionData.SetDataCollectionAsync(headerDescriptionData.DataCollection))
                    {
                        GetBidPageData();
                    }
                }
                , (obj) => IsAuthorized && SelectedTabItem == 16 && headerDescriptionData?.DataCollection != null
                 ); ;
            }
        }
        #endregion

        #region Страница заявок
        /// <summary>
        /// Сохранить отредактированный статус заявки
        /// </summary>
        public RelayCommand SaveBidStatusCommand
        {
            get
            {
                return saveBidStatusCommand ??= new RelayCommand(async obj =>
                {
                    if (obj != null)
                    {
                        int id = (int) obj;
                        BidStatus status = bidStatus.FirstOrDefault(i => i.IsChecked);

                        if (status != null)
                        {
                            if (await bidData?.SetUrlStringAsync(url.TrimEnd('/') + $"/api/BidData/id={id}&SelectedBidStutus={status.Status}"))
                            {
                                await GetSortedBidData(true);
                                SelectedTabItem = 2;
                            }
                        }
                    }
                }
                , (obj) => bidData?.SelectedItem != null && SelectedTabItem == 3

                 );
            }
        }

        /// <summary>
        /// редактировать статус заявки
        /// </summary>
        public RelayCommand EditBidStatusCommand
        {
            get
            {
                return editBidStatusCommand ??= new RelayCommand(obj =>
                {
                    if (obj != null && obj is Bid)
                    {
                        bidData.SelectedItem = obj as Bid;
                        foreach (BidStatus status in bidStatus)
                        {
                            status.IsChecked = bidData.SelectedItem.BidStatus == status.Status;
                        }
                        SelectedTabItem = 3;
                    }
                }
                , (obj) => bidData != null

                 );
            }
        }

        /// <summary>
        /// показать Рабочий стол
        /// </summary>
        public RelayCommand ShowDesctopCommand
        {
            get
            {
                return showDesctopCommand ??= new RelayCommand(async obj =>
                {
                    RefreshBidFilterData();
                    await GetSortedBidData();
                    SelectedTabItem = 2;
                }
                , (obj) => bidData != null && bidFilter?.Count > 0

                 );
            }
        }

        /// <summary>
        /// Обновить данные на вкладке Рабочий стол
        /// </summary>
        public RelayCommand RefreshDesctopDataCommand
        {
            get
            {
                return refreshDesctopDataCommand ??= new RelayCommand(async obj =>
                {
                    await GetSortedBidData(true);
                }
                , (obj) => bidData != null 
                           && bidFilter?.Count > 0 
                           && sortedBidData !=null
                           && SelectedTabItem == 2
                           && ((bidFilter[^1].IsChecked && bidFilter[^1].DateFrom <= bidFilter[^1].DateUpTo)
                           || (bidFilter.FirstOrDefault(i => i.IsChecked) != null && !bidFilter[^1].IsChecked))


                 );
            }
        }
        /// <summary>
        /// отправить заявку
        /// </summary>
        public RelayCommand SendBidCommand
        {
            get
            {
                return sendBidCommand ??= new RelayCommand(async obj =>
                {
                    bidData.EditObject.Date = DateTime.Now;
                    bidData.EditObject.BidStatus = BidStatusEnum.received;
                    if (await bidData.SaveObjectAsync())
                        GetBidPageData();                  
                }
                ,(obj) => bidData?.EditObject != null &&
                          !string.IsNullOrWhiteSpace(bidData.EditObject.Name) &&
                          !string.IsNullOrWhiteSpace(bidData.EditObject.Text) &&
                          !string.IsNullOrWhiteSpace(bidData.EditObject.UserEmail) &&
                          SkillProfiHelper.CheckStringByRegexPattern(bidData.EditObject.UserEmail, @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z")
                 );
            }
        }

        /// <summary>
        /// Показать страницу Заявки
        /// </summary>
        public RelayCommand ShowBidPageCommand
        {
            get
            {
                return showBidPageCommand ??= new RelayCommand(obj =>
                {
                    GetBidPageData();
                }
                ,(obj) => bidPageData != null
                 );
            }
        }

        /// <summary>
        /// Редактировать странцу заявок
        /// </summary>
        public RelayCommand EditBidPageCommand
        {
            get
            {
                return editBidPageCommand ??= new RelayCommand(obj =>
                {
                    SelectedTabItem = 1;
                }
                , (obj) => IsAuthorized && SelectedTabItem == 0
                 );
            }
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public RelayCommand SaveChangeBidPageCommand
        {
            get
            {
                return saveChangeBidPageCommand ??= new RelayCommand(async obj =>
                {
                    await bidPageData.SaveObjectAsync();
                    GetBidPageData();
                }
                , (obj) => IsAuthorized && SelectedTabItem == 1 /*&&
                          !string.IsNullOrWhiteSpace(bidPageData?.EditObject?.Header) && 
                          !string.IsNullOrWhiteSpace(bidPageData?.EditObject?.Description) */
                 );
            }
        }
        #endregion
        #endregion

        #region конструктор
        public SkillProfiViewModel(IDialogService service)
        {
            url = "";
            http_client = new();

            http_client.Timeout = new (0, 10 ,0);
            ServicePointManager.DefaultConnectionLimit = 10;

            string filePath = SkillProfiHelper.GetLocalFilePath(@"\appsettings.json");
            if (File.Exists(filePath))
            {
                using StreamReader reader = File.OpenText(filePath);
                JObject obj = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                url = (string)obj["DefaultConnection"];
            }
            dialogService = service;

            serviceData = new DataView<Service>(url.TrimEnd('/') + "/api/ServiceData", http_client);
            serviceData.ErrorMessage += ShowErrorMessage;
            serviceData.LoadData += OnLoadData;

            projectData = new DataView<Project>(url.TrimEnd('/') + "/api/ProjectData", http_client);
            projectData.ErrorMessage += ShowErrorMessage;
            projectData.LoadData += OnLoadData;

            blogData = new DataView<Blog>(url.TrimEnd('/') + "/api/BlogData", http_client);
            blogData.ErrorMessage += ShowErrorMessage;
            blogData.LoadData += OnLoadData;

            menuNameAndPageHeaderData = new DataView<MenuNameAndPageHeader>(url.TrimEnd('/') + "/api/MenuNameAndPageHeader", http_client);
            menuNameAndPageHeaderData.ErrorMessage += ShowErrorMessage;
            menuNameAndPageHeaderData.LoadData += OnLoadData;

            headerDescriptionData = new DataView<HeaderDescription>(url.TrimEnd('/') + "/api/HeaderQuote", http_client);
            headerDescriptionData.ErrorMessage += ShowErrorMessage;
            headerDescriptionData.LoadData += OnLoadData;

            contactData = new DataView<Contact>(url.TrimEnd('/') + "/api/ContactData", http_client);
            contactData.ErrorMessage += ShowErrorMessage;
            contactData.LoadData += OnLoadData;

            bidPageData = new DataView<BidPageData>(url.TrimEnd('/') + "/api/BidPage", http_client);
            bidPageData.ErrorMessage += ShowErrorMessage;
            bidPageData.LoadData += OnLoadData;

            bidData = new DataView<Bid>(url.TrimEnd('/') + "/api/BidData", http_client);
            bidData.ErrorMessage += ShowErrorMessage;
            bidData.LoadData += OnLoadData;

            bidFilter = new();
            bidFilter.Add(new() {Name = "today"});
            bidFilter.Add(new() { Name = "yesterday"});
            bidFilter.Add(new() { Name = "week" });
            bidFilter.Add(new() { Name = "month"});
            bidFilter.Add(new() { Name = "period"});

            bidStatus = new();
            foreach (BidStatusEnum bidstatus in Enum.GetValues(typeof(BidStatusEnum)))
            {
                bidStatus.Add(new()
                {
                    Status = bidstatus,
                    IsChecked = false
                });
            }

            //sortedBidData = new();
                userList = null;
            AuthorizedUser = null;
            LoadUserData();
            GetBidPageData();
            //Task.WaitAll(Task.Run(() => GetMenuName()));
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
