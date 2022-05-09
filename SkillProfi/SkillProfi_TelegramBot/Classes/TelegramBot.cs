using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Telegram.Bot.Types.ReplyMarkups;
using System.Globalization;
using SkillProfi_Shared;

namespace SkillProfi_TelegramBot.Classes
{
    public class TelegramBot
    {
        #region константы
        private const string date_format = "yyyyMMdd_HH-mm-ss-fff";
        private const string menu_main_callback = "main_menu";
        private const string menu_project_callback = "project_menu";
        private const string menu_service_callback = "service_menu";
        private const string menu_blog_callback = "blog_menu";
        private const string menu_contact_callback = "contact_menu";
        private const string blog_callback = "get_blog_";
        private const string service_callback = "get_service_";
        private const string project_callback = "get_project_";
        private const string bid_yes_callback = "bid_yes";
        private const string bid_no_callback = "bid_no";

        private const string help_text = "Это Telegramm бот компании #SkillProfi.\nВ нем Вы можете ознакомится с нашими услугами, "+
                                        "посмотреть проекты, почитать блог и оставить свое обращение или задать вопрос.\nДля начала работы наберите команду /start.\n"+
                                        "Желаем Вам приятного общения с нашим ботом.";
        #endregion

        #region поля

        private readonly TelegramBotClient bot_telegram;
        private readonly CancellationTokenSource cancellationToken;
        private readonly HttpClient http_client;
        private readonly string url;
        private readonly SkiillProfiWebAPI<MenuNameAndPageHeader> menuHeaderData;
        private readonly SkiillProfiWebAPI<Project> projectData;
        private readonly SkiillProfiWebAPI<Service> serviceData;
        private readonly SkiillProfiWebAPI<Blog> blogData;
        private readonly SkiillProfiWebAPI<Contact> contactData;
        private readonly SkiillProfiWebAPI<Bid> bidData;
        private readonly Dictionary<long, Bid> bid_dictonary;

        #endregion

