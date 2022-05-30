using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace SkillProfi_WebSite.Classes
{
    public static class ExceptionView
    {
        /// <summary>
        /// обработка исключения и формирование сообщения
        /// </summary>
        /// <param name="ex"></param>
        public static IActionResult View(Exception ex, Controller controller)
        {
            if (controller != null && ex != null)
            {
                string exception_string;
                switch(ex)
                {
                    case InvalidOperationException:
                        exception_string = $"Ошибка URI.\n{ex.InnerException?.Message ?? ex.Message}";
                        break;
                    case System.Net.Http.HttpRequestException:
                        string err = "Ошибка выполнения запроса.\n";
                        if (((System.Net.Http.HttpRequestException)ex)?.StatusCode >= System.Net.HttpStatusCode.InternalServerError)
                            exception_string = $"{err}Внутренняя ошибка сервера.";
                        else
                            exception_string = $"{err}{ex.InnerException?.Message ?? ex.Message}";
                        break;
                    case TaskCanceledException:
                        exception_string = $"Отмена выполнения задачи.\n{ex.InnerException?.Message ?? ex.Message}";
                        break;
                    case Newtonsoft.Json.JsonSerializationException:
                        exception_string = $"Ошибка преобазования данных.\n{ex.InnerException?.Message ?? ex.Message}";
                        break;
                    default:
                        throw ex;
                };
                return controller.View("Error", exception_string);
            }
            return null;
        }
    }
}