        #region Методы
        /// <summary>
        /// Запуск ожидания запросов Telegram боту
        /// </summary>
        /// <param name="token">token телеграм бота</param>
        public async void Start()
        {
            try
            {
                // StartReceiving не блокирует поток вызывающего абонента. Прием осуществляется в ThreadPool.
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { } // получать все типы обновлений
                };
                bot_telegram.StartReceiving(
                                            HandleUpdateAsync,
                                            HandleErrorAsync,
                                            receiverOptions,
                                            cancellationToken: cancellationToken.Token);

                Telegram.Bot.Types.User me = await bot_telegram.GetMeAsync();
                Console.WriteLine($"Запуск ожидания запросов боту @{me.FirstName}");
                Console.WriteLine("Приложение запущено. Для завершения работы нажмите Ctrl + C");
                while (true)
                {
                    var k = Console.ReadKey(true);
                    if ((k.Modifiers & ConsoleModifiers.Control) != 0)
                    {
                        if ((k.Key & ConsoleKey.C) != 0)
                        {
                            StopBot();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddError(ex.InnerException?.Message??ex.Message);
                Console.WriteLine("Для завершения нажмите любую клавишу...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// запрос на отмену, чтобы остановить бота
        /// </summary>
        public void StopBot()
        {
            if (cancellationToken != null)
            try
            {
                // Отправим запрос на отмену, чтобы остановить бота
                cancellationToken.Cancel();
                cancellationToken.Dispose();
            }
            catch { }
        }


        /// <summary>
        /// Обработчик обновлений: Делегат, используемый для обработки типов Telegram.Bot.Types.Update
        /// </summary>
        /// <param name="botClient">Telegram bot клиент</param>
        /// <param name="update">входящее сообщение</param>
        /// <param name="cancellationToken">Распространяет уведомление о том, что операции должны быть отменены</param>
        /// <returns></returns>
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            //if (update?.Type != Telegram.Bot.Types.Enums.UpdateType.Message || update?.Message == null)
            //    return;
            #region обработка текстовых сообщений и команд
            if (update.Message?.Type == Telegram.Bot.Types.Enums.MessageType.Text && !string.IsNullOrWhiteSpace(update.Message?.Text))
            {
                await ProcessingCommandsAsync(update);
                return;
            }
            #endregion

            #region обработка текстовых сообщений и команд
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && update.CallbackQuery!= null)
            {
                await ProcessingCallbackQueryAsync(update.CallbackQuery);
                return;
            }
            #endregion
        }

        /// <summary>
        /// Обработчик ошибок: Делегат, используемый для обработки ошибок опроса
        /// </summary>
        /// <param name="botClient">Telegram bot клиент</param>
        /// <param name="exception">исключение</param>
        /// <param name="cancellationToken">Распространяет уведомление о том, что операции должны быть отменены</param>
        /// <returns></returns>
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            string ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Ошибка Telegram API:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            AddError(ErrorMessage);
            return Task.CompletedTask;
        }

        /// <summary>
        ///  обработка команд текстовых сообщений 
        /// </summary>
        /// <param name="update">входящее сообщение</param>
        /// <returns></returns>
        private async Task ProcessingCommandsAsync(Telegram.Bot.Types.Update update)
        {
            string messageText;
            if (update?.Message?.Chat != null)
            {
                messageText = update.Message.Text;
                string command = messageText;
                if (string.Equals(command?.Trim(), "/start", StringComparison.CurrentCultureIgnoreCase))
                {
                    messageText = "";
                    await SendStartMenu(update);
                }
                else if(string.Equals(command?.Trim(), "/help", StringComparison.CurrentCultureIgnoreCase))
                    messageText = help_text;
                else
                {
                    if (bid_dictonary.ContainsKey(update.Message.Chat.Id))
                    {
                        await AddBidDataAsync(update.Message.Chat.Id, messageText);
                        messageText = string.Empty;
                    }
                    else
                        messageText = string.IsNullOrEmpty(messageText) ? messageText : $"Вы сказали:\n\"{ messageText}\"\nкоманда не распознана!";
                }    
                    
                if (!string.IsNullOrWhiteSpace(messageText))
                    await SendMessageTextAsync(update.Message.Chat.Id, messageText);
            }
            
        }

        /// <summary>
        /// обработка обратных запросов от inline кнопок
        /// </summary>
        /// <param name="callbackQuery">обратный зхапрос от клавиатуры бота </param>
        /// <returns></returns>
        private async Task ProcessingCallbackQueryAsync(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            if (!string.IsNullOrWhiteSpace(callbackQuery?.Data) && callbackQuery?.Message?.Chat != null)
            {
                if (callbackQuery.Data.Equals(menu_main_callback))
                {
                    //TODO: заполняем форму заявки
                    await SendBidQwestionAsync(callbackQuery.Message.Chat.Id);
                }
                else if (callbackQuery.Data.Equals(menu_project_callback))
                {
                    //проекты
                    await SendAllProjectDataAsync(callbackQuery.Message.Chat.Id);
                }
                else if (callbackQuery.Data.Equals(menu_service_callback))
                {
                    //услуги
                    await SendAllServiceDataAsync(callbackQuery.Message.Chat.Id);
                }
                else if (callbackQuery.Data.Equals(menu_blog_callback))
                {
                    //блог
                    await SendAllBlogDataAsync(callbackQuery.Message.Chat.Id);
                }
                else if (callbackQuery.Data.Equals(menu_contact_callback))
                {
                    //контакты
                    await SendContactDataAsync(callbackQuery.Message.Chat.Id);
                }
                else if (callbackQuery.Data.StartsWith(project_callback))
                {
                    //проект подробное описание
                    await SendProjectAsync(callbackQuery.Message.Chat.Id, callbackQuery.Data);
                }
                else if (callbackQuery.Data.StartsWith(service_callback))
                {
                    //усдуга подробное писаное
                    await SendServiceAsync(callbackQuery.Message.Chat.Id, callbackQuery.Data);
                }
                else if (callbackQuery.Data.StartsWith(blog_callback))
                {
                    //блог подробное описание
                    await SendBlogAsync(callbackQuery.Message.Chat.Id, callbackQuery.Data);
                }
                else if (callbackQuery.Data.Equals(bid_yes_callback))
                {
                    //Заполенение данных формы заявки
                    if (bid_dictonary.ContainsKey(callbackQuery.Message.Chat.Id))
                        await SendMessageTextAsync(callbackQuery.Message.Chat.Id, "Вы еще не закончили ввод данных предыдущего обращения!");
                    else
                        bid_dictonary.Add(callbackQuery.Message.Chat.Id, new());
                    await AddBidDataAsync(callbackQuery.Message.Chat.Id, string.Empty);
                }
                else if (callbackQuery.Data.Equals(bid_no_callback))
                {
                    bid_dictonary.Remove(callbackQuery.Message.Chat.Id);
                    await SendMessageTextAsync(callbackQuery.Message.Chat.Id, "Вы можете отправить обращение в любой момент!");
                }

            }
        }

        /// <summary>
        /// стартовое меню (клавиатура)
        /// </summary>
        /// <param name="update">входящее сообщение</param>
        /// <returns></returns>
        private async Task SendStartMenu(Telegram.Bot.Types.Update update)
        {
            if (update?.Message?.Chat != null)
            {
                if (await GetMenuName())
                {
                    List<List<InlineKeyboardButton>> buttons = new();
                    buttons.Add(new() { InlineKeyboardButton.WithCallbackData(text: !string.IsNullOrEmpty(menuHeaderData.EditObject.MainPage) ? menuHeaderData.EditObject.MainPage : "Добавить заявку"
                                                                            , callbackData: menu_main_callback) });
                    buttons.Add(new() { InlineKeyboardButton.WithCallbackData(text: !string.IsNullOrEmpty(menuHeaderData.EditObject.ProjectPage) ? menuHeaderData.EditObject.ProjectPage : "Проекты"
                                                                            , callbackData: menu_project_callback) });
                    buttons.Add(new() { InlineKeyboardButton.WithCallbackData(text: !string.IsNullOrEmpty(menuHeaderData.EditObject.ServicePage) ? menuHeaderData.EditObject.ServicePage : "Услуги"
                                                                             , callbackData: menu_service_callback) });
                    buttons.Add(new() { InlineKeyboardButton.WithCallbackData(text: !string.IsNullOrEmpty(menuHeaderData.EditObject.BlogPage) ? menuHeaderData.EditObject.BlogPage : "Блог"
                                                                            , callbackData: menu_blog_callback) });
                    buttons.Add(new() { InlineKeyboardButton.WithCallbackData(text: !string.IsNullOrEmpty(menuHeaderData.EditObject.ContactPage) ? menuHeaderData.EditObject.ContactPage : "Контакты"
                                                                            , callbackData: menu_contact_callback) });    
                    InlineKeyboardMarkup inlineKeyboard = new(buttons);

                    _ = await bot_telegram.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: "SkillProfi Telegram Бот.\nВыбирайте действия нажимая на кнопки.",
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken.Token);
                }
                else
                    await SendMessageTextAsync(update.Message.Chat.Id, "Сервис занят попробуйте позже");
            }
        }

        /// <summary>
        /// получить имя файла по полному пути к нему
        /// </summary>
        /// <param name="filePath">путь к файлу</param>
        /// <returns>возвращает имя файла если он существует по указанному пути/ признак существования файла</returns>
        private static (string fileName, bool fileExists) GetFileName(string filePath)
        {
            FileInfo file = new(filePath);
            if (file.Exists)
                return (file.Name, true);
            return (string.Empty, false);
        }

        /// <summary>
        /// отправка тестового сообщения
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <param name="messageText">тестовое сообщение</param>
        /// <returns></returns>
        private async Task<Telegram.Bot.Types.Message> SendTextMessageAsync(long chatId, string messageText)
        {
            Telegram.Bot.Types.Message sentMessage = null;
            if (bot_telegram != null && cancellationToken != null && chatId > 0 && !string.IsNullOrEmpty(messageText?.Trim()) )
            {
                sentMessage = await bot_telegram.SendTextMessageAsync(
                     chatId: chatId,
                     text: messageText,
                     cancellationToken: cancellationToken.Token);
            }
            return sentMessage;
        }

        /// <summary>
        /// отправка тестового сообщения c разбиением по сообщениям если оно не умешается в одно сообщение
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <param name="messageText">тестовое сообщение</param>
        /// <returns></returns>
        public async Task SendMessageTextAsync(long chatId, string messageText)
        {
            //т.к. сообщения могут быть длинной больше чем 4096 символов, а телеграм не позволяет отправлять сообщения более 4096
            //разобъем сообщение на части которые сможем отправить
            if (messageText?.Length <= 4096)
            {
                //для нормального отображения будем делить текст по 50 строк
                string[] messageTextArray = messageText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (messageTextArray.Length > 50)
                {
                    string mess = string.Empty;
                    for (int i = 0; i < messageTextArray.Length; i++)
                    {
                        int endMess = i + 50 < messageTextArray.Length ? i + 50 : messageTextArray.Length ;
                        mess = string.Join('\n', messageTextArray[i..endMess]);
                        i = endMess - 1;
                        _ = await SendTextMessageAsync(chatId, mess);
                    }

                }
                else
                {
                    _ = await SendTextMessageAsync(chatId,  messageText);
                }
            }
            else
            {
                //разбиваем сообщение если оно больше 4096 символов
                for (int i = 0; i < messageText?.Length; i++)
                {
                    string mess = string.Empty;
                    int endMess = i + 4096 < messageText.Length ? i + 4096 : messageText.Length;
                    mess = messageText[i..endMess];
                    i = endMess - 1;
                    _ = await SendTextMessageAsync(chatId, mess);
                }
            }
        }

        /// <summary>
        /// Выгрузка файлов в Telegram
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <param name="filePath">путь к выгружаемому файл</param>
        /// <param name="message">сообщение отправляемое с документом</param>
        public async Task UploadImage(long chatId, string filePath, string message = "")
        {
            (string file_Name, _) = GetFileName(filePath);
            if (chatId > 0)
            {
                using FileStream file_Stream = new(filePath, FileMode.Open);
                
                Telegram.Bot.Types.ChatId chatID = new(chatId);
                Telegram.Bot.Types.InputFiles.InputOnlineFile doc = new(file_Stream, fileName: file_Name);
                _ = await bot_telegram.SendPhotoAsync(chatId: chatID, photo: doc, caption: message);
                file_Stream.Close();
            }
        }

        /// <summary>
        /// Выгрузка файлов в виде байтового массива в Telegram
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <param name="image">файл в виде массива байт</param>
        /// <param name="message">сообщение отправляемое с документом</param>
        public async Task UploadImage(long chatId, byte[] image, string message = "")
        {
            if (chatId > 0 && image?.Length > 0)
            {
                using Stream stream = new MemoryStream(image);

                Telegram.Bot.Types.ChatId chatID = new(chatId);
                Telegram.Bot.Types.InputFiles.InputOnlineFile doc = new(stream);//, @"\Data\image_" + DateTime.Now.ToString(date_format) + ".png"
                _ = await bot_telegram.SendPhotoAsync(chatId: chatID, photo: doc, caption: message);
                stream.Close();
            }
        }

        /// <summary>
        /// добавить сообщение с ошибкой в консоль
        /// </summary>
        /// <param name="Message"></param>
        public static void AddError(string Message)
        {
            if (!string.IsNullOrWhiteSpace(Message))
            {
                try{Console.ForegroundColor = ConsoleColor.Red;} catch { }
                Console.WriteLine(Message);
                try { Console.ResetColor(); } catch { }
            }
        }

        /// <summary>
        /// сохранение массива байт в изображение
        /// </summary>
        /// <param name="image">изображение в виде массива байт</param>
        /// <returns></returns>
        private async Task<string> SaveByteArrayToFile(byte[] image)
        {
            if (image?.Length > 0)
            {
                string file_path = SkillProfiHelper.GetLocalFilePath(@"\Data\image_" + DateTime.Now.ToString(date_format) + ".png");
                try
                {
                    await File.WriteAllBytesAsync(file_path, image, cancellationToken.Token);
                    return file_path;
                }
                catch(Exception ex) 
                {
                    AddError(ex.InnerException?.Message ?? ex.Message);
                }                
            }
            return string.Empty;
        }

        /// <summary>
        /// удаление файла
        /// </summary>
        /// <param name="file_path">путь к файлу</param>
        /// <returns></returns>
        private static async Task RemoveFileAsynk(string file_path)
        {
            if (File.Exists(file_path))
            {
                try
                {
                    await Task.Run(()=>File.Delete(file_path));
                }
                catch { }
            }
        }

        /// <summary>
        /// отправить станицу контакты
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <returns></returns>
        private async Task SendContactDataAsync(long chatId)
        {
            bool send_wait = false;
            if (chatId > 0)
            {
                if (await GetMenuName())
                {
                    (Contact contact, bool isnotError) = await contactData?.GetOneObjectDataAsync();
                    
                    if (isnotError)
                    {
                        if (contact != null)
                        {
                            contactData.SetEditObject(contact);
                            string address = $"{menuHeaderData.EditObject.ContactPageHeader}\n\n{SkillProfiHelper.GetStringWithOutHtml(contactData.EditObject.Address)}";

                            //ссылки в виде кнопок
                            List<List<InlineKeyboardButton>> buttons = new();
                            foreach (ContactLink link in contactData.EditObject.Links)
                            {
                                if (CheckURLValid(link.Description))
                                    buttons.Add(new() { InlineKeyboardButton.WithUrl(text: link.Description, url: link.Description) });
                            }
                            InlineKeyboardMarkup inlineKeyboard = new(buttons);

                            _ = await bot_telegram.SendTextMessageAsync(
                                chatId: chatId,
                                text: address,
                                replyMarkup: inlineKeyboard,
                                cancellationToken: cancellationToken.Token);
                            //foreach (ContactLink link in contactData.EditObject.Links)
                            //{
                            //    await UploadImage(chatId, link.Image,link.Description);
                            //}

                            //отправить рисунок
                            if (contactData.EditObject.Image?.Length > 0)
                                await UploadImage(chatId, contactData.EditObject.Image);
                        }
                        else
                            await SendMessageTextAsync(chatId, "Данные в контакты еще не добавлены");
                    }
                    else
                        send_wait = true;
                }
                else
                    send_wait = true;
                if(send_wait)
                    await SendMessageTextAsync(chatId, "Сервис занят попробуйте позже");
            }
        }

        /// <summary>
        /// получить все блоги
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <returns></returns>
        private async Task SendAllBlogDataAsync(long chatId)
        {
            bool send_wait = false;
            if (chatId > 0)
            {
                if (await GetMenuName())
                {
                    if (await blogData.UpdateDataCollectionAsync())
                    {
                        if (blogData?.DataCollection?.Count > 0)
                        {
                            var ci = new CultureInfo("ru-RU");
                            //заголовки всех блогов в виде кнопок
                            List<List<InlineKeyboardButton>> buttons = new();
                            foreach (Blog blog in blogData.DataCollection)
                            {
                                buttons.Add(new() { InlineKeyboardButton.WithCallbackData(text: blog.Header + "\n" + blog.Date.ToString("dd MMMM yyyy", ci), callbackData: blog_callback + blog.Id) });
                            }
                            InlineKeyboardMarkup inlineKeyboard = new(buttons);

                            _ = await bot_telegram.SendTextMessageAsync(
                                chatId: chatId,
                                text: string.IsNullOrEmpty(menuHeaderData.EditObject.BlogPageHeader) ? "Блог" : menuHeaderData.EditObject.BlogPageHeader,
                                replyMarkup: inlineKeyboard,
                                cancellationToken: cancellationToken.Token);
                        }
                        else
                            await SendMessageTextAsync(chatId, "Данные в блог еще не добавлены");
                    }
                    else
                        send_wait = true;
                }
                else
                    send_wait = true;
                if (send_wait)
                    await SendMessageTextAsync(chatId, "Сервис занят попробуйте позже");
            }
        }

        /// <summary>
        /// получить все услуги
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <returns></returns>
        private async Task SendAllServiceDataAsync(long chatId)
        {
            bool send_wait = false;
            if (chatId > 0)
            {                
                if (await GetMenuName())
                {
                    if (await serviceData.UpdateDataCollectionAsync())
                    {
                        if (serviceData?.DataCollection?.Count > 0)
                        {
                            //заголовки всех услуг в виде кнопок
                            List<List<InlineKeyboardButton>> buttons = new();
                            foreach (Service service in serviceData.DataCollection)
                            {
                                buttons.Add(new() { InlineKeyboardButton.WithCallbackData(text: service.Header, callbackData: service_callback + service.Id) });
                            }
                            InlineKeyboardMarkup inlineKeyboard = new(buttons);

                            _ = await bot_telegram.SendTextMessageAsync(
                                chatId: chatId,
                                text: string.IsNullOrEmpty(menuHeaderData.EditObject.ServicePageHeader) ? "Услуги" : menuHeaderData.EditObject.ServicePageHeader,
                                replyMarkup: inlineKeyboard,
                                cancellationToken: cancellationToken.Token);
                        }
                        else
                            await SendMessageTextAsync(chatId, "Данные в услуги еще не добавлены");
                    }
                    else
                        send_wait = true;
                }
                else
                    send_wait = true;
                if (send_wait)
                    await SendMessageTextAsync(chatId, "Сервис занят попробуйте позже");
            }
        }

        /// <summary>
        /// получить все проекты
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <returns></returns>
        private async Task SendAllProjectDataAsync(long chatId)

        {
            bool send_wait = false;
            if (chatId > 0)
            {
                if (await GetMenuName())
                {
                    if (await projectData.UpdateDataCollectionAsync())
                    {
                        if (projectData?.DataCollection?.Count > 0)
                        {
                            //заголовки всех проектов в виде кнопок
                            List<List<InlineKeyboardButton>> buttons = new();
                            foreach (Project project in projectData.DataCollection)
                            {
                                buttons.Add(new() { InlineKeyboardButton.WithCallbackData(text: project.Header, callbackData: project_callback + project.Id) });
                            }
                            InlineKeyboardMarkup inlineKeyboard = new(buttons);

                            _ = await bot_telegram.SendTextMessageAsync(
                                chatId: chatId,
                                text: string.IsNullOrEmpty(menuHeaderData.EditObject.ProjectPageHeader) ? "Проекты": menuHeaderData.EditObject.ProjectPageHeader,
                                replyMarkup: inlineKeyboard,
                                cancellationToken: cancellationToken.Token);
                        }
                        else
                            await SendMessageTextAsync(chatId, "Данные в проекты еще не добавлены");
                    }
                    else
                        send_wait = true;
                }
                else
                    send_wait = true;
                if (send_wait)
                    await SendMessageTextAsync(chatId, "Сервис занят попробуйте позже");
            }
        }

        /// <summary>
        /// отправить данные по запрошенной услуге
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <param name="callBackData">строка обратного вызова</param>
        /// <returns></returns>
        private async Task SendServiceAsync(long chatId, string callBackData)
        {
            if (chatId > 0 && !string.IsNullOrWhiteSpace(callBackData))
            {
                bool send_notFound = false;
                if (int.TryParse(callBackData.Replace(service_callback, ""), out int id))
                {
                    Service service = await serviceData.GetDataByIdAsync(id);
                    if (service != null)
                    {
                        string message = service.Header + "\n" + SkillProfiHelper.GetStringWithOutHtml(service.Description);
                        await SendMessageTextAsync(chatId, message);
                    }
                    else
                    send_notFound = true;
                }
                else
                    send_notFound = true;
                if (send_notFound)
                    await SendMessageTextAsync(chatId, "Услуга не найдена");
            }
        }

        /// <summary>
        /// отправить данные по запрошенному проекту
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <param name="callBackData">строка обратного вызова</param>
        /// <returns></returns>
        private async Task SendProjectAsync(long chatId, string callBackData)
        {
            if (chatId > 0 && !string.IsNullOrWhiteSpace(callBackData))
            {
                bool send_notFound = false;
                if (int.TryParse(callBackData.Replace(project_callback, ""), out int id))
                {
                    Project project = await projectData.GetDataByIdAsync(id);
                    if (project != null)
                    {
                        //отправить рисунок
                        if (project.Image?.Length>0)
                        {
                            await UploadImage(chatId, project.Image);
                        }
                        string message = project.Header + "\n" + SkillProfiHelper.GetStringWithOutHtml(project.Description);
                        await SendMessageTextAsync(chatId, message);
                    }
                    else
                        send_notFound = true;
                }
                else
                    send_notFound = true;
                if (send_notFound)
                    await SendMessageTextAsync(chatId, "Проект не найден");
            }
        }

        /// <summary>
        /// отправить данные по запрошенному блогу
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <param name="callBackData">строка обратного вызова</param>
        /// <returns></returns>
        private async Task SendBlogAsync(long chatId, string callBackData)
        {
            if (chatId > 0 && !string.IsNullOrWhiteSpace(callBackData))
            {
                bool send_notFound = false;
                if (int.TryParse(callBackData.Replace(blog_callback, ""), out int id))
                {
                    Blog blog = await blogData.GetDataByIdAsync(id);
                    if (blog != null)
                    {
                        if (blog.Image?.Length>0)
                        {
                            await UploadImage(chatId, blog.Image);

                        }
                        string message = blog.Header + "\n" + blog.Date.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")) + "\n" + SkillProfiHelper.GetStringWithOutHtml(blog.Description);
                        await SendMessageTextAsync(chatId, message);
                    }
                    else
                        send_notFound = true;
                }
                else
                    send_notFound = true;
                if (send_notFound)
                    await SendMessageTextAsync(chatId, "Блог не найден");
            }
        }

        /// <summary>
        /// добавление данных заявки пользователем
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <param name="data">добавляемые данные</param>
        /// <returns></returns>
        private async Task AddBidDataAsync(long chatId, string data)
        {
            if (chatId > 0)
            {
                string header = "Ввод данных для обращения.\n\n";
                bid_dictonary.TryGetValue(chatId, out Bid bid);
                if (bid != null)
                {
                    bool data_isNull = string.IsNullOrWhiteSpace(data);
                    if (string.IsNullOrEmpty(bid.Name))
                    {
                        if (data_isNull)
                            await SendMessageTextAsync(chatId, header+"Введите имя:");
                        else
                        {
                            bid.Name = data.Trim();
                            await SendMessageTextAsync(chatId, header+"Введите email:");
                        }
                    }
                    else if (string.IsNullOrEmpty(bid.UserEmail))
                    {
                        if (data_isNull)
                            await SendMessageTextAsync(chatId, header + "Введите email:");
                        else
                        {
                            if (SkillProfiHelper.CheckStringByRegexPattern(data,
                                    @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z"))
                            {
                                bid.UserEmail = data;
                                await SendMessageTextAsync(chatId, header + "Введите текст обращения:");
                            }
                            else
                            {
                                await SendMessageTextAsync(chatId, header + $"Введенный email \"{data}\" не корректный!\n Введите email повторно:");
                            }
                        }
                    }
                    else if (string.IsNullOrEmpty(bid.Text))
                    {
                        if (data_isNull)
                            await SendMessageTextAsync(chatId, header + "Введите текст обращения:");
                        else
                        {
                            bid.Text = data.Trim();
                        }                        
                    }

                    if (!string.IsNullOrEmpty(bid.Name) && !string.IsNullOrEmpty(bid.UserEmail) && !string.IsNullOrEmpty(bid.Text))
                    {
                        bid.Date = DateTime.Now;
                        bid.BidStatus = BidStatusEnum.received;
                        bidData.SetEditObject(bid);
                        bidData.IsNewObject = true;
                        if (await bidData.SaveObjectAsync())
                        {
                            await SendMessageTextAsync(chatId, "Обращение отправлено");
                        }
                        bid_dictonary.Remove(chatId);
                    }
                }
            }
        }

        /// <summary>
        /// запрос на начало заполенния формы заявки
        /// </summary>
        /// <param name="chatId">уникальный идентификатор чата</param>
        /// <returns></returns>
        private async Task SendBidQwestionAsync(long chatId)
        {
            if (chatId > 0)
            {
                List<InlineKeyboardButton> buttons = new();
                buttons.Add(InlineKeyboardButton.WithCallbackData(text: "Да", callbackData: bid_yes_callback));
                buttons.Add(InlineKeyboardButton.WithCallbackData(text: "Нет", callbackData: bid_no_callback));
                InlineKeyboardMarkup inlineKeyboard = new(buttons);

                _ = await bot_telegram.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Начать заполненеи формы?",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: cancellationToken.Token);
            }
        }
        /// <summary>
        /// проверка валидности URL
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static bool CheckURLValid(string source) => Uri.TryCreate(source, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;

        /// <summary>
        /// получить данные названий меню и страниц
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetMenuName()
        {
            (MenuNameAndPageHeader menu, bool is_noterror) = await menuHeaderData?.GetOneObjectDataAsync();
            if (is_noterror)
                menuHeaderData.SetEditObject(menu);
            return is_noterror;
        }
        
        #endregion

        #region Конструкторы
        public TelegramBot()
        {
            string token = "";
            string filePath = SkillProfiHelper.GetLocalFilePath(@"\appsettings.json");
            if (File.Exists(filePath))
            {
                using StreamReader reader = File.OpenText(filePath);
                JObject obj = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                url = (string)obj["DefaultConnection"];
                string botTokenPath = (string)obj["DefaultBotTokenPath"];
                if (string.IsNullOrWhiteSpace(botTokenPath))
                {
                    botTokenPath = SkillProfiHelper.GetLocalFilePath(@"\TelegramBotTorken.txt");
                }
                if (File.Exists(botTokenPath))
                    token = File.ReadAllText(botTokenPath);
                else
                    throw new ArgumentNullException("Файл с токеном не найден!");

            }
            if(string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException("Токен не может быть пустым!");
            bot_telegram = new(token.Trim());
            cancellationToken = new();
            http_client = new();

            serviceData = new SkiillProfiWebAPI<Service>(url.TrimEnd('/') + "/api/ServiceData", http_client);
            serviceData.ErrorMessage += AddError;

            projectData = new SkiillProfiWebAPI<Project>(url.TrimEnd('/') + "/api/ProjectData", http_client);
            projectData.ErrorMessage += AddError;

            blogData = new SkiillProfiWebAPI<Blog>(url.TrimEnd('/') + "/api/BlogData", http_client);
            blogData.ErrorMessage += AddError;

            menuHeaderData = new SkiillProfiWebAPI<MenuNameAndPageHeader>(url.TrimEnd('/') + "/api/MenuNameAndPageHeader", http_client);
            menuHeaderData.ErrorMessage += AddError;

            contactData = new SkiillProfiWebAPI<Contact>(url.TrimEnd('/') + "/api/ContactData", http_client);
            contactData.ErrorMessage += AddError;

            bidData = new SkiillProfiWebAPI<Bid>(url.TrimEnd('/') + "/api/BidData", http_client);
            bidData.ErrorMessage += AddError;

            bid_dictonary = new();
        }

        #endregion

    }
}
